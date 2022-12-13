using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IQueueService
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey);
    }
}
