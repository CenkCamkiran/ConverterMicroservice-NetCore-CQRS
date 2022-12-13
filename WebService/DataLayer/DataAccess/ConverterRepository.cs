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
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly ILoggingRepository loggingRepository;

        public ConverterRepository(IMinioClient minioClient, IConnection rabbitConnection, ILog4NetRepository log4NetRepository, ILoggingRepository loggingRepository)
        {
            _minioClient = minioClient;
            _rabbitConnection = rabbitConnection;
            _log4NetRepository = log4NetRepository;
            this.loggingRepository = loggingRepository;
        }

        public void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey)
        {
            try
            {
                var channel = _rabbitConnection.CreateModel();
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string serializedObj = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serializedObj);

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                                     body: body);


                loggingRepository.
                string logText = $"Exchange: {exchange} - Queue: {queue} - Routing Key: {routingKey} - Message: (fileGuid: {message.fileGuid} && email: {message.email})";
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
