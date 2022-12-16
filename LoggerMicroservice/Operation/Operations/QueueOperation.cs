using DataAccess.Repository;
using Models;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
