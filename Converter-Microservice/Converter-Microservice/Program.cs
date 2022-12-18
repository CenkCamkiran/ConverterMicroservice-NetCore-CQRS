using Configuration;
using DataAccess.Interfaces;
using DataAccess.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Models;
using Nest;
using Operation.Interfaces;
using Operation.Operations;
using RabbitMQ.Client;
using System.Threading.Channels;

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
serviceProvider.AddScoped<IQueueOperation<OtherLog>, QueueOperation<OtherLog>>();
serviceProvider.AddScoped<IQueueOperation<ErrorLog>, QueueOperation<ErrorLog>>();
serviceProvider.AddScoped<IQueueOperation<object>, QueueOperation<object>>();
serviceProvider.AddScoped<ILoggingOperation, LoggingOperation>();
serviceProvider.AddScoped<ILoggingOperation, LoggingOperation>();
serviceProvider.AddScoped<IConverterOperation, ConverterOperation>();

//Repositories
serviceProvider.AddScoped<IQueueRepository<object>, QueueRepository<object>>();
serviceProvider.AddScoped<IQueueRepository<OtherLog>, QueueRepository<OtherLog>>();
serviceProvider.AddScoped<IQueueRepository<ErrorLog>, QueueRepository<ErrorLog>>();
serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

serviceProvider.BuildServiceProvider();

Initialize initialize = new Initialize();
List<QueueMessage> messageList = await initialize._queueOperation.ConsumeQueueAsync("converter");

foreach (var message in messageList)
{
    ObjectDataModel objModel = await initialize._objectStorageOperation.GetFileAsync("videos", message.fileGuid);
    await initialize._converterOperation.ConvertMP4_to_MP3(objModel, message);
}

Console.ReadLine();

class Initialize
{
    public IQueueOperation<object> _queueOperation;
    public IObjectStorageOperation _objectStorageOperation;
    public IConverterOperation _converterOperation;

    public Initialize()
    {

    }

    public Initialize(IQueueOperation<object> queueOperation, IObjectStorageOperation objectStorageOperation, IConverterOperation converterOperation)
    {
        _queueOperation = queueOperation;
        _objectStorageOperation = objectStorageOperation;
        _converterOperation = converterOperation;
    }
}


/* ************************************************************************************************* */

//Consume queue (get guid and email from message)
//Convert mp4 to mp3 (get mp4 object from videos bucket using guid)
//Get stream data of mp3
//Put stream data of mp3 as object to minio (audio bucket)
//send message to notification queue (message consists of guid of mp3 file and email)
//Write notif microservice!

/* ************************************************************************************************* */
