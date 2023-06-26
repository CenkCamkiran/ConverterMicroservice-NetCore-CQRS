using Elasticsearch.Net;
using Initilization_Microservice.Common;
using Initilization_Microservice.Common.Events;
using Initilization_Microservice.Models;
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


services.AddScoped(typeof(IElkRepository<>), typeof(ElkRepository<>));
services.AddScoped<IS3StorageRepository, S3StorageRepository>();
services.AddScoped<IQueueRepository, QueueRepository>();
services.AddScoped<IQueueOperation, QueueOperation>();
services.AddScoped<IS3StorageOperation, S3StorageOperation>();
services.AddScoped(typeof(IElkOperation<>), typeof(ElkOperation<>));

var builder = services.BuildServiceProvider();

var queueService = builder.GetService<IQueueOperation>();
var converterOperation = builder.GetService<IS3StorageOperation>();
var errorLogElkOperation = builder.GetService<IElkOperation<ErrorLog>>();
var otherLogElkOperation = builder.GetService<IElkOperation<OtherLog>>();
var objectStorageLogElkOperation = builder.GetService<IElkOperation<ObjectStorageLog>>();
var requestResponseLogElkOperation = builder.GetService<IElkOperation<RequestResponseLog>>();
var queueLogElkOperation = builder.GetService<IElkOperation<QueueLog>>();
var logger = builder.GetService<ILogger<Program>>();


logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");

logger.LogInformation(LogEvents.ServiceConfigurationPhase, LogEvents.ServiceConfigurationPhaseMessage);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_HOST: " + ProjectConstants.RabbitmqHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_PORT: " + ProjectConstants.RabbitmqPort);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_USERNAME: " + ProjectConstants.RabbitmqUsername);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_PASSWORD: " + ProjectConstants.RabbitmqPassword);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_HOST: " + ProjectConstants.ElkHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_DEFAULT_INDEX_NAME: " + ProjectConstants.ElkUsername);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_USERNAME: " + ProjectConstants.ElkPassword);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_PASSWORD " + ProjectConstants.ElkDefaultIndexName);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_HOST: " + ProjectConstants.MinioHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_USE_SSL: " + ProjectConstants.MinioUseSsl);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_ACCESS_KEY: " + ProjectConstants.MinioAccessKey);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_SECRET_KEY " + ProjectConstants.MinioSecretKey);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_AUDIO_BUCKET_NAME " + ProjectConstants.MinioAudioBucket);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_VIDEO_BUCKET_NAME " + ProjectConstants.MinioVideoBucket);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");

var ConverterServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.ConverterServiceExchangeName, ProjectConstants.ConverterServiceExchange_Type, ProjectConstants.ConverterServiceExchangeIsDurable, ProjectConstants.ConverterServiceExchangeIsAutoDelete, ProjectConstants.ConverterServiceQueueName, ProjectConstants.ConverterServiceQueueIsDurable, ProjectConstants.ConverterServiceQueueIsExclusive, ProjectConstants.ConverterServiceQueueIsAutoDelete, ProjectConstants.ConverterServiceRoutingKey, ProjectConstants.ConverterServiceExchangeTtl, null);
var NotificationServiceQueueConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.NotificationServiceExchangeName, ProjectConstants.NotificationServiceExchange_Type, ProjectConstants.NotificationServiceExchangeIsDurable, ProjectConstants.NotificationServiceExchangeIsAutoDelete, ProjectConstants.NotificationServiceQueueName, ProjectConstants.NotificationServiceQueueIsDurable, ProjectConstants.NotificationServiceQueueIsExclusive, ProjectConstants.NotificationServiceQueueIsAutoDelete, ProjectConstants.NotificationServiceRoutingKey, ProjectConstants.NotificationServiceExchangeTtl, null);
var ErrorLoggerConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.LoggerServiceExchangeName, ProjectConstants.LoggerServiceExchange_Type, ProjectConstants.LoggerServiceExchangeIsDurable, ProjectConstants.LoggerServiceExchangeIsAutoDelete, ProjectConstants.ErrorLoggerServiceQueueName, ProjectConstants.LoggerServiceQueueIsDurable, ProjectConstants.LoggerServiceQueueIsExclusive, ProjectConstants.LoggerServiceQueueIsAutoDelete, ProjectConstants.ErrorLogsServiceRoutingKey, ProjectConstants.LoggerServiceExchangeTtl, null);
var OtherLoggerConfiguration = queueService.ConfigureExchangeAndQueueAsync(ProjectConstants.LoggerServiceExchangeName, ProjectConstants.LoggerServiceExchange_Type, ProjectConstants.LoggerServiceExchangeIsDurable, ProjectConstants.LoggerServiceExchangeIsAutoDelete, ProjectConstants.OtherLoggerServiceQueueName, ProjectConstants.LoggerServiceQueueIsDurable, ProjectConstants.LoggerServiceQueueIsExclusive, ProjectConstants.LoggerServiceQueueIsAutoDelete, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.LoggerServiceExchangeTtl, null);

var ConverterAudioConfiguration = converterOperation.ConfigureS3Async(ProjectConstants.MinioAudioBucket);
var ConverterVideoConfiguration = converterOperation.ConfigureS3Async(ProjectConstants.MinioVideoBucket);


var ErrorLogsElkConfiguration = errorLogElkOperation.ConfigureIndex(ProjectConstants.LoggerServiceErrorLogsIndex, ProjectConstants.LoggerServiceErrorLogsNumberOfShards, ProjectConstants.LoggerServiceErrorLogsNumberOfReplicas);
var OtherLogsElkConfiguration = otherLogElkOperation.ConfigureIndex(ProjectConstants.LoggerServiceOtherLogsIndex, ProjectConstants.LoggerServiceOtherLogsNumberOfShards, ProjectConstants.LoggerServiceErrorLogsNumberOfReplicas);
var WebServiceObjectStorageLogsElkConfiguration = objectStorageLogElkOperation.ConfigureIndex(ProjectConstants.WebServiceObjectStorageLogs, ProjectConstants.WebServiceObjectStorageNumberOfShards, ProjectConstants.LoggerServiceOtherLogsNumberOfReplicas);
var WebServiceRequestResponseLogsElkConfiguration = requestResponseLogElkOperation.ConfigureIndex(ProjectConstants.WebServiceRequestResponseLogs, ProjectConstants.WebServiceRequestResponseNumberOfShards, ProjectConstants.WebServiceRequestResponseNumberOfReplicas);
var WebServiceQueueLogsElkConfiguration = queueLogElkOperation.ConfigureIndex(ProjectConstants.WebServiceQueueLogs, ProjectConstants.WebServiceQueueNumberOfShards, ProjectConstants.WebServiceQueueNumberOfReplicas);
var WebServiceErrorLogsElkConfiguration = queueLogElkOperation.ConfigureIndex(ProjectConstants.WebServiceErrorLogs, ProjectConstants.WebServiceErrorLogsNumberOfShards, ProjectConstants.WebServiceErrorLogsNumberOfReplicas);


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
    WebServiceQueueLogsElkConfiguration,
    WebServiceErrorLogsElkConfiguration
    );

