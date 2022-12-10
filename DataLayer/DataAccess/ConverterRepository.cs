using DataLayer.Interfaces;
using Helpers;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;
using System.Security.Cryptography;
using System.Text;

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

        public void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey)
        {
            try
            {
                var channel = _rabbitConnection.CreateModel();

                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string serializedObj = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serializedObj);

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

        public async Task StoreFileAsync(string bucketName, string objectName, Stream fileStream, string contentType)
        {
            ServerSideEncryption? sse = null;
            Dictionary<string, string> metaData = new Dictionary<string, string>
            {
                { "Guid", objectName }
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
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType)
                    .WithHeaders(metaData)
                    .WithServerSideEncryption(sse);
                await _minioClient.Build().PutObjectAsync(putObjectArgs).ConfigureAwait(false);

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
