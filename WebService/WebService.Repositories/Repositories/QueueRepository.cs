using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;
using System.Text;
using WebService.Common.Constants;
using WebService.Common.Events;
using WebService.Exceptions;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly IConnection _rabbitConnection;
        private readonly ILogRepository<QueueLog> _logRepository;
        private readonly ILogger<QueueRepository> _logger;

        public QueueRepository(IConnection rabbitConnection, ILogRepository<QueueLog> logRepository, ILogger<QueueRepository> logger)
        {
            _rabbitConnection = rabbitConnection;
            _logRepository = logRepository;
            _logger = logger;
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
                        Message = serializedObj,
                        QueueName = ProjectConstants.ConverterServiceQueueName,
                        RoutingKey = routingKey
                    };
                    await _logRepository.IndexDocAsync(ProjectConstants.QueueLogsIndex, queueLog);
                }

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
