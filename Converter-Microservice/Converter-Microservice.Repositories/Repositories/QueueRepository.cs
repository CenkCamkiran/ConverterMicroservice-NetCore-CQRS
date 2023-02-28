using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Converter_Microservice.Repositories.Repositories
{
    public class QueueRepository<TMessage> : IQueueRepository<TMessage> where TMessage : class
    {
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);

        private readonly IConnection _connection;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IObjectRepository _objectStorageRepository;
        private readonly IConverterRepository _converterRepository;

        uint msgCount = 0;

        public QueueRepository(IConnection connection, ILog4NetRepository log4NetRepository, IObjectRepository objectStorageRepository, IConverterRepository converterRepository)
        {
            _connection = connection;
            _log4NetRepository = log4NetRepository;
            _objectStorageRepository = objectStorageRepository;
            _converterRepository = converterRepository;
        }

        public void ConsumeQueue(string queue, long messageTtl = 0)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    var queueResult = channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: messageTtl == 0 ? null : new Dictionary<string, object>()
                                         {
                                             {
                                                 "x-message-ttl", messageTtl
                                             }
                                         });

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += ConverterQueue_ReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    // Wait here until all messages are retrieved
                    msgsRecievedGate.Wait();

                }
            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = "BasicConsume",
                    Date = DateTime.Now,
                    QueueName = queue,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };
                //_queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);
            }
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey, long messageTtl = 0)
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
                                         arguments: messageTtl == 0 ? null : new Dictionary<string, object>()
                                         {
                                             {
                                                 "x-message-ttl", messageTtl
                                             }
                                         });

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
                //_queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

            }
        }

        public async void ConverterQueue_ReceivedEvent(object? se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            msgCount = e.Model.MessageCount("converter");
            QueueMessage queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);

            ObjectData objModel = await _objectStorageRepository.GetFileAsync("videos", queueMsg.fileGuid);
            if (objModel == null)
            {
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            var converterResult = _converterRepository.ConvertMP4_to_MP3_Async(objModel, queueMsg);

            if (converterResult.Wait(60000))
            {
                e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }

            QueueLog queueLog = new QueueLog()
            {
                OperationType = "BasicConsume",
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = "converter",
                RoutingKey = ea.RoutingKey
            };
            OtherLog otherLog = new OtherLog()
            {
                queueLog = queueLog
            };

            string logText = $"{JsonConvert.SerializeObject(otherLog)}";
            _log4NetRepository.Info(logText);

            if (msgCount == 0)
            {
                msgsRecievedGate.Set();

                return;
            }
        }
    }
}
