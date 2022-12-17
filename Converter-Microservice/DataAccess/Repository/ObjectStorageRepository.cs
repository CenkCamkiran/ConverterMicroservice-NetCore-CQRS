using Configuration;
using DataAccess.Interfaces;
using Minio;
using Minio.DataModel;
using Models;

namespace DataAccess.Repository
{
    public class ObjectStorageRepository : IObjectStorageRepository
    {
        private Log4NetRepository log = new Log4NetRepository();

        private MinioClient ConnectMinio()
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
                    "FileByteLength", stream.Length.ToString()
                },
                {
                    "ContentType", contentType
                },
                {
                    "ModifiedDate", DateTime.Now.ToString()
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
                    .WithContentType(contentType)
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "PutObjectAsync",
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };
                OtherLog otherLog = new OtherLog()
                {
                    storageLog = objectStorageLog
                };

                LoggingHelperRepository logOtherRepository = new LoggingHelperRepository();
                await logOtherRepository.LogStorageOther(otherLog);

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
                ErrorLog errorLog = new ErrorLog()
                {
                    storageLog = objectStorageLog
                };

                LoggingHelperRepository logOtherRepository = new LoggingHelperRepository();
                await logOtherRepository.LogStorageError(errorLog);
            }
        }

        public async Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName)
        {

            ObjectDataModel? objDataModel = null;
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

                string filePath = Path.Combine(Path.GetTempPath(), objectName + ".mp4");
                var args = new GetObjectArgs()
                               .WithBucket(bucketName)
                               .WithObject(objectName)
                               .WithFile(filePath);

                ObjectStat objStat = await minioClient.GetObjectAsync(args);

                objDataModel = new ObjectDataModel()
                {
                    FileFullPath = filePath,
                    ObjectStats = objStat
                };

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "GetObjectAsync",
                    BucketName = bucketName,
                    ContentLength = objStat != null ? objStat.Size : 0,
                    ObjectName = objectName,
                    Date = DateTime.Now,
                    ContentType = objStat != null ? objStat.ContentType : ""
                };
                OtherLog otherLog = new OtherLog()
                {
                    storageLog = objectStorageLog
                };

                LoggingHelperRepository logOtherRepository = new LoggingHelperRepository();
                await logOtherRepository.LogStorageOther(otherLog);

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
                ErrorLog errorLog = new ErrorLog()
                {
                    storageLog = objectStorageLog
                };

                LoggingHelperRepository logOtherRepository = new LoggingHelperRepository();
                await logOtherRepository.LogStorageError(errorLog);

            }

            return objDataModel;
        }
    }
}
