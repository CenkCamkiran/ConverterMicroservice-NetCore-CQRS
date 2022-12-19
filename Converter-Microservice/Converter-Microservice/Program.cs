using Configuration;
using DataAccess.Interfaces;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Models;
using Operation.Interfaces;
using Operation.Operations;
using RabbitMQ.Client;

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
serviceProvider.AddScoped(typeof(IQueueOperation<>), typeof(QueueOperation<>));
//serviceProvider.AddScoped<IQueueOperation<OtherLog>, QueueOperation<OtherLog>>();
//serviceProvider.AddScoped<IQueueOperation<ErrorLog>, QueueOperation<ErrorLog>>();
//serviceProvider.AddScoped<IQueueOperation<object>, QueueOperation<object>>();

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
//serviceProvider.AddScoped<IQueueRepository<object>, QueueRepository<object>>();
//serviceProvider.AddScoped<IQueueRepository<OtherLog>, QueueRepository<OtherLog>>();
//serviceProvider.AddScoped<IQueueRepository<ErrorLog>, QueueRepository<ErrorLog>>();
//serviceProvider.AddScoped<IQueueRepository<QueueMessage>, QueueRepository<QueueMessage>>();

serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

var builder = serviceProvider.BuildServiceProvider();

var _queueOperation = builder.GetService<IQueueRepository<object>>();
var _objectStorageOperation = builder.GetService<IObjectStorageOperation>();
await _queueOperation.ConsumeQueueAsync("converter");


//Console.ReadLine();

/* ************************************************************************************************* */

//Consume queue (get guid and email from message)
//Convert mp4 to mp3 (get mp4 object from videos bucket using guid)
//Get stream data of mp3
//Put stream data of mp3 as object to minio (audio bucket)
//send message to notification queue (message consists of guid of mp3 file and email)
//Write notif microservice!

/* ************************************************************************************************* */
