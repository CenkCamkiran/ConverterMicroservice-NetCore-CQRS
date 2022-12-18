using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        private IQueueRepository<object> _queueRepository;

        public QueueOperation(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<List<QueueMessage>> ConsumeQueueAsync(string queue)
        {
            return await _queueRepository.ConsumeQueueAsync(queue);
        }

        public async Task QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey)
        {
            await _queueRepository.QueueMessageDirectAsync(message, queue, exchange, routingKey);
        }
    }
}
