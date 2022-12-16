using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface IQueueOperation
    {
        List<QueueMessage> ConsumeErrorLogsQueue(string queue);
        List<QueueMessage> ConsumeOtherLogsQueue(string queue);
    }
}
