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

        public ConverterRepository(IMinioClient minioClient, IConnection rabbitConnection)
        {
            _minioClient = minioClient;
            _rabbitConnection = rabbitConnection;
        }

        public void QueueMessageDirect(string message, string queue, string exchange, string routingKey)
        {
            try
            {
                var channel = _rabbitConnection.CreateModel();

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
            catch (Exception exception)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            } 
        }

        //KODU DÜZENLE!
        public async Task<bool> StoreFileAsync(string bucketName, string location, string objectName, string filePath, string fileContent, string contentType)
        {

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
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName()
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
