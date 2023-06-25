using Converter_Microservice.Common.Events;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using Minio;
using Minio.DataModel;
using Newtonsoft.Json;
using WebService.Common.Constants;

namespace Converter_Microservice.Repositories.Repositories
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly Lazy<IQueueRepository> _queueErrorRepository;
        private readonly Lazy<IQueueRepository> _queueOtherRepository;

        public ObjectRepository(IMinioClient minioClient, ILog4NetRepository log4NetRepository, Lazy<IQueueRepository> queueErrorRepository, Lazy<IQueueRepository> queueOtherRepository)
        {
            _minioClient = minioClient;
            _log4NetRepository = log4NetRepository;
            _queueErrorRepository = queueErrorRepository;
            _queueOtherRepository = queueOtherRepository;
        }

        public async Task<bool> StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
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
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.OtherLogsServiceExchangeTtl);

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";
                _log4NetRepository.Info(logText);

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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey, ProjectConstants.ErrorLogsServiceExchangeTtl);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                throw;
            }
        }

        public async Task<ObjectData> GetFileAsync(string bucketName, string objectName)
        {
            ObjectData? objDataModel = null;
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

                string mp4FileFullPath = Path.Combine(Path.GetTempPath(), objectName + ".mp4");
                var args = new GetObjectArgs()
                               .WithBucket(bucketName)
                               .WithObject(objectName)
                               .WithFile(mp4FileFullPath);

                ObjectStat objStat = await _minioClient.Build().GetObjectAsync(args);

                objDataModel = new ObjectData()
                {
                    Mp4FileFullPath = mp4FileFullPath,
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
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.OtherLogsServiceExchangeTtl);

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";
                _log4NetRepository.Info(logText);

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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey, ProjectConstants.ErrorLogsServiceExchangeTtl);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

            }

            return objDataModel;
        }
    }
}
