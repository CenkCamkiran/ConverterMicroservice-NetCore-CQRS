using Logger_Microservice.Commands.LogCommands;
using Logger_Microservice.Common.Events;
using Logger_Microservice.Repositories.Interfaces;
using LoggerMicroservice.Models;
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using WebService.Common.Constants;

namespace Logger_Microservice.Repositories.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private ManualResetEventSlim errorLogsMsgsRecievedGate = new ManualResetEventSlim(false);
        private ManualResetEventSlim otherLogsMsgsRecievedGate = new ManualResetEventSlim(false);
        uint msgCount = 0;

        private readonly IConnection _connection;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IMediator _mediator;

        public QueueRepository(ManualResetEventSlim errorLogsMsgsRecievedGate, ManualResetEventSlim otherLogsMsgsRecievedGate, uint msgCount, IConnection connection, ILog4NetRepository log4NetRepository, IMediator mediator)
        {
            this.errorLogsMsgsRecievedGate = errorLogsMsgsRecievedGate;
            this.otherLogsMsgsRecievedGate = otherLogsMsgsRecievedGate;
            this.msgCount = msgCount;
            _connection = connection;
            _log4NetRepository = log4NetRepository;
            _mediator = mediator;
        }

        public void ConsumeOtherLogsQueue(string queue)
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += OtherLogsQueueReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    if (channel.MessageCount(queue) == 0)
                    {
                        errorLogsMsgsRecievedGate.Set();

                        return;
                    }

                    // Wait here until all messages are retrieved
                    errorLogsMsgsRecievedGate.Wait();

                    return;
                }

            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = LogEvents.BasicConsumeEvent,
                    Date = DateTime.Now,
                    Message = JsonConvert.SerializeObject(""),
                    QueueName = queue,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };
                Task.Run(async () =>
                {
                    await _mediator.Send(new LogCommand(errorLog, ProjectConstants.LoggerServiceErrorLogsIndex));
                });

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);
            }
        }

        public void ConsumeErrorLogsQueue(string queue)
        {
            try
            {

                using (var channel = _connection.CreateModel())
                {
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += ErrorLogsQueueReceivedEvent;

                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);

                    if (channel.MessageCount(queue) == 0)
                    {
                        otherLogsMsgsRecievedGate.Set();

                        return;
                    }

                    // Wait here until all messages are retrieved
                    otherLogsMsgsRecievedGate.Wait();

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
                Task.Run(async () =>
                {
                    await _mediator.Send(new LogCommand(errorLog, ProjectConstants.LoggerServiceErrorLogsIndex));
                });

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);
            }
        }

        public async void QueueMessageDirect(object message, string queue, string exchange, string routingKey)
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
                _log4NetRepository.Info(logText);

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
                await _mediator.Send(new LogCommand(errorLog, ProjectConstants.LoggerServiceErrorLogsIndex));

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

            }
        }

        public void ErrorLogsQueueReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            msgCount = e.Model.MessageCount(ProjectConstants.ErrorLogsServiceQueueName);

            ErrorLog queueMsg = JsonConvert.DeserializeObject<ErrorLog>(message);

            var task = _mediator.Send(new LogCommand(queueMsg, ProjectConstants.LoggerServiceErrorLogsIndex));
            if (task.Result)
                e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            else
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);

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
            _log4NetRepository.Info(logText);

            if (msgCount == 0)
            {
                errorLogsMsgsRecievedGate.Set();

                return;
            }
        }

        public void OtherLogsQueueReceivedEvent(object se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            msgCount = e.Model.MessageCount(ProjectConstants.OtherLogsServiceQueueName);

            OtherLog queueMsg = JsonConvert.DeserializeObject<OtherLog>(message);

            var task = _mediator.Send(new LogCommand(queueMsg, ProjectConstants.LoggerServiceOtherLogsIndex));
            if (task.Result)
                e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            else
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);

            QueueLog queueLog = new QueueLog()
            {
                OperationType = LogEvents.BasicConsumeEvent,
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = ProjectConstants.OtherLogsServiceQueueName,
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
                otherLogsMsgsRecievedGate.Set();

                return;
            }
        }
    }
}
