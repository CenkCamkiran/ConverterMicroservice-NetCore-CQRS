using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;
using System.Text;
using WebService.Common.Constants;
using WebService.Exceptions;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
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

        public async Task<bool> QueueMessageDirectAsync(QueueMessage message, string exchange, string routingKey, long messageTtl = 0)
        {
            try
            {
                using (var channel = _rabbitConnection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

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
                        QueueName = ProjectConstants.ConverterServiceQueueName,
                        RoutingKey = routingKey
                    };
                    await _logOtherRepository.LogQueueOther(queueLog);
                }

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
    }
}
