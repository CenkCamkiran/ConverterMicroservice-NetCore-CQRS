using Configuration;
using DataAccess.Interfaces;
using Minio;
using Minio.DataModel;
using Models;

namespace DataAccess.Repository
{
    public class ObjectStorageRepository : IObjectStorageRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly ILoggingRepository _loggingOtherRepository;

        public ObjectStorageRepository(IMinioClient minioClient, ILoggingRepository loggingOtherRepository)
        {
            _minioClient = minioClient;
            _loggingOtherRepository = loggingOtherRepository;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
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
                bool found = await _minioClient.Build().BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.Build().MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(contentType)
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await _minioClient.Build().PutObjectAsync(putObjectArgs).ConfigureAwait(false);
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

                await _loggingOtherRepository.LogStorageOther(otherLog);

            }
            catch (Exception exception)
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = "PutObjectAsync",
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

                await _loggingOtherRepository.LogStorageError(errorLog);
            }
        }

        public async Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName)
        {

            ObjectDataModel? objDataModel = null;
            ServerSideEncryption? sse = null;

            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minioClient.Build().BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.Build().MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                string filePath = Path.Combine(Path.GetTempPath(), objectName + ".mp4");
                var args = new GetObjectArgs()
                               .WithBucket(bucketName)
                               .WithObject(objectName)
                               .WithFile(filePath);

                ObjectStat objStat = await _minioClient.Build().GetObjectAsync(args);

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

                await _loggingOtherRepository.LogStorageOther(otherLog);

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

                await _loggingOtherRepository.LogStorageError(errorLog);

            }

            return objDataModel;
        }
    }
}
