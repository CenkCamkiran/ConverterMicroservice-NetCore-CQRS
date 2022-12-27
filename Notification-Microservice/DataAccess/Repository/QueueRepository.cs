using DataAccess.Interfaces;
using Helper.Interfaces;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using IConnection = RabbitMQ.Client.IConnection;

namespace DataAccess.Repository
{
    public class QueueRepository<TMessage> : IQueueRepository<TMessage> where TMessage : class
    {
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);
        uint msgCount = 0;

        private readonly IConnection _connection;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly Lazy<IQueueRepository<ErrorLog>> _queueErrorRepository;
        private readonly Lazy<IQueueRepository<OtherLog>> _queueOtherRepository;
        private readonly Lazy<IMailSenderRepository> _mailSenderHelper;
        private readonly IObjectStorageRepository _objectStorageRepository;

        public QueueRepository(IConnection connection, ILog4NetRepository log4NetRepository, Lazy<IQueueRepository<ErrorLog>> queueErrorRepository, Lazy<IQueueRepository<OtherLog>> queueOtherRepository, Lazy<IMailSenderRepository> mailSenderHelper, IObjectStorageRepository objectStorageRepository)
        {
            _connection = connection;
            _log4NetRepository = log4NetRepository;
            _queueErrorRepository = queueErrorRepository;
            _queueOtherRepository = queueOtherRepository;
            _mailSenderHelper = mailSenderHelper;
            _objectStorageRepository = objectStorageRepository;
        }

        public void ConsumeQueue(string queue)
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

                    consumer.Received += QueueMsgReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    if (channel.MessageCount(queue) == 0)
                    {
                        msgsRecievedGate.Set();

                        return;
                    }

                    //throw new Exception();
                    // Wait here until all messages are retrieved
                    msgsRecievedGate.Wait();

                    return;
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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);
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

        public async void QueueMsgReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            msgCount = e.Model.MessageCount("notification");

            QueueMessage queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);

            ObjectDataModel objModel = await _objectStorageRepository.GetFileAsync("audios", queueMsg.fileGuid);
            if (objModel == null)
            {
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            await _mailSenderHelper.Value.SendMailToUser(queueMsg.email, objModel.FileFullPath);
            e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            if (File.Exists(objModel.FileFullPath))
                File.Delete(objModel.FileFullPath);

            QueueLog queueLog = new QueueLog()
            {
                OperationType = "BasicConsume",
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = "errorlogs",
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
