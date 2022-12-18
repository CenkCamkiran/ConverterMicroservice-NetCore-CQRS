using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage : class
    {
        Task<List<QueueMessage>> ConsumeQueueAsync(string queue);
        Task QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey);
    }
}
