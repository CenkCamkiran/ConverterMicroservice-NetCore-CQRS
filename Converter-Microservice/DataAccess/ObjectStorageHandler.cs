using Configuration;
using Minio;
using Minio.DataModel;
using Models;
using System.IO;
using System.Net.Mime;

namespace DataAccess
{
    public class ObjectStorageHandler
    {
        private Logger log = new Logger();

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
                ElkLogging<ObjectStorageLog> elkLogging = new ElkLogging<ObjectStorageLog>();
                await elkLogging.IndexExceptionAsync("converter_objstorage_logs", objectStorageLog);

                string logText = $"BucketName: {bucketName} - ObjectName: {objectName} - Content Type: {contentType} - Content Length from Bytes: {stream.Length}";
                log.Info(logText);

            }
            catch (Exception exception)
            {
                ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converterservice_logs", exceptionModel);
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
                    OperationType = nameof(minioClient.SelectObjectContentAsync),
                    BucketName = bucketName,
                    ContentLength = responseStream.Stats.BytesReturned,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };
                ElkLogging<ObjectStorageLog> elkLogging = new ElkLogging<ObjectStorageLog>();
                await elkLogging.IndexExceptionAsync("converter_objstorage_logs", objectStorageLog);

                string logText = $"BucketName: {bucketName} - ObjectName: {objectName} - Content Length from Bytes: {responseStream.Stats.BytesReturned}";
                log.Info(logText);

                return responseStream;

            }
            catch (Exception exception)
            {
                ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converterservice_logs", exceptionModel);

                return responseStream;
            }
        }
    }
}
