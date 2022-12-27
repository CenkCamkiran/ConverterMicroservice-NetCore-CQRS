using Configuration;
using DataAccess.Interfaces;
using DataAccess.Providers;
using DataAccess.Repository;
using Elasticsearch.Net;
using Helper.Helpers;
using Helper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Models;
using Nest;
using Operation.Interfaces;
using Operation.Operations;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();

EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
MinioConfiguration minioConfiguration = envVariablesHandler.GetMinioEnvVariables();
MinioClient minioClient = new MinioClient()
                        .WithEndpoint(minioConfiguration.MinioHost)
                        .WithCredentials(minioConfiguration.MinioAccessKey, minioConfiguration.MinioSecretKey)
                        .WithSSL(false);
serviceProvider.AddSingleton<IMinioClient>(minioClient);

RabbitMqConfiguration rabbitMqConfiguration = envVariablesHandler.GetRabbitEnvVariables();
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
serviceProvider.AddScoped<IMailSenderOperation, MailSenderOperation>();
serviceProvider.AddScoped(typeof(IQueueOperation<>), typeof(QueueOperation<>));

//Helpers
serviceProvider.AddScoped<IMailSenderRepository, MailSenderRepository>();

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

serviceProvider.AddLazyResolution();

var builder = serviceProvider.BuildServiceProvider();
var _queueOperation = builder.GetService<IQueueOperation<QueueMessage>>();

_queueOperation.ConsumeQueue("notification", 3600000);

//var _queueOperation = builder.GetService<IQueueOperation<object>>();
//var _objectStorageOperation = builder.GetService<IObjectStorageOperation>();
//_queueOperation.ConsumeQueue("converter");