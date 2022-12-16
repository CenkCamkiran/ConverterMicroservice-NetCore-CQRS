using Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface IQueueRepository<TMessage> where TMessage : class
    {
        List<QueueMessage> ConsumeQueue(string queue);
        IConnection ConnectRabbitMQ();
        void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey);
    }
}
