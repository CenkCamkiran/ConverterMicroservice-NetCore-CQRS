using Configuration;
using Minio;
using Minio.DataModel;
using Models;
using Newtonsoft.Json;
using System;

namespace DataAccess.Repository
{
    public class ObjectStorageRepository
    {
        private Log4NetRepository log = new Log4NetRepository();

        public MinioClient ConnectMinio()
        {
            EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
            MinioConfiguration minioConfiguration = envVariablesHandler.GetMinioEnvVariables();

            MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(minioConfiguration.MinioHost)
                                    .WithCredentials(minioConfiguration.MinioAccessKey, minioConfiguration.MinioSecretKey)
                                    .WithSSL(false);

            return minioClient;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {

            MinioClient minioClient = ConnectMinio();

            ServerSideEncryption? sse = null;
            stream.Position = 0;

            Dictionary<string, string> metadata = new Dictionary<string, string>()
            {
                {
                    "Id", objectName
                },
                {
                    "FileLength", stream.Length.ToString()
                },
                {
                    "ContentType", contentType
                }
            };

            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType("video/mp4")
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = nameof(minioClient.PutObjectAsync),
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };
                LogOtherRepository logOtherRepository = new LogOtherRepository();
                await logOtherRepository.LogStorageOther(objectStorageLog);

            }
            catch (Exception exception)
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = nameof(minioClient.PutObjectAsync),
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now,
                    ExceptionMessage = exception.Message.ToString()
                };

                QueueRepository<ObjectStorageLog> queueHandler = new QueueRepository<ObjectStorageLog>();
                queueHandler.QueueMessageDirect(objectStorageLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(objectStorageLog)}";
                log.Info(logText);

                throw;
            }
            finally
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "PutObjectAsync",
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };

                QueueRepository<ObjectStorageLog> queueHandler = new QueueRepository<ObjectStorageLog>();
                queueHandler.QueueMessageDirect(objectStorageLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"{JsonConvert.SerializeObject(objectStorageLog)}";
                log.Info(logText);
            }

        }

        public async Task<SelectResponseStream> GetFileAsync(string bucketName, string objectName)
        {

            SelectResponseStream? responseStream = null;
            ServerSideEncryption? sse = null;
            MinioClient minioClient = ConnectMinio();

            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minioClient.Build().BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minioClient.Build().MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var args = new SelectObjectContentArgs()
                               .WithBucket(bucketName)
                               .WithObject(objectName)
                               .WithServerSideEncryption(sse);

                responseStream = await minioClient.SelectObjectContentAsync(args);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "SelectObjectContentAsync",
                    BucketName = bucketName,
                    ContentLength = responseStream != null ? responseStream.Stats.BytesReturned : 0,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };
                LogOtherRepository logOtherRepository = new LogOtherRepository();
                await logOtherRepository.LogStorageOther(objectStorageLog);

            }
            catch (Exception exception)
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "SelectObjectContentAsync",
                    BucketName = bucketName,
                    ObjectName = objectName,
                    Date = DateTime.Now,
                    ExceptionMessage = exception.Message.ToString()
                };

                QueueRepository<ObjectStorageLog> queueHandler = new QueueRepository<ObjectStorageLog>();
                queueHandler.QueueMessageDirect(objectStorageLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(objectStorageLog)}";
                log.Error(logText);

                throw;

            }

            return responseStream;
        }
    }
}
