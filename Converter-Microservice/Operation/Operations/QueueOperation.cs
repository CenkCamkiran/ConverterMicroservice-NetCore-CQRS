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
        QueueRepository<object> queueRepository = new QueueRepository<object>();

        public List<QueueMessage> ConsumeQueue(string queue)
        {
            return queueRepository.ConsumeQueue(queue);
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            queueRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }
    }
}
