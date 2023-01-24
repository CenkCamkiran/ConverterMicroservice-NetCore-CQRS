using Interfaces;

namespace Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        IQueueRepository<TMessage> _queueRepository;

        public QueueOperation(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public void ConsumeErrorLogsQueue(string queue)
        {
            _queueRepository.ConsumeErrorLogsQueue(queue);
        }

        public void ConsumeOtherLogsQueue(string queue)
        {
            _queueRepository.ConsumeOtherLogsQueue(queue);
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            _queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
