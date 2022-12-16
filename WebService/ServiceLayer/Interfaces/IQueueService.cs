using Models;

namespace ServiceLayer.Interfaces
{
    public interface IQueueService
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey);
    }
}
