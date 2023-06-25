using Elasticsearch.Net;
using Initilization_Microservice.Common;
using Initilization_Microservice.Operation.Interfaces;
using Initilization_Microservice.Operation.Operations;
using Initilization_Microservice.Repository.Interfaces;
using Initilization_Microservice.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Nest;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var services = new ServiceCollection();

services.AddLogging(logging =>
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


//ELK
ConnectionSettings connection = new ConnectionSettings(new Uri(ProjectConstants.ElkHost)).
DefaultIndex(ProjectConstants.ElkDefaultIndexName).
ServerCertificateValidationCallback(CertificateValidations.AllowAll).
ThrowExceptions(ProjectConstants.ElkExceptions).
PrettyJson().
RequestTimeout(TimeSpan.FromSeconds(ProjectConstants.ElkRequestTimeout)).
BasicAuthentication(ProjectConstants.ElkUsername, ProjectConstants.ElkPassword);
ElasticClient elasticClient = new ElasticClient(connection);
services.AddSingleton<IElasticClient>(elasticClient);


//RABBITMQ
var connectionFactory = new ConnectionFactory
{
    HostName = ProjectConstants.RabbitmqHost,
    Port = Convert.ToInt32(ProjectConstants.RabbitmqPort),
    UserName = ProjectConstants.RabbitmqUsername,
    Password = ProjectConstants.RabbitmqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();
services.AddSingleton<IConnection>(rabbitConnection);


//Minio S3
MinioClient minioClient = new MinioClient()
                        .WithEndpoint(ProjectConstants.MinioHost)
                        .WithCredentials(ProjectConstants.MinioAccessKey, ProjectConstants.MinioSecretKey)
                        .WithSSL(ProjectConstants.MinioUseSsl);
services.AddSingleton<IMinioClient>(minioClient);


services.AddScoped<IElkRepository, ElkRepository>();
services.AddScoped<IS3StorageRepository, S3StorageRepository>();
services.AddScoped<IQueueRepository, QueueRepository>();
services.AddScoped<IQueueOperation, QueueOperation>();
services.AddScoped<IS3StorageOperation, S3StorageOperation>();
services.AddScoped<IElkOperation, ElkOperation>();

var builder = services.BuildServiceProvider();

var queueService = builder.GetService<IQueueOperation>();
var storageOperation = builder.GetService<IS3StorageOperation>();
var elkOperation = builder.GetService<IElkOperation>();

var storageServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.StorageServiceExchangeName, ProjectConstants.StorageServiceExchange_Type, ProjectConstants.StorageServiceExchangeIsDurable, ProjectConstants.StorageServiceExchangeIsAutoDelete, ProjectConstants.StorageServiceQueueName, ProjectConstants.StorageServiceQueueIsDurable, ProjectConstants.StorageServiceQueueIsExclusive, ProjectConstants.StorageServiceQueueIsAutoDelete, null);
var userServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.UserServiceExchangeName, ProjectConstants.UserServiceExchange_Type, ProjectConstants.UserServiceExchangeIsDurable, ProjectConstants.UserServiceExchangeIsAutoDelete, ProjectConstants.UserServiceQueueName, ProjectConstants.UserServiceQueueIsDurable, ProjectConstants.UserServiceQueueIsExclusive, ProjectConstants.UserServiceQueueIsAutoDelete, null);
var chatBrokerServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.ChatBrokerServiceExchangeName, ProjectConstants.ChatBrokerServiceExchange_Type, ProjectConstants.ChatBrokerServiceExchangeIsDurable, ProjectConstants.ChatBrokerServiceExchangeIsAutoDelete, ProjectConstants.ChatBrokerServiceQueueName, ProjectConstants.ChatBrokerServiceQueueIsDurable, ProjectConstants.ChatBrokerServiceQueueIsExclusive, ProjectConstants.ChatBrokerServiceQueueIsAutoDelete, null);


var storageConfiguration = storageOperation.ConfigureS3Async(ProjectConstants.MinioBucketName);


var userServiceElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.ElkUserServiceIndexName, ProjectConstants.UserServiceNumberOfShards, ProjectConstants.UserServiceNumberOfReplicas);
var storageServiceElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.ElkStorageServiceIndexName, ProjectConstants.StorageServiceNumberOfShards, ProjectConstants.StorageServiceNumberOfReplicas);
var chatBrokerServiceElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.ElkChatBrokerServiceIndexName, ProjectConstants.ChatBrokerServiceNumberOfShards, ProjectConstants.ChatBrokerServiceNumberOfReplicas);


await Task.WhenAll(
    storageServiceQueueConfiguration,
    userServiceQueueConfiguration,
    chatBrokerServiceQueueConfiguration,
    storageConfiguration,
    userServiceElkConfiguration,
    storageServiceElkConfiguration,
    chatBrokerServiceElkConfiguration
    );

