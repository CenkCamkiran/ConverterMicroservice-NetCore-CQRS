namespace NotificationMicroservice.OperationLayer.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage : class
    {
        void ConsumeQueue(string queue, long messageTtl = 0);
        void QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey);
    }
}
