﻿using Elasticsearch.Net;
using Logger_Microservice.Commands.LogCommands;
using Logger_Microservice.Commands.QueueCommands;
using Logger_Microservice.Common.Constants;
using Logger_Microservice.Common.Events;
using Logger_Microservice.Handlers.LogHandlers;
using Logger_Microservice.Handlers.QueueHandlers;
using Logger_Microservice.Queries.QueueQueries;
using Logger_Microservice.Repositories.Interfaces;
using Logger_Microservice.Repositories.Providers;
using Logger_Microservice.Repositories.Repositories;
using LoggerMicroservice.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();

serviceProvider.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    logging.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                            | ActivityTrackingOptions.TraceId
                                            | ActivityTrackingOptions.ParentId
                                            | ActivityTrackingOptions.Baggage
                                            | ActivityTrackingOptions.Tags;
    });
});


//ELK
ConnectionSettings connection = new ConnectionSettings(new Uri(ProjectConstants.ElkHost)).
DefaultIndex(ProjectConstants.ElkDefaultIndexName).
ServerCertificateValidationCallback(CertificateValidations.AllowAll).
ThrowExceptions(ProjectConstants.ElkExceptions).
PrettyJson().
RequestTimeout(TimeSpan.FromSeconds(ProjectConstants.ElkRequestTimeout)).
BasicAuthentication(ProjectConstants.ElkUsername, ProjectConstants.ElkPassword);
ElasticClient elasticClient = new ElasticClient(connection);
serviceProvider.AddSingleton<IElasticClient>(elasticClient);


//RabbitMQ
var connectionFactory = new ConnectionFactory
{
    HostName = ProjectConstants.RabbitmqHost,
    Port = Convert.ToInt32(ProjectConstants.RabbitmqPort),
    UserName = ProjectConstants.RabbitmqUsername,
    Password = ProjectConstants.RabbitmqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();
serviceProvider.AddSingleton(rabbitConnection);


//Repository
serviceProvider.AddScoped(typeof(IQueueRepository), typeof(QueueRepository));
serviceProvider.AddScoped(typeof(ILogRepository), typeof(LogRepository));


serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(LogCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(LogHandler).Assembly,
        typeof(QueueErrorQueryHandler).Assembly,
        typeof(QueueCommandHandler).Assembly,
        typeof(QueueErrorQueryHandler).Assembly,
        typeof(QueueErrorQuery).Assembly,
        typeof(QueueOtherQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

IMediator _mediator = builder.GetService<IMediator>();
ILogger<Program>? logger = builder.GetService<ILogger<Program>>();

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

logger.LogInformation(LogEvents.ServiceConfigurationEvent, LogEvents.ServiceConfigurationPhaseMessage);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_HOST: " + ProjectConstants.RabbitmqHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PORT: " + ProjectConstants.RabbitmqPort);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_USERNAME: " + ProjectConstants.RabbitmqUsername);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PASSWORD: " + ProjectConstants.RabbitmqPassword);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "ELK_HOST: " + ProjectConstants.ElkHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "ELK_DEFAULT_INDEX_NAME: " + ProjectConstants.ElkUsername);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "ELK_USERNAME: " + ProjectConstants.ElkPassword);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "ELK_PASSWORD " + ProjectConstants.ElkDefaultIndexName);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

try
{
    await Task.Run(async () =>
    {
        await _mediator.Send(new QueueErrorQuery(ProjectConstants.ErrorLogsServiceQueueName), ct);
    });

    await Task.Run(async () =>
    {
        await _mediator.Send(new QueueOtherQuery(ProjectConstants.OtherLogsServiceQueueName), ct);
    });
}
catch (Exception exception)
{
    QueueLog queueLog = new QueueLog()
    {
        OperationType = LogEvents.BasicConsumeEventMessage,
        Date = DateTime.Now,
        ExceptionMessage = exception.Message.ToString()
    };
    ErrorLog errorLog = new ErrorLog()
    {
        queueLog = queueLog
    };
    await _mediator.Send(new QueueCommand(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey));

    logger.LogError(LogEvents.BasicConsumeEventMessage, exception.Message.ToString());

}
