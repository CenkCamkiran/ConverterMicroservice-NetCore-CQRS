using Models;
using RabbitMQ.Client;

namespace DataAccess.Interfaces
{
    public interface IQueueRepository<TMessage> where TMessage : class
    {
        Task<List<QueueMessage>> ConsumeQueueAsync(string queue);
        Task QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey);
    }
}
