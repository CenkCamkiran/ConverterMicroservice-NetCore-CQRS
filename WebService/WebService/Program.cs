using Elasticsearch.Net;
using Minio;
using Nest;
using RabbitMQ.Client;
using WebService.Commands.LogCommands;
using WebService.Commands.ObjectCommands;
using WebService.Commands.QueueCommands;
using WebService.Handlers.HealthHandlers;
using WebService.Handlers.LogHandlers;
using WebService.Handlers.ObjectHandlers;
using WebService.Handlers.QueueHandlers;
using WebService.Helpers.Helpers;
using WebService.Helpers.Interfaces;
using WebService.Middlewares;
using WebService.Middlewares.Contexts;
using WebService.Middlewares.Contexts.Interfaces;
using WebService.Models;
using WebService.ProjectConfigurations;
using WebService.Queries.HealthQueries;
using WebService.Repositories.Interfaces;
using WebService.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

EnvVariablesConfiguration variablesConfiguration = new EnvVariablesConfiguration();
ElkConfiguration elkEnvVariables = variablesConfiguration.GetElkEnvVariables();
RabbitMqConfiguration rabbitEnvVariables = variablesConfiguration.GetRabbitEnvVariables();
MinioConfiguration minioEnvVariables = variablesConfiguration.GetMinioEnvVariables();

Console.WriteLine($"RABBITMQ_HOST {rabbitEnvVariables.RabbitMqHost}");
Console.WriteLine($"RABBITMQ_PORT {rabbitEnvVariables.RabbitMqPort}");
Console.WriteLine($"RABBITMQ_USERNAME {rabbitEnvVariables.RabbitMqUsername}");
Console.WriteLine($"RABBITMQ_PASSWORD {rabbitEnvVariables.RabbitMqPassword}");

Console.WriteLine($"MINIO_HOST {minioEnvVariables.MinioHost}");
Console.WriteLine($"MINIO_ACCESSKEY {minioEnvVariables.MinioAccessKey}");
Console.WriteLine($"MINIO_SECRETKEY {minioEnvVariables.MinioSecretKey}");

Console.WriteLine($"ELK_HOST {elkEnvVariables.ElkHost}");
Console.WriteLine($"ELK_DEFAULT_INDEX {elkEnvVariables.ElkDefaultIndex}");
Console.WriteLine($"ELK_USERNAME {elkEnvVariables.ElkUsername}");
Console.WriteLine($"ELK_PASSWORD {elkEnvVariables.ElkPassword}");

//Repository
builder.Services.AddScoped(typeof(ILogRepository<>), typeof(LogRepository<>));
builder.Services.AddScoped<ILog4NetRepository, Log4NetRepository>();
builder.Services.AddScoped<IObjectRepository, ObjectRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<ILogOtherRepository, LogOtherRepository>();


builder.Services.AddScoped<IPingHelper, PingHelper>();

//Context
builder.Services.AddScoped<IWebServiceContext, WebServiceContext>();

ConnectionSettings? connection = new ConnectionSettings(new Uri(elkEnvVariables.ElkHost)).
   DefaultIndex(elkEnvVariables.ElkDefaultIndex).
   ServerCertificateValidationCallback(CertificateValidations.AllowAll).
   ThrowExceptions(true).
   PrettyJson().
   RequestTimeout(TimeSpan.FromSeconds(300)).
   BasicAuthentication(elkEnvVariables.ElkUsername, elkEnvVariables.ElkPassword);

ElasticClient? elasticClient = new ElasticClient(connection);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

var connectionFactory = new ConnectionFactory
{
    HostName = rabbitEnvVariables.RabbitMqHost,
    Port = Convert.ToInt32(rabbitEnvVariables.RabbitMqPort),
    UserName = rabbitEnvVariables.RabbitMqUsername,
    Password = rabbitEnvVariables.RabbitMqPassword
};
var rabbitConnection = connectionFactory.CreateConnection();
builder.Services.AddSingleton<RabbitMQ.Client.IConnection>(rabbitConnection);

MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(minioEnvVariables.MinioHost)
                                    .WithCredentials(minioEnvVariables.MinioAccessKey, minioEnvVariables.MinioSecretKey)
                                    .WithSSL(false);
builder.Services.AddSingleton<IMinioClient>(minioClient);

//new MediatRServiceConfiguration().RegisterServicesFromAssembly()
//builder.Services.AddMediatR(typeof(Program));
//builder.Services.AddMediatR(typeof(GetAllCustomersQuery).Assembly);
//Assembly.GetExecutingAssembly()
//AppDomain.CurrentDomain.GetAssemblies()

builder.Services.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    //var Handlers = AppDomain.CurrentDomain.Load("WebService.Handlers");
    //var Queries = AppDomain.CurrentDomain.Load("WebService.Queries");
    //var Commands = AppDomain.CurrentDomain.Load("WebService.Commands");

    configuration.RegisterServicesFromAssemblies(
        typeof(LogCommand).Assembly, 
        typeof(ObjectCommand).Assembly, 
        typeof(QueueCommand).Assembly, 
        typeof(LogHandler).Assembly,
        typeof(ObjectHandler).Assembly,
        typeof(HealthHandler).Assembly,
        typeof(QueueHandler).Assembly,
        typeof(HealthQuery).Assembly);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseReadableResponseStreamMiddleware();

app.UseLoggingMiddleware();

app.UseErrorHandlerMiddleware();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/v1/main"), appBuilder =>  // The path must be started with '/'
{
    appBuilder.UseRequestValidationMiddleware();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
