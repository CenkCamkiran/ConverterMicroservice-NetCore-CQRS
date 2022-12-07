using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;
using Helpers.ErrorHelper;
using Models.Errors;
using Newtonsoft.Json;
using System.Net;

namespace DataLayer.DataAccess
{
    public class ConverterRepository : IConverterRepository
    {

        private readonly IMinioClient _minioClient;

        public ConverterRepository(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<bool> QueueMessageDirectAsync(string message, string queue, string exchange, string routingKey)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StoreFileAsync(string bucketName, string location, string objectName, string fileName, string fileContent, string contentType)
        {
            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(fileName)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                Console.WriteLine("Successfully uploaded " + objectName);
            }
            catch (MinioException exception)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }
    }
}
