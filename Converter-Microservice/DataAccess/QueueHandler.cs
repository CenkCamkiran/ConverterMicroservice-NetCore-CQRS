using Configuration;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public class QueueHandler
    {
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

        public async Task<QueueMessage> ConsumeQueue()
        {
            QueueMessage? queueMsg = null;
            try
            {
                IConnection rabbitConnection = ConnectRabbitMQ();

                using (var channel = rabbitConnection.CreateModel())
                {
                    channel.QueueDeclare(queue: "converter",
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

                    await Task.FromResult(queueMsg);
                    //Console.WriteLine(" Press [enter] to exit.");
                    //Console.ReadLine();

                }
            }
            catch (Exception exception)
            {
                ElkLogging logging = new ElkLogging();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);
            }

        }

        public async Task QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey)
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

            }
            catch (Exception exception)
            {
                ElkLogging logging = new ElkLogging();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);
            }
        }
    }
}
