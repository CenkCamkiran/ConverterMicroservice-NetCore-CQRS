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
        private IQueueService _queueService;

        public QueueService(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey)
        {
            await _queueService.QueueMessageDirectAsync(message, queue, exchange, routingKey);
        }
    }
}
