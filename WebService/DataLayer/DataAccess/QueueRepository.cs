using DataLayer.Interfaces;
using Helpers;
using Minio;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataAccess
{
    public class QueueRepository: IQueueRepository
    {
        private readonly IConnection _rabbitConnection;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly ILoggingRepository<QueueLog> loggingRepository;

        public QueueRepository(IConnection rabbitConnection, ILog4NetRepository log4NetRepository, ILoggingRepository<QueueLog> loggingRepository)
        {
            _rabbitConnection = rabbitConnection;
            _log4NetRepository = log4NetRepository;
            this.loggingRepository = loggingRepository;
        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey)
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

                QueueLog queueLog = new QueueLog()
                {
                    DateTime = DateTime.Now,
                    Exchange = exchange,
                    Message = message,
                    QueueName = queue,
                    RoutingKey = routingKey
                };
                await loggingRepository.IndexDocAsync("", queueLog);
                
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
    }
}
