using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Notification_Microservice.Commands.ObjectCommands;
using Notification_Microservice.Commands.QueueCommands;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Handlers.ObjectHandlers;
using Notification_Microservice.Handlers.QueueHandlers;
using Notification_Microservice.Queries.ObjectQueries;
using Notification_Microservice.Queries.QueueQueries;
using Notification_Microservice.Repositories.Interfaces;
using Notification_Microservice.Repositories.Providers;
using Notification_Microservice.Repositories.Repositories;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

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


//Helpers
serviceProvider.AddScoped<IMailSenderRepository, MailSenderRepository>();


//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository), typeof(QueueRepository));
serviceProvider.AddScoped<IObjectRepository, ObjectRepository>();


serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(ObjectCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(ObjectCommandHandler).Assembly,
        typeof(ObjectQueryHandler).Assembly,
        typeof(QueueCommandHandler).Assembly,
        typeof(QueueQueryHandler).Assembly,
        typeof(ObjectQuery).Assembly,
        typeof(QueueQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();

var builder = serviceProvider.BuildServiceProvider();

var _mediator = builder.GetService<IMediator>();
await _mediator.Send(new QueueQuery(ProjectConstants.NotificationServiceQueueName, ProjectConstants.NotificationServiceExchangeTtl));
