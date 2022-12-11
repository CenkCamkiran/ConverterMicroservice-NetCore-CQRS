using Configuration;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

        public async Task ConsumeQueue()
        {
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

                        QueueMessage? queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);



                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };
                    channel.BasicConsume(queue: "converter",
                                         autoAck: false,
                                         consumer: consumer);

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
    }
}
