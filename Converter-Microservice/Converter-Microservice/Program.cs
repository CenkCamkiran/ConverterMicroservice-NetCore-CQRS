using Converter_Microservice.Commands.ConverterCommands;
using Converter_Microservice.Commands.ObjectCommands;
using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Common.Constants;
using Converter_Microservice.Common.Events;
using Converter_Microservice.Handlers.ConverterHandlers;
using Converter_Microservice.Handlers.LogHandlers;
using Converter_Microservice.Handlers.ObjectHandlers;
using Converter_Microservice.Handlers.QueueHandlers;
using Converter_Microservice.Queries.ObjectQueries;
using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using Converter_Microservice.Repositories.Providers;
using Converter_Microservice.Repositories.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using RabbitMQ.Client;

var serviceProvider = new ServiceCollection();

serviceProvider.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
    logging.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                            | ActivityTrackingOptions.TraceId
                                            | ActivityTrackingOptions.ParentId
                                            | ActivityTrackingOptions.Baggage
                                            | ActivityTrackingOptions.Tags;
    });
});


MinioClient minioClient = new MinioClient()
                        .WithEndpoint(ProjectConstants.MinioHost)
                        .WithCredentials(ProjectConstants.MinioAccessKey, ProjectConstants.MinioSecretKey)
                        .WithSSL(ProjectConstants.MinioUseSsl);
serviceProvider.AddSingleton<IMinioClient>(minioClient);


var connectionFactory = new ConnectionFactory
{
    HostName = ProjectConstants.RabbitmqHost,
    Port = Convert.ToInt32(ProjectConstants.RabbitmqPort),
    UserName = ProjectConstants.RabbitmqUsername,
    Password = ProjectConstants.RabbitmqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();

serviceProvider.AddSingleton(rabbitConnection);

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository), typeof(QueueRepository));
serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(ConverterHandler).Assembly,
        typeof(ObjectCommandHandler).Assembly,
        typeof(ObjectQueryHandler).Assembly,
        typeof(QueueCommandHandler).Assembly,
        typeof(QueueQueryHandler).Assembly,
        typeof(LogHandler).Assembly,
        typeof(ObjectCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(ConverterCommand).Assembly,
        typeof(ObjectQuery).Assembly,
        typeof(QueueQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

ILogger<Program>? logger = builder.GetService<ILogger<Program>>();

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

logger.LogInformation(LogEvents.ServiceConfigurationEvent, LogEvents.ServiceConfigurationPhaseMessage);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_HOST: " + ProjectConstants.RabbitmqHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PORT: " + ProjectConstants.RabbitmqPort);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_USERNAME: " + ProjectConstants.RabbitmqUsername);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PASSWORD: " + ProjectConstants.RabbitmqPassword);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_HOST: " + ProjectConstants.MinioHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_USE_SSL: " + ProjectConstants.MinioUseSsl);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_ACCESS_KEY: " + ProjectConstants.MinioAccessKey);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_SECRET_KEY " + ProjectConstants.MinioSecretKey);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_VIDEO_BUCKET_NAME " + ProjectConstants.MinioVideoBucket);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

var _mediator = builder.GetService<IMediator>();
await _mediator.Send(new QueueQuery(ProjectConstants.ConverterServiceQueueName, ProjectConstants.ConverterExchangeTtl));
