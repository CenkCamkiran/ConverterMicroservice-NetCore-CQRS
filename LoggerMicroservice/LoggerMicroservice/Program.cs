using Configuration;
using DataAccess.Interfaces;
using DataAccess.Repository;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Nest;
using Operation.Interfaces;
using Operation.Operations;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();
EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();

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


serviceProvider.AddScoped<ILoggingOperation<OtherLog>, LoggingOperation<OtherLog>>();
serviceProvider.AddScoped<ILoggingOperation<ErrorLog>, LoggingOperation<ErrorLog>>();
serviceProvider.AddScoped<IQueueOperation, QueueOperation>();

serviceProvider.AddScoped<IQueueRepository<object>, QueueRepository<object>>();
serviceProvider.AddScoped<ILoggingRepository<OtherLog>, LoggingRepository<OtherLog>>();
serviceProvider.AddScoped<ILoggingRepository<ErrorLog>, LoggingRepository<ErrorLog>>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

