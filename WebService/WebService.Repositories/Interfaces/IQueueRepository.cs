using WebService.Models;

namespace WebService.Repositories.Interfaces
{
    public interface IQueueRepository
    {
        Task<bool> QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey, long messageTtl = 0);
    }
}
