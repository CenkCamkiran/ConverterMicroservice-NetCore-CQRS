using Converter_Microservice.Handlers.ConverterHandlers;
using Converter_Microservice.Handlers.LogHandlers;
using Converter_Microservice.Handlers.ObjectHandlers;
using Converter_Microservice.Handlers.QueueHandlers;
using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using Converter_Microservice.Repositories.Repositories;
using ConverterMicroservice.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using RabbitMQ.Client;
using System.Reflection;
using WebService.ProjectConfigurations;

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

Assembly.GetAssembly(typeof(ConverterHandler));
Assembly.GetAssembly(typeof(ObjectCommandHandler));
Assembly.GetAssembly(typeof(ObjectQueryHandler));
Assembly.GetAssembly(typeof(QueueCommandHandler<>));
Assembly.GetAssembly(typeof(QueueQueryHandler<>));
Assembly.GetAssembly(typeof(LogHandler));

var Handlers = AppDomain.CurrentDomain.Load("Converter-Microservice.Handlers");
var Queries = AppDomain.CurrentDomain.Load("Converter-Microservice.Queries");
var Commands = AppDomain.CurrentDomain.Load("Converter-Microservice.Commands");

serviceProvider.AddMediatR(Handlers);
serviceProvider.AddMediatR(Queries);
serviceProvider.AddMediatR(Commands);

Assembly.GetAssembly(typeof(LogHandler));

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

IMediator _mediator = builder.GetService<IMediator>();
_mediator.Send(new QueueQuery("converter", 43200000));
