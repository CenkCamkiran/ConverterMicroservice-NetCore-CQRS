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
        IQueueRepository<object> _queueRepository;

        public QueueOperation(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public List<QueueMessage> ConsumeQueue(string queue)
        {
            return _queueRepository.ConsumeQueue(queue);
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            _queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
