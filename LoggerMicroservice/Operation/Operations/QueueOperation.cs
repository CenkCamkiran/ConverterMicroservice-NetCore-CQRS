using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation : IQueueOperation
    {
        IQueueRepository<object> _queueRepository;

        public QueueOperation(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public List<QueueMessage> ConsumeErrorLogsQueue(string queue)
        {
            return _queueRepository.ConsumeQueue(queue);
        }

        public List<QueueMessage> ConsumeOtherLogsQueue(string queue)
        {
            return _queueRepository.ConsumeQueue(queue);
        }
    }
}
