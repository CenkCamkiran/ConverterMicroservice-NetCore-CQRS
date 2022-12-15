using Configuration;
using log4net;
using Models;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;

namespace DataAccess
{
    public class QueueHandler
    {
        private Logger log = new Logger();

        public IConnection ConnectRabbitMQ()
        {
            EnvVariablesHandler envHandler = new EnvVariablesHandler();
            RabbitMqConfiguration rabbitMqConfiguration = envHandler.GetRabbitEnvVariables();

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMqConfiguration.RabbitMqHost,
                Port = Convert.ToInt32(rabbitMqConfiguration.RabbitMqPort),
                UserName = rabbitMqConfiguration.RabbitMqUsername,
                Password = rabbitMqConfiguration.RabbitMqPassword
            };

            IConnection rabbitConnection = connectionFactory.CreateConnection();

            return rabbitConnection;

        }

        public async Task<QueueMessage> ConsumeQueueAsync(string queue)
        {
            QueueMessage? queueMsg = null;
            try
            {
                IConnection rabbitConnection = ConnectRabbitMQ();

                using (var channel = rabbitConnection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };

                    channel.BasicConsume(queue: "converter",
                                         autoAck: false,
                                         consumer: consumer);

                    QueueLog queueLog = new QueueLog()
                    {
                        Date = DateTime.Now,
                        Message = queueMsg,
                        QueueName = queue
                    };
                    ElkLogging<QueueLog> elkLogging = new ElkLogging<QueueLog>();
                    await elkLogging.IndexExceptionAsync("converter_queue_logs", queueLog);

                    string logText = $"Queue: {queue} - Message: (fileGuid: {queueLog.Message?.fileGuid} && email: {queueLog.Message?.email})";
                    log.Info(logText);

                    return await Task.FromResult(queueMsg);
                 
                    //Console.WriteLine(" Press [enter] to exit.");
                    //Console.ReadLine();
                }
            }
            catch (Exception exception)
            {
                ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);

                return null;
            }

        }

        public async Task QueueMessageDirectAsync(QueueMessage message, string queue, string exchange, string routingKey)
        {
            try
            {
                IConnection rabbitConnection = ConnectRabbitMQ();
                var channel = rabbitConnection.CreateModel();
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
                    OperationType = nameof(channel.BasicPublish),
                    Date = DateTime.Now,
                    ExchangeName = exchange,
                    Message = message,
                    QueueName = queue,
                    RoutingKey = routingKey
                };
                ElkLogging<QueueLog> elkLogging = new ElkLogging<QueueLog>();
                await elkLogging.IndexExceptionAsync("converter_queue_logs", queueLog);

                string logText = $"Exchange: {exchange} - Queue: {queue} - Routing Key: {routingKey} - Message: (fileGuid: {message.fileGuid} && email: {message.email})";
                log.Info(logText);

            }
            catch (Exception exception)
            {
                ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);
            }
        }
    }
}
