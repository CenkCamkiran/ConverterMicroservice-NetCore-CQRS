namespace Operation.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage : class
    {
        void ConsumeQueue(string queue, long messageTtl = 0);
        void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey);
    }
}
