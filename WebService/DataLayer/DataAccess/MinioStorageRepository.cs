using DataLayer.Interfaces;
using Helpers;
using Minio.DataModel;
using Minio;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataAccess
{
    public class MinioStorageRepository: IMinioStorageRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly ILoggingRepository<ObjectStorageLog> _loggingRepository;

        public MinioStorageRepository(IMinioClient minioClient, ILog4NetRepository log4NetRepository, ILoggingRepository<ObjectStorageLog> loggingRepository)
        {
            _minioClient = minioClient;
            _log4NetRepository = log4NetRepository;
            _loggingRepository = loggingRepository;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
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
                    .WithContentType("video/mp4")
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await _minioClient.Build().PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

                ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                {
                    BucketName = bucketName,
                    ContentLength = stream.Length,
                    ContentType = contentType,
                    ObjectName = objectName,
                    Date = DateTime.Now
                };
                await _loggingRepository.IndexDocAsync("webservice_objstorage_logs", objectStorageLog);

                string logText = $"BucketName: {bucketName} - ObjectName: {objectName} - Content Type: {contentType} - Content Length from Bytes: {stream.Length}";
                _log4NetRepository.Info(logText);

            }
            catch (Exception exception)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

        }
    }
}
