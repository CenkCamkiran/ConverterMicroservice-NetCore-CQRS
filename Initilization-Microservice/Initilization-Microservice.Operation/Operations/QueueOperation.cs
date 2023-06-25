using Initilization_Microservice.Common;
using Initilization_Microservice.Operation.Interfaces;
using Initilization_Microservice.Repository.Interfaces;

namespace Initilization_Microservice.Operation.Operations
{
    public class QueueOperation : IQueueOperation
    {

        private readonly IQueueRepository _queueRepository;

        public QueueOperation(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public Task<bool> ConfigureExchangeAndQueueAsync(string exchange, string exchangeType, bool exchangeDurable, bool exchangeAutoDelete, string queue, bool queueDurable, bool queueExclusive, bool queueAutoDelete, string routingKey, IDictionary<string, object> arguments = null)
        {
            Dictionary<string, object> exchangeArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", ProjectConstants.ExchangeTtl }
            };

            bool queueResult = false;
            bool exchangeResult = false;

            exchangeResult = _queueRepository.ConfigureExchange(exchange, exchangeType, exchangeDurable, exchangeAutoDelete, exchangeArgs);

            if (exchangeResult)
                queueResult = _queueRepository.ConfigureQueue(queue, queueDurable, queueExclusive, queueAutoDelete, null);

            if (queueResult)
            {
                bool bindResult = _queueRepository.BindQueueToExchange(queue, exchange, routingKey, null);
                if (bindResult)
                    return Task.FromResult(true);

                else
                    return Task.FromResult(false);
            }
            else
                return Task.FromResult(false);
        }

    }
}
