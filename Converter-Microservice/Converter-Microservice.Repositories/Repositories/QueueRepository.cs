using Converter_Microservice.Commands.ConverterCommands;
using Converter_Microservice.Common.Constants;
using Converter_Microservice.Common.Events;
using Converter_Microservice.Queries.ObjectQueries;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Converter_Microservice.Repositories.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private ManualResetEventSlim msgsRecievedGate = new ManualResetEventSlim(false);

        private readonly IConnection _connection;
        private readonly IObjectRepository _objectStorageRepository;
        private readonly IConverterRepository _converterRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<QueueRepository> _logger;

        uint msgCount = 0;

        public QueueRepository(IConnection connection, IObjectRepository objectStorageRepository, IConverterRepository converterRepository, IMediator mediator, ILogger<QueueRepository> logger)
        {
            _connection = connection;
            _objectStorageRepository = objectStorageRepository;
            _converterRepository = converterRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public void ConsumeQueue(string queue, long messageTtl = 0)
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    _logger.LogInformation(LogEvents.BasicConsumeEvent, LogEvents.BasicConsumeEventMessage);

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
                    OperationType = LogEvents.BasicConsumeEventMessage,
                    Date = DateTime.Now,
                    QueueName = queue,
                    ExceptionMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    queueLog = queueLog
                };

                _logger.LogError(LogEvents.BasicConsumeEvent, exception.Message.ToString());
            }
        }

        public void QueueMessageDirect(object message, string queue, string exchange, string routingKey, long messageTtl = 0)
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
                    OperationType = LogEvents.BasicPublishEventMessage,
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

                _logger.LogInformation(LogEvents.BasicPublishEvent, JsonConvert.SerializeObject(otherLog));

            }
            catch (Exception exception)
            {
                QueueLog queueLog = new QueueLog()
                {
                    OperationType = LogEvents.BasicPublishEventMessage,
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

                _logger.LogError(LogEvents.BasicPublishEvent, exception.Message.ToString());
            }
        }

        public async void ConverterQueue_ReceivedEvent(object? se, BasicDeliverEventArgs ea)
        {
            var e = (EventingBasicConsumer)se;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            msgCount = e.Model.MessageCount(ProjectConstants.ConverterServiceQueueName);
            QueueMessage queueMsg = JsonConvert.DeserializeObject<QueueMessage>(message);

            ObjectData objModel = await _mediator.Send(new ObjectQuery(ProjectConstants.MinioVideoBucket, queueMsg.fileGuid));
            if (objModel == null)
            {
                e.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            var converterResult = await _mediator.Send(new ConverterCommand(objModel, queueMsg));
            if (converterResult != null)
            {
                e.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }

            QueueLog queueLog = new QueueLog()
            {
                OperationType = LogEvents.BasicConsumeEventMessage,
                Date = DateTime.Now,
                ExchangeName = ea.Exchange,
                Message = JsonConvert.SerializeObject(message),
                QueueName = ProjectConstants.ConverterServiceQueueName,
                RoutingKey = ea.RoutingKey
            };
            OtherLog otherLog = new OtherLog()
            {
                queueLog = queueLog
            };

            _logger.LogInformation(LogEvents.BasicConsumeEvent, JsonConvert.SerializeObject(otherLog));

            if (msgCount == 0)
            {
                msgsRecievedGate.Set();

                return;
            }
        }
    }
}
