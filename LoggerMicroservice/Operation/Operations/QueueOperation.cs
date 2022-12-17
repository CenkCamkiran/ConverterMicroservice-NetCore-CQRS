using DataAccess.Repository;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation : IQueueOperation
    {
        QueueRepository<object> queueRepository = new QueueRepository<object>();

        public List<QueueMessage> ConsumeErrorLogsQueue(string queue)
        {
            return queueRepository.ConsumeQueue(queue);
        }

        public List<QueueMessage> ConsumeOtherLogsQueue(string queue)
        {
            return queueRepository.ConsumeQueue(queue);
        }
    }
}
