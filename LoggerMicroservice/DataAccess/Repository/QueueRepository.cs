using DataAccess.Interfaces;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DataAccess.Repository
{
    public class QueueRepository<TMessage> : IQueueRepository<TMessage> where TMessage : class
    {
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);
        private List<TMessage> messageList = new List<TMessage>();
        private readonly IConnection _connection;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly Lazy<IQueueRepository<ErrorLog>> _queueErrorRepository;
        private readonly Lazy<IQueueRepository<OtherLog>> _queueOtherRepository;

        uint msgCount = 0;
        uint counter = 0;

        public QueueRepository(IConnection connection, ILog4NetRepository log4NetRepository, Lazy<IQueueRepository<ErrorLog>> queueErrorRepository, Lazy<IQueueRepository<OtherLog>> queueOtherRepository)
        {
            _connection = connection;
            _log4NetRepository = log4NetRepository;
            _queueErrorRepository = queueErrorRepository;
            _queueOtherRepository = queueOtherRepository;
        }

        public List<TMessage> ConsumeQueue(string queue)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    var queueResult = channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel); //EventingBasicConsumer

                    consumer.Received += QueueReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    // Wait here until all messages are retrieved
                    msgsRecievedGate.Wait();

                    return messageList;
                }
            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = "BasicConsume",
                    Date = DateTime.Now,
                    Message = JsonConvert.SerializeObject(""),
                    QueueName = queue,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return new List<TMessage>();
            }
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string serializedObj = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(serializedObj);

                    channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: properties,
                                         body: body);
                }

                QueueLog queueLog = new QueueLog()
                {
                    OperationType = "BasicPublish",
                    Date = DateTime.Now,
                    ExchangeName = exchange,
                    Message = JsonConvert.SerializeObject(message),
                    QueueName = queue,
                    RoutingKey = routingKey
                };
                OtherLog otherLog = new OtherLog()
                {
                    queueLog = queueLog
                };
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";
                _log4NetRepository.Info(logText);

            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = "BasicPublish",
                    Date = DateTime.Now,
                    ExchangeName = exchange,
                    Message = JsonConvert.SerializeObject(message),
                    QueueName = queue,
                    RoutingKey = routingKey,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

            }
        }

        public void QueueReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            counter++;

            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            msgCount = e.Model.MessageCount("otherlogs");
            OtherLog queueMsg = JsonConvert.DeserializeObject<OtherLog>(message);

            e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            QueueLog queueLog = new QueueLog()
            {
                OperationType = "BasicConsume",
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = "converter", //Bu nereden gelir?
                RoutingKey = ea.RoutingKey
            };
            OtherLog otherLog = new OtherLog()
            {
                queueLog = queueLog
            };
            _queueOtherRepository.Value.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(otherLog)}";
            _log4NetRepository.Info(logText);

            if (counter == msgCount)
            {
                msgsRecievedGate.Set();

                return;
            }
        }
    }
}
