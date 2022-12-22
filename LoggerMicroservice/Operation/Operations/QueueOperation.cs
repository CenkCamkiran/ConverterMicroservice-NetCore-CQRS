using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        IQueueRepository<TMessage> _queueRepository;

        public QueueOperation(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<List<TMessage>> ConsumeQueue(string queue)
        {
            return await _queueRepository.ConsumeQueue(queue); 
        }
    }
}
