using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        private IQueueRepository<TMessage> _queueRepository;

        public QueueOperation(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public void ConsumeQueue(string queue)
        {
            _queueRepository.ConsumeQueue(queue);
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            _queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
