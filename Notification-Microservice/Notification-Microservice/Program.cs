using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Notification_Microservice.ProjectConfigurations;
using Notification_Microservice.Repositories.Interfaces;
using Notification_Microservice.Repositories.Repositories;
using NotificationMicroservice.Models;
using RabbitMQ.Client;
using System.Reflection;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();

EnvVariablesConfiguration envVariablesHandler = new EnvVariablesConfiguration();

MinioConfiguration minioConfiguration = envVariablesHandler.GetMinioEnvVariables();
Console.WriteLine($"MINIO_HOST {minioConfiguration.MinioHost}");
Console.WriteLine($"MINIO_ACCESSKEY {minioConfiguration.MinioAccessKey}");
Console.WriteLine($"MINIO_SECRETKEY {minioConfiguration.MinioSecretKey}");

MinioClient minioClient = new MinioClient()
                        .WithEndpoint(minioConfiguration.MinioHost)
                        .WithCredentials(minioConfiguration.MinioAccessKey, minioConfiguration.MinioSecretKey)
                        .WithSSL(false);
serviceProvider.AddSingleton<IMinioClient>(minioClient);

RabbitMqConfiguration rabbitMqConfiguration = envVariablesHandler.GetRabbitEnvVariables();
Console.WriteLine($"RABBITMQ_HOST {rabbitMqConfiguration.RabbitMqHost}");
Console.WriteLine($"RABBITMQ_PORT {rabbitMqConfiguration.RabbitMqPort}");
Console.WriteLine($"RABBITMQ_USERNAME {rabbitMqConfiguration.RabbitMqUsername}");
Console.WriteLine($"RABBITMQ_PASSWORD {rabbitMqConfiguration.RabbitMqPassword}");

var connectionFactory = new ConnectionFactory
{
    HostName = rabbitMqConfiguration.RabbitMqHost,
    Port = Convert.ToInt32(rabbitMqConfiguration.RabbitMqPort),
    UserName = rabbitMqConfiguration.RabbitMqUsername,
    Password = rabbitMqConfiguration.RabbitMqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();
serviceProvider.AddSingleton(rabbitConnection);

//Helpers
serviceProvider.AddScoped<IMailSenderRepository, MailSenderRepository>();

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
serviceProvider.AddScoped<IObjectRepository, ObjectRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

Assembly.GetAssembly(typeof(ConverterHandler));
Assembly.GetAssembly(typeof(ObjectCommandHandler));
Assembly.GetAssembly(typeof(ObjectQueryHandler));
Assembly.GetAssembly(typeof(QueueCommandHandler<>));
Assembly.GetAssembly(typeof(QueueQueryHandler<>));
Assembly.GetAssembly(typeof(LogHandler));

var Handlers = AppDomain.CurrentDomain.Load("Notification-Microservice.Handlers");
var Queries = AppDomain.CurrentDomain.Load("Notification-Microservice.Queries");
var Commands = AppDomain.CurrentDomain.Load("Notification-Microservice.Commands");

serviceProvider.AddMediatR(Handlers);
serviceProvider.AddMediatR(Queries);
serviceProvider.AddMediatR(Commands);

serviceProvider.AddLazyResolution();

var builder = serviceProvider.BuildServiceProvider();

var _mediator = builder.GetService<IMediator>();

_mediator.Send(new QueueQuery("notification", 3600000)).GetAwaiter();
