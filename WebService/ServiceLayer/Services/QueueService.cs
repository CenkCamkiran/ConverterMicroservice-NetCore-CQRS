using DataLayer.Interfaces;
using Models;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
