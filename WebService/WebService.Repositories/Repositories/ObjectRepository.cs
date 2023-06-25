using Minio;
using Minio.DataModel;
using Newtonsoft.Json;
using System.Net;
using WebService.Exceptions;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogRepository<ObjectStorageLog> _loggingRepository;

        public ObjectRepository(IMinioClient minioClient, ILogRepository<ObjectStorageLog> loggingRepository)
        {
            _minioClient = minioClient;
            _loggingRepository = loggingRepository;
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
                    "Fullname", objectName + ".mp4"
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
                    .WithContentType("video/mp4")
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };

                await LogStorage(objectStorageLog);

                return await Task.FromResult(true);

            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

        }

        public async Task LogStorage(ObjectStorageLog objectStorageLog)
        {
            await _loggingRepository.IndexDocAsync("webservice_objstorage_logs", objectStorageLog);

            string logText = $"BucketName: {objectStorageLog.BucketName} - ObjectName: {objectStorageLog.ObjectName} - Content Type: {objectStorageLog.ContentType}";
        }
    }
}
