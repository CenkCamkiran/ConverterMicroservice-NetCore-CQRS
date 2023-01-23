namespace LoggerMicroservice.DataAccessLayer.Interfaces
{
    public interface IQueueRepository<TMessage> where TMessage : class
    {
        void ConsumeErrorLogsQueue(string queue);
        void ConsumeOtherLogsQueue(string queue);
        void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey);
    }
}
