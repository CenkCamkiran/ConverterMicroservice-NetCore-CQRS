using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;
using System.Text;
using WebService.DataAccessLayer.Interfaces;
using WebService.Models;

namespace WebService.DataAccessLayer.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly IConnection _rabbitConnection;
        private readonly ILogOtherRepository _logOtherRepository;

        public QueueRepository(IConnection rabbitConnection, ILogOtherRepository logOtherRepository)
        {
            _rabbitConnection = rabbitConnection;
            _logOtherRepository = logOtherRepository;
        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey, long messageTtl = 0)
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
                                     arguments: new Dictionary<string, object>()
                                     {
                                         {
                                             "x-message-ttl", 43200000
                                         }
                                     });

                string serializedObj = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serializedObj);

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                body: body);

                QueueLog queueLog = new QueueLog()
                {
                    Date = DateTime.Now,
                    ExchangeName = exchange,
                    Message = message,
                    QueueName = queue,
                    RoutingKey = routingKey
                };
                await _logOtherRepository.LogQueueOther(queueLog);

            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }
    }
}
