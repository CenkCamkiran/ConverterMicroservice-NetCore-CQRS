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

internal class Program
{
    private IConverterOperation _converterOperation;
    private IObjectStorageOperation _objectStorageOperation;
    private IQueueOperation<object> _queueOperation;

    public Program(IConverterOperation converterOperation, IObjectStorageOperation objectStorageOperation, IQueueOperation<object> queueOperation)
    {
        _converterOperation = converterOperation;
        _objectStorageOperation = objectStorageOperation;
        _queueOperation = queueOperation;
    }

    private async Task Main(string[] args)
    {
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

        List<QueueMessage> messageList = _queueOperation.ConsumeQueue("converter");

        foreach (var message in messageList)
        {
            ObjectDataModel objModel = await _objectStorageOperation.GetFileAsync("videos", message.fileGuid);
            await _converterOperation.ConvertMP4_to_MP3(objModel, message);
        }

        Console.ReadLine();
    }
}

//Convert to MP3

/* ************************************************************************************************* */

//Consume queue (get guid and email from message)
//Convert mp4 to mp3 (get mp4 object from videos bucket using guid)
//Get stream data of mp3
//Put stream data of mp3 as object to minio (audio bucket)
//send message to notification queue (message consists of guid of mp3 file and email)
//Write notif microservice!

/* ************************************************************************************************* */
