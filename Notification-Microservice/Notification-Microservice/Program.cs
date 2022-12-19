using Configuration;
using DataAccess.Interfaces;
using DataAccess.Repository;
using Elasticsearch.Net;
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

ElkConfiguration elkConfiguration = envVariablesHandler.GetElkEnvVariables();
ConnectionSettings connection = new ConnectionSettings(new Uri(elkConfiguration.ElkHost)).
DefaultIndex(elkConfiguration.ElkDefaultIndex).
ServerCertificateValidationCallback(CertificateValidations.AllowAll).
ThrowExceptions(true).
PrettyJson().
RequestTimeout(TimeSpan.FromSeconds(300)).
BasicAuthentication(elkConfiguration.ElkUsername, elkConfiguration.ElkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 
ElasticClient elasticClient = new ElasticClient(connection);
serviceProvider.AddSingleton<IElasticClient>(elasticClient);

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
serviceProvider.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

serviceProvider.BuildServiceProvider();

//var _queueOperation = builder.GetService<IQueueOperation<object>>();
//var _objectStorageOperation = builder.GetService<IObjectStorageOperation>();
//_queueOperation.ConsumeQueue("converter");