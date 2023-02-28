using Converter_Microservice.Commands.ConverterCommands;
using Converter_Microservice.Commands.ObjectCommands;
using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Handlers.ConverterHandlers;
using Converter_Microservice.Handlers.LogHandlers;
using Converter_Microservice.Handlers.ObjectHandlers;
using Converter_Microservice.Handlers.QueueHandlers;
using Converter_Microservice.ProjectConfigurations;
using Converter_Microservice.Queries.ObjectQueries;
using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using Converter_Microservice.Repositories.Providers;
using Converter_Microservice.Repositories.Repositories;
using ConverterMicroservice.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using RabbitMQ.Client;
using System.Reflection;

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

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
serviceProvider.AddScoped<IObjectRepository, ObjectRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(ConverterHandler).Assembly,
        typeof(ObjectCommandHandler).Assembly,
        typeof(ObjectQueryHandler).Assembly,
        typeof(QueueCommandHandler<>).Assembly,
        typeof(QueueQueryHandler<>).Assembly,
        typeof(LogHandler).Assembly,
        typeof(ObjectCommand).Assembly,
        typeof(QueueCommand<>).Assembly,
        typeof(ConverterCommand).Assembly,
        typeof(ObjectQuery).Assembly,
        typeof(QueueQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

var _mediator = builder.GetService<IMediator>();
_mediator.Send(new QueueQuery("converter", 43200000));
