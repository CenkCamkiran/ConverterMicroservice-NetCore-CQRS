using MediatR;
using Newtonsoft.Json;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Common.Events;
using Notification_Microservice.Queries.ObjectQueries;
using Notification_Microservice.Repositories.Interfaces;
using NotificationMicroservice.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Notification_Microservice.Repositories.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);
        uint msgCount = 0;

        private readonly IConnection _connection;
        private readonly Lazy<IQueueRepository> _queueErrorRepository;
        private readonly Lazy<IMailSenderRepository> _mailSenderHelper;
        private readonly IMediator _mediator;

        public QueueRepository(ManualResetEventSlim msgsRecievedGate, uint msgCount, IConnection connection, Lazy<IQueueRepository> queueErrorRepository, Lazy<IMailSenderRepository> mailSenderHelper, IMediator mediator)
        {
            this.msgsRecievedGate = msgsRecievedGate;
            this.msgCount = msgCount;
            _connection = connection;
            _queueErrorRepository = queueErrorRepository;
            _mailSenderHelper = mailSenderHelper;
            _mediator = mediator;
        }

        public void ConsumeQueue(string queue, long messageTtl = 0)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += QueueMsgReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    // Wait here until all messages are retrieved
                    msgsRecievedGate.Wait();

                    return;
                }
            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = LogEvents.BasicConsumeEvent,
                    Date = DateTime.Now,
                    QueueName = queue,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
            }
        }

        public void QueueMessageDirect(object message, string queue, string exchange, string routingKey)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    string serializedObj = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(serializedObj);

                    channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: properties,
                                         body: body);
                }

                QueueLog queueLog = new QueueLog()
                {
                    OperationType = LogEvents.BasicPublishEvent,
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

            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = LogEvents.BasicPublishEvent,
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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";

            }
        }

        public async void QueueMsgReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            msgCount = e.Model.MessageCount(ProjectConstants.NotificationServiceQueueName);

            QueueMessage queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);

            ObjectData objModel = await _mediator.Send(new ObjectQuery(ProjectConstants.MinioAudiosBucket, queueMsg.fileGuid));
            if (objModel == null)
            {
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            using (FileStream fs = File.OpenRead(objModel.Mp3FileFullPath))
            {
                _mailSenderHelper.Value.SendMailToUser(queueMsg.email, Path.GetFileName(objModel.Mp3FileFullPath), fs);
                e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }

            if (File.Exists(objModel.Mp3FileFullPath))
                File.Delete(objModel.Mp3FileFullPath);

            QueueLog queueLog = new QueueLog()
            {
                OperationType = LogEvents.BasicConsumeEvent,
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = ProjectConstants.ErrorLogsServiceQueueName,
                RoutingKey = ea.RoutingKey
            };
            OtherLog otherLog = new OtherLog()
            {
                queueLog = queueLog
            };

            string logText = $"{JsonConvert.SerializeObject(otherLog)}";

            if (msgCount == 0)
            {
                msgsRecievedGate.Set();

                return;
            }
        }

    }
}
