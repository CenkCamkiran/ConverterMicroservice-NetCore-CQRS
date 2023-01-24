using Elasticsearch.Net;
using Interfaces;
using LoggerMicroservice.Configuration;
using LoggerMicroservice.Models;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Operations;
using Providers;
using RabbitMQ.Client;
using Repositories;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();

EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
ElkConfiguration elkEnvVariables = envVariablesHandler.GetElkEnvVariables();
RabbitMqConfiguration rabbitEnvVariables = envVariablesHandler.GetRabbitEnvVariables();

Console.WriteLine($"RABBITMQ_HOST {rabbitEnvVariables.RabbitMqHost}");
Console.WriteLine($"RABBITMQ_PORT {rabbitEnvVariables.RabbitMqPort}");
Console.WriteLine($"RABBITMQ_USERNAME {rabbitEnvVariables.RabbitMqUsername}");
Console.WriteLine($"RABBITMQ_PASSWORD {rabbitEnvVariables.RabbitMqPassword}");

Console.WriteLine($"ELK_HOST {elkEnvVariables.ElkHost}");
Console.WriteLine($"ELK_DEFAULT_INDEX {elkEnvVariables.ElkDefaultIndex}");
Console.WriteLine($"ELK_USERNAME {elkEnvVariables.ElkUsername}");
Console.WriteLine($"ELK_PASSWORD {elkEnvVariables.ElkPassword}");

ConnectionSettings connection = new ConnectionSettings(new Uri(elkEnvVariables.ElkHost)).
DefaultIndex(elkEnvVariables.ElkDefaultIndex).
ServerCertificateValidationCallback(CertificateValidations.AllowAll).
ThrowExceptions(true).
PrettyJson().
RequestTimeout(TimeSpan.FromSeconds(300)).
BasicAuthentication(elkEnvVariables.ElkUsername, elkEnvVariables.ElkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 
ElasticClient elasticClient = new ElasticClient(connection);
serviceProvider.AddSingleton<IElasticClient>(elasticClient);

var connectionFactory = new ConnectionFactory
{
    HostName = rabbitEnvVariables.RabbitMqHost,
    Port = Convert.ToInt32(rabbitEnvVariables.RabbitMqPort),
    UserName = rabbitEnvVariables.RabbitMqUsername,
    Password = rabbitEnvVariables.RabbitMqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();
serviceProvider.AddSingleton(rabbitConnection);

//Operation
serviceProvider.AddScoped(typeof(ILoggingOperation<>), typeof(LoggingOperation<>));
serviceProvider.AddScoped(typeof(IQueueOperation<>), typeof(QueueOperation<>));

//Repository
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
serviceProvider.AddScoped(typeof(ILoggingRepository<>), typeof(LoggingRepository<>));
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

Console.WriteLine("Program Started!");

var _queueErrorLogsOperation = builder.GetService<IQueueOperation<ErrorLog>>();
var _queueOtherLogsOperation = builder.GetService<IQueueOperation<OtherLog>>();
var _loggingOtherLogsOperation = builder.GetService<ILoggingOperation<OtherLog>>();
var _loggingErrorLogsOperation = builder.GetService<ILoggingOperation<ErrorLog>>();

try
{
    await Task.Run(() =>
    {
        _queueErrorLogsOperation.ConsumeErrorLogsQueue("errorlogs");
    });

    await Task.Run(() =>
    {
        _queueOtherLogsOperation.ConsumeOtherLogsQueue("otherlogs");
    });

}
catch (Exception exception)
{
    QueueLog queueLog = new QueueLog()
    {
        OperationType = "Program.cs",
        Date = DateTime.Now,
        ExceptionMessage = exception.Message.ToString()
    };
    ErrorLog errorLog = new ErrorLog()
    {
        queueLog = queueLog
    };
    _queueErrorLogsOperation.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

}
