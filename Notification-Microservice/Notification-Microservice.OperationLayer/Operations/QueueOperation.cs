using Interfaces;

namespace Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        private IQueueRepository<object> _queueRepository;

        public QueueOperation(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public void ConsumeQueue(string queue, long messageTtl = 0)
        {
            _queueRepository.ConsumeQueue(queue, messageTtl);
        }

        public void QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey)
        {
            _queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
