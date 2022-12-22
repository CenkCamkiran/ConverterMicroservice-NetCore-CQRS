using DataAccess.Interfaces;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        IQueueRepository<TMessage> _queueRepository;

        public QueueOperation(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public List<TMessage> ConsumeQueue(string queue)
        {
            return _queueRepository.ConsumeQueue(queue);
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            _queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
