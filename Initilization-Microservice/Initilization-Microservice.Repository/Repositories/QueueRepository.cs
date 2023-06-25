using Initilization_Microservice.Common.Events;
using Initilization_Microservice.Helpers;
using Initilization_Microservice.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Initilization_Microservice.Repository.Repositories
{
    public class QueueRepository : IQueueRepository
    {

        private readonly IConnection _connection;
        private readonly ILogger<QueueRepository> _logger;

        public QueueRepository(IConnection connection, ILogger<QueueRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public bool ConfigureExchange(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);

                    exchangeResult = true;

                    _logger.LogInformation(LogEvents.ExchangeCreationPhase, exchange + " " + LogEvents.ExchangeCreationPhaseMessage);
                    return exchangeResult;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.ExchangeCreationPhaseError, exception.Message.ToString());
                throw new JobInitializerException(exception.Message.ToString());
            }
        }

        public bool ConfigureQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

                    exchangeResult = true;

                    _logger.LogInformation(LogEvents.QueueCreationPhase, queue + " " + LogEvents.QueueCreationPhaseMessage);
                    return exchangeResult;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.QueueCreationPhaseError, exception.Message.ToString());
                throw new JobInitializerException(exception.Message.ToString());
            }
        }

        public bool BindQueueToExchange(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueBind(queue, exchange, routingKey, arguments);

                    exchangeResult = true;

                    _logger.LogInformation(LogEvents.QueueExchangeBindingPhase, queue + " - " + exchange + " " + LogEvents.QueueExchangeBindingPhaseMessage);
                    return exchangeResult;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.QueueExchangeBindingPhaseError, exception.Message.ToString());
                throw new JobInitializerException(exception.Message.ToString());
            }
        }

    }
}
