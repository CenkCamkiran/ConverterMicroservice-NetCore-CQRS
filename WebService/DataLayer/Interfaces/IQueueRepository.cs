using Models;

namespace DataLayer.Interfaces
{
    public interface IQueueRepository
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey, long messageTtl = 0);
    }
}
