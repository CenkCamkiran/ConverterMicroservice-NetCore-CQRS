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
using RabbitMQ.Client;

namespace DataLayer.DataAccess
{
    public class ConverterRepository : IConverterRepository
    {

        private readonly IMinioClient _minioClient;
        private readonly IConnection _rabbitConnection;

        public ConverterRepository(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public ConverterRepository(IConnection rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
        }

        //KODU DÜZENLE!
        public async Task<bool> QueueMessageDirectAsync(string message, string queue, string exchange, string routingKey)
        {
            using (var channel = _rabbitConnection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }

            return true;    
        }

        //KODU DÜZENLE!
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
