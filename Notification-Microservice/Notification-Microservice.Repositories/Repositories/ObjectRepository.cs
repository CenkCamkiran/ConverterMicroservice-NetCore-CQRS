using Minio;
using Minio.DataModel;
using Newtonsoft.Json;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Common.Events;
using Notification_Microservice.Repositories.Interfaces;
using NotificationMicroservice.Models;

namespace Notification_Microservice.Repositories.Repositories
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly Lazy<IQueueRepository> _queueErrorRepository;
        private readonly Lazy<IQueueRepository> _queueOtherRepository;

        public ObjectRepository(IMinioClient minioClient, Lazy<IQueueRepository> queueErrorRepository, Lazy<IQueueRepository> queueOtherRepository)
        {
            _minioClient = minioClient;
            _queueErrorRepository = queueErrorRepository;
            _queueOtherRepository = queueOtherRepository;
        }

        public async Task<bool> StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            IServerSideEncryption? sse = null;
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
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(contentType)
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = LogEvents.PubObjectEvent,
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
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey);

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";

                return await Task.FromResult(true);

            }
            catch (Exception exception)
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = LogEvents.PubObjectEvent,
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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";

                throw;
            }
        }

        public async Task<ObjectData> GetFileAsync(string bucketName, string objectName)
        {
            ObjectData? objDataModel = null;

            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                string filePath = Path.Combine(Path.GetTempPath(), objectName + ".mp3"); //Get mp3 file from object storage
                var args = new GetObjectArgs()
                               .WithBucket(bucketName)
                               .WithObject(objectName)
                               .WithFile(filePath);

                ObjectStat objStat = await _minioClient.GetObjectAsync(args);

                objDataModel = new ObjectData()
                {
                    Mp3FileFullPath = filePath,
                    ObjectStats = objStat
                };

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = LogEvents.GetObjectEvent,
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
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey);

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";

            }
            catch (Exception exception)
            {
                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    OperationType = LogEvents.GetObjectEvent,
                    BucketName = bucketName,
                    ObjectName = objectName,
                    Date = DateTime.Now,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    storageLog = objectStorageLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";

            }

            return objDataModel;
        }
    }
}
