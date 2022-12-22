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
using DataAccess.Providers;
using IConnection = RabbitMQ.Client.IConnection;
using Newtonsoft.Json;

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

//Operation
serviceProvider.AddScoped(typeof(ILoggingOperation<>), typeof(LoggingOperation<>));
serviceProvider.AddScoped(typeof(IQueueOperation<>), typeof(QueueOperation<>));

//Repository
serviceProvider.AddScoped(typeof(IQueueRepository<>), typeof(QueueRepository<>));
serviceProvider.AddScoped(typeof(ILoggingRepository<>), typeof(LoggingRepository<>));
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

var _queueErrorLogsOperation = builder.GetService<IQueueOperation<ErrorLog>>();
var _queueOtherLogsOperation = builder.GetService<IQueueOperation<OtherLog>>();

try
{

    await Task.Run(() =>
    {
        List<ErrorLog> errorLogs = _queueErrorLogsOperation.ConsumeQueue("errorlogs");

    });

    await Task.Run(() =>
    {
        List<OtherLog> otherLogs = _queueOtherLogsOperation.ConsumeQueue("otherlogs");

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
