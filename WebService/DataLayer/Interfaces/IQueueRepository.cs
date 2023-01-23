using WebService.Models;

namespace WebService.DataAccessLayer.Interfaces
{
    public interface IQueueRepository
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey, long messageTtl = 0);
    }
}
