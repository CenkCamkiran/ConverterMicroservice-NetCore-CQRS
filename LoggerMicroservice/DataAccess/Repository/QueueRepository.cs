using DataAccess.Interfaces;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class QueueRepository<TMessage> : IQueueRepository<TMessage> where TMessage : class
    {
        private List<QueueMessage> messageList = new List<QueueMessage>();
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);

        private readonly IConnection _connection;
        private readonly ILog4NetRepository _log4NetRepository;

        uint msgCount = 0;
        uint counter = 0;

        public QueueRepository(IConnection connection, ILog4NetRepository log4NetRepository)
        {
            _connection = connection;
            _log4NetRepository = log4NetRepository;
        }

        public async Task<List<QueueMessage>> ConsumeErrorLogsQueueAsync(string queue)
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

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += ErrorLogsQueue_ReceivedEvent;


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
                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return new List<QueueMessage>();
            }
        }

        public async Task<List<QueueMessage>> ConsumeOtherLogsQueueAsync(string queue)
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

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += OtherLogsQueue_ReceivedEvent;


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
                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return new List<QueueMessage>();
            }
        }

        public void OtherLogsQueue_ReceivedEvent(object se, BasicDeliverEventArgs ea)
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
            string logText = $"{JsonConvert.SerializeObject(otherLog)}";
            _log4NetRepository.Info(logText);

            if (counter == msgCount)
            {
                msgsRecievedGate.Set();

                return;
            }
        }

        public void ErrorLogsQueue_ReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            counter++;

            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            msgCount = e.Model.MessageCount("errorlogs");
            ErrorLog queueMsg = JsonConvert.DeserializeObject<ErrorLog>(message);

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
