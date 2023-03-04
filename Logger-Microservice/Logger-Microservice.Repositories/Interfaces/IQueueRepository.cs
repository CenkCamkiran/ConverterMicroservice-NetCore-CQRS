namespace Logger_Microservice.Repositories.Interfaces
{
    public interface IQueueRepository
    {
        void ConsumeErrorLogsQueue(string queue);
        void ConsumeOtherLogsQueue(string queue);
        void QueueMessageDirect(object message, string queue, string exchange, string routingKey);
    }
}
