using Elasticsearch.Net;
using Logger_Microservice.Commands.LogCommands;
using Logger_Microservice.Commands.QueueCommands;
using Logger_Microservice.Handlers.LogHandlers;
using Logger_Microservice.Handlers.QueueHandlers;
using Logger_Microservice.ProjectConfigurations;
using Logger_Microservice.Queries.QueueQueries;
using Logger_Microservice.Repositories.Interfaces;
using Logger_Microservice.Repositories.Providers;
using Logger_Microservice.Repositories.Repositories;
using LoggerMicroservice.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using RabbitMQ.Client;
using System.Reflection;
using IConnection = RabbitMQ.Client.IConnection;

var serviceProvider = new ServiceCollection();

EnvVariablesConfiguration envVariablesHandler = new EnvVariablesConfiguration();
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

//Repository
serviceProvider.AddScoped(typeof(IQueueRepository), typeof(QueueRepository));
serviceProvider.AddScoped(typeof(ILogRepository), typeof(LogRepository));
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();

var Handlers = AppDomain.CurrentDomain.Load("Logger-Microservice.Handlers");
var Queries = AppDomain.CurrentDomain.Load("Logger-Microservice.Queries");
var Commands = AppDomain.CurrentDomain.Load("Logger-Microservice.Commands");

serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(LogCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(LogHandler).Assembly,
        typeof(QueueErrorQueryHandler).Assembly,
        typeof(QueueCommandHandler).Assembly,
        typeof(QueueErrorQueryHandler).Assembly,
        typeof(QueueErrorQuery).Assembly,
        typeof(QueueOtherQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

IMediator _mediator = builder.GetService<IMediator>();

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

try
{
    await Task.Run(async () =>
    {
        await _mediator.Send(new QueueErrorQuery("errorlogs"), ct);
    });

    await Task.Run(async () =>
    {
        await _mediator.Send(new QueueOtherQuery("otherlogs"), ct);
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
    await _mediator.Send(new QueueCommand(errorLog, "errorlogs", "log_exchange.direct", "error_log"));

}
