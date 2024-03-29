﻿using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Newtonsoft.Json;
using System.Net;
using WebService.Common.Constants;
using WebService.Common.Events;
using WebService.Exceptions;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
{
    public class ObjectStorageRepository : IObjectStorageRepository
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogRepository<ObjectStorageLog> _loggingRepository;
        private readonly ILogger<ObjectStorageRepository> _logger;

        public ObjectStorageRepository(IMinioClient minioClient, ILogRepository<ObjectStorageLog> loggingRepository, ILogger<ObjectStorageRepository> logger)
        {
            _minioClient = minioClient;
            _loggingRepository = loggingRepository;
            _logger = logger;
        }

        public async Task<bool> PutObjectAsync(string bucketName, string objectName, Stream stream, string contentType)
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

                await _loggingRepository.IndexDocAsync(ProjectConstants.ObjectStorageLogsIndex, objectStorageLog);

                return await Task.FromResult(true);

            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                _logger.LogError(error.ErrorCode, exception.Message.ToString());

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

        }

    }
}
