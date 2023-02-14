using WebService.DataAccessLayer.Interfaces;
using WebService.Models;
using WebService.OperationLayer.Interfaces;

namespace WebService.OperationLayer.Operations
{
    public class QueueOperation : IQueueOperation
    {
        private IQueueRepository _queueRepository;

        public QueueOperation(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey)
        {
            await _queueRepository.QueueMessageDirectAsync(message, queue, exchange, routingKey);
        }
    }
}
