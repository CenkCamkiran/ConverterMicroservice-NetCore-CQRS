using Models;
using RabbitMQ.Client;

namespace DataAccess.Interfaces
{
    public interface IQueueRepository<TMessage> where TMessage : class
    {
        List<QueueMessage> ConsumeQueue(string queue);
        void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey);
    }
}
