﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Notification_Microservice.Commands.ObjectCommands;
using Notification_Microservice.Commands.QueueCommands;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Common.Events;
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
                        .WithSSL(ProjectConstants.MinioUseSsl).Build();
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
serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();


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

ILogger<Program>? logger = builder.GetService<ILogger<Program>>();

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

logger.LogInformation(LogEvents.ServiceConfigurationEvent, LogEvents.ServiceConfigurationPhaseMessage);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_HOST: " + ProjectConstants.RabbitmqHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PORT: " + ProjectConstants.RabbitmqPort);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_USERNAME: " + ProjectConstants.RabbitmqUsername);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "RABBITMQ_PASSWORD: " + ProjectConstants.RabbitmqPassword);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "SMTP_HOST: " + ProjectConstants.SmtpHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "SMTP_PORT: " + ProjectConstants.SmtpPort);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "SMTP_MAIL_FROM: " + ProjectConstants.SmtpMailFrom);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "SMTP_MAIL_PASSWORD " + ProjectConstants.SmtpMailPassword);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "SMTP_MAIL_USERNAME " + ProjectConstants.SmtpMailUsername);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_HOST: " + ProjectConstants.MinioHost);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_USE_SSL: " + ProjectConstants.MinioUseSsl);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_ACCESS_KEY: " + ProjectConstants.MinioAccessKey);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_SECRET_KEY " + ProjectConstants.MinioSecretKey);
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "MINIO_AUDIO_BUCKET_NAME " + ProjectConstants.MinioAudioBucket);

logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationEvent, "********************************************************************************");

var _mediator = builder.GetService<IMediator>();
await _mediator.Send(new QueueQuery(ProjectConstants.NotificationServiceQueueName, ProjectConstants.NotificationServiceExchangeTtl));
