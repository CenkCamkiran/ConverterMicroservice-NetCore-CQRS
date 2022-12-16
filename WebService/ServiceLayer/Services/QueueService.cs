using DataLayer.Interfaces;
using Models;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services
{
    public class QueueService : IQueueService
    {
        private IQueueRepository _queueRepository;

        public QueueService(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey)
        {
            await _queueRepository.QueueMessageDirectAsync(message, queue, exchange, routingKey);
        }
    }
}
