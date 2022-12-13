using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IQueueRepository
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey);
    }
}
