namespace Notification_Microservice.Repositories.Interfaces
{
    public interface IQueueRepository
    {
        void ConsumeQueue(string queue, long messageTtl = 0);
        void QueueMessageDirect(object message, string queue, string exchange, string routingKey);
    }
}
