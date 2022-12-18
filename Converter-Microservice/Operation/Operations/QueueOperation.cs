using DataAccess.Interfaces;
using DataAccess.Repository;
using log4net;
using Models;
using Newtonsoft.Json;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
{
    public class QueueOperation<TMessage> : IQueueOperation<TMessage> where TMessage : class
    {
        private IQueueRepository<object> _queueRepository;

        public QueueOperation(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<List<QueueMessage>> ConsumeQueueAsync(string queue)
        {
            return await _queueRepository.ConsumeQueueAsync(queue);
        }

        public async Task QueueMessageDirectAsync(TMessage message, string queue, string exchange, string routingKey)
        {
            await _queueRepository.QueueMessageDirectAsync(message, queue, exchange, routingKey);
        }
    }
}
