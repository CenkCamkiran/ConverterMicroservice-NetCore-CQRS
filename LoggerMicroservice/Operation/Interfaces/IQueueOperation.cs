using Models;

namespace Operation.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage: class
    {
        List<TMessage> ConsumeQueue(string queue);
    }
}
