using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IQueueRepository<TMessage> where TMessage : class
    {
        Task<List<QueueMessage>> ConsumeOtherLogsQueueAsync(string queue);
        Task<List<QueueMessage>> ConsumeErrorLogsQueueAsync(string queue);
    }
}
