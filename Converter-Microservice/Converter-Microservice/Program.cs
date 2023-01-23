using ConverterMicroservice.DataAccessLayer.Interfaces;
using ConverterMicroservice.DataAccessLayer.Providers;
using ConverterMicroservice.DataAccessLayer.Repositories;
using ConverterMicroservice.Models;
using ConverterMicroservice.OperationLayer.Interfaces;
using ConverterMicroservice.OperationLayer.Operations;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using RabbitMQ.Client;

var serviceProvider = new ServiceCollection();

EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();

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

//Operations
serviceProvider.AddScoped<IObjectStorageOperation, ObjectStorageOperation>();
serviceProvider.AddScoped(typeof(IQueueOperation<>), typeof(QueueOperation<>));

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));

serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

Console.WriteLine("Program Started!");

var _queueOperation = builder.GetService<IQueueOperation<QueueMessage>>();
_queueOperation.ConsumeQueue("converter", 43200000);
