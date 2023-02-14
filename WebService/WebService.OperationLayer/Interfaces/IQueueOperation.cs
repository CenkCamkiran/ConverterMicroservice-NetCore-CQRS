using WebService.Models;

namespace WebService.OperationLayer.Interfaces
{
    public interface IQueueOperation
    {
        Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey);
    }
}
