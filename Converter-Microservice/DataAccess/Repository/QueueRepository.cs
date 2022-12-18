using Configuration;
using DataAccess.Interfaces;
using Elasticsearch.Net;
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
        private List<QueueMessage> messageList = new List<QueueMessage>();
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);

        private readonly IConnection _connection;
        private readonly ILoggingRepository _loggingRepository;

        public QueueRepository(IConnection connection, ILoggingRepository loggingRepository)
        {
            _connection = connection;
            _loggingRepository = loggingRepository;
        }

        public List<QueueMessage> ConsumeQueue(string queue)
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

                    uint msgCount = queueResult.MessageCount;
                    uint counter = 0;

                    consumer.Received += (sender, ea) =>
                    {
                        counter++;

                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        QueueMessage queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);
                        messageList.Add(queueMsg);

                        //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        QueueLog queueLog = new QueueLog()
                        {
                            Date = DateTime.Now,
                            Message = message,
                            QueueName = queue
                        };
                        OtherLog otherLog = new OtherLog()
                        {
                            queueLog = queueLog
                        };

                        _loggingRepository.LogQueueOther(otherLog);

                        if (msgCount == counter)
                        {
                            msgsRecievedGate.Set();

                            return;
                        }

                    };

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

                _loggingRepository.LogQueueError(errorLog);

                return null;
            }
        }

        public void QueueMessageDirect(TMessage message, string queue, string exchange, string routingKey)
        {
            try
            {
                var channel = _connection.CreateModel();
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
                _loggingRepository.LogQueueOther(otherLog);

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

                _loggingRepository.LogQueueError(errorLog);

            }
        }
    }
}
