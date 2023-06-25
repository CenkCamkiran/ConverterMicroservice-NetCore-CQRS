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
var converterOperation = builder.GetService<IS3StorageOperation>();
var elkOperation = builder.GetService<IElkOperation>();


var ConverterServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.ConverterServiceExchangeName, ProjectConstants.ConverterServiceExchange_Type, ProjectConstants.ConverterServiceExchangeIsDurable, ProjectConstants.ConverterServiceExchangeIsAutoDelete, ProjectConstants.ConverterServiceQueueName, ProjectConstants.ConverterServiceQueueIsDurable, ProjectConstants.ConverterServiceQueueIsExclusive, ProjectConstants.ConverterServiceQueueIsAutoDelete, ProjectConstants.ConverterServiceRoutingKey, ProjectConstants.ConverterServiceExchangeTtl, null);
var NotificationServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.NotificationServiceExchangeName, ProjectConstants.NotificationServiceExchange_Type, ProjectConstants.NotificationServiceExchangeIsDurable, ProjectConstants.NotificationServiceExchangeIsAutoDelete, ProjectConstants.NotificationServiceQueueName, ProjectConstants.NotificationServiceQueueIsDurable, ProjectConstants.NotificationServiceQueueIsExclusive, ProjectConstants.NotificationServiceQueueIsAutoDelete, ProjectConstants.NotificationServiceRoutingKey, ProjectConstants.NotificationServiceExchangeTtl, null);
var ErrorLoggerConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.LoggerServiceExchangeName, ProjectConstants.LoggerServiceExchange_Type, ProjectConstants.LoggerServiceExchangeIsDurable, ProjectConstants.LoggerServiceExchangeIsAutoDelete, ProjectConstants.ErrorLoggerServiceQueueName, ProjectConstants.LoggerServiceQueueIsDurable, ProjectConstants.LoggerServiceQueueIsExclusive, ProjectConstants.LoggerServiceQueueIsAutoDelete, ProjectConstants.ErrorLogsServiceRoutingKey, ProjectConstants.LoggerServiceExchangeTtl, null);
var OtherLoggerConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.LoggerServiceExchangeName, ProjectConstants.LoggerServiceExchange_Type, ProjectConstants.LoggerServiceExchangeIsDurable, ProjectConstants.LoggerServiceExchangeIsAutoDelete, ProjectConstants.OtherLoggerServiceQueueName, ProjectConstants.LoggerServiceQueueIsDurable, ProjectConstants.LoggerServiceQueueIsExclusive, ProjectConstants.LoggerServiceQueueIsAutoDelete, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.LoggerServiceExchangeTtl, null);

var ConverterAudioConfiguration = converterOperation.ConfigureS3Async(ProjectConstants.MinioAudioBucket);
var ConverterVideoConfiguration = converterOperation.ConfigureS3Async(ProjectConstants.MinioVideoBucket);

var ErrorLogsElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.LoggerServiceErrorLogsIndex, ProjectConstants.LoggerServiceErrorLogsNumberOfShards, ProjectConstants.LoggerServiceErrorLogsNumberOfReplicas);
var OtherLogsElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.LoggerServiceOtherLogsIndex, ProjectConstants.LoggerServiceOtherLogsNumberOfShards, ProjectConstants.LoggerServiceErrorLogsNumberOfReplicas);
var WebServiceObjectStorageLogsElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.WebServiceObjectStorageLogs, ProjectConstants.WebServiceObjectStorageNumberOfShards, ProjectConstants.LoggerServiceOtherLogsNumberOfReplicas);
var WebServiceRequestResponseLogsElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.WebServiceRequestResponseLogs, ProjectConstants.WebServiceRequestResponseNumberOfShards, ProjectConstants.WebServiceRequestResponseNumberOfReplicas);
var WebServiceQueueLogsElkConfiguration = elkOperation.ConfigureIndex(ProjectConstants.WebServiceQueueLogs, ProjectConstants.WebServiceQueueNumberOfShards, ProjectConstants.WebServiceQueueNumberOfReplicas);


await Task.WhenAll(
    ConverterServiceQueueConfiguration,
    NotificationServiceQueueConfiguration,
    ErrorLoggerConfiguration,
    OtherLoggerConfiguration,
    ConverterAudioConfiguration,
    ConverterVideoConfiguration,
    ErrorLogsElkConfiguration,
    OtherLogsElkConfiguration,
    WebServiceObjectStorageLogsElkConfiguration,
    WebServiceRequestResponseLogsElkConfiguration,
    WebServiceQueueLogsElkConfiguration
    );

