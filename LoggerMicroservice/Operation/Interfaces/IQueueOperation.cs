using Models;

namespace Operation.Interfaces
{
    public interface IQueueOperation<TMessage> where TMessage: class
    {
        Task<List<TMessage>> ConsumeQueue(string queue);
    }
}
