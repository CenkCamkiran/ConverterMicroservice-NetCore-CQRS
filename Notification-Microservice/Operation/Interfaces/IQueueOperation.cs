using Models;

namespace Operation.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage : class
    {
        Task<List<QueueMessage>> ConsumeQueueAsync(string queue);
        Task QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey);
    }
}
