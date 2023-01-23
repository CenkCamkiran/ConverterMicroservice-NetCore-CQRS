using Elasticsearch.Net;
using Minio;
using Nest;
using RabbitMQ.Client;
using WebService.Configuration;
using WebService.DataAccessLayer.Interfaces;
using WebService.DataAccessLayer.Repositories;
using WebService.Helpers.Helpers;
using WebService.Helpers.Interfaces;
using WebService.MiddlewareLayer;
using WebService.MiddlewareLayer.Contexts;
using WebService.MiddlewareLayer.Contexts.Interfaces;
using WebService.Models;
using WebService.OperationLayer.Interfaces;
using WebService.OperationLayer.Operations;
//using MongoDB.Driver;
//using StackExchange.Redis;
using IConnection = RabbitMQ.Client.IConnection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
ElkConfiguration elkEnvVariables = envVariablesHandler.GetElkEnvVariables();
RabbitMqConfiguration rabbitEnvVariables = envVariablesHandler.GetRabbitEnvVariables();
MinioConfiguration minioEnvVariables = envVariablesHandler.GetMinioEnvVariables();

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
builder.Services.AddScoped(typeof(ILoggingRepository<>), typeof(LoggingRepository<>));
builder.Services.AddScoped<ILog4NetRepository, Log4NetRepository>();
builder.Services.AddScoped<IMinioStorageRepository, MinioStorageRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<ILogOtherRepository, LogOtherRepository>();

//Service
builder.Services.AddScoped<IQueueOperation, QueueOperation>();
builder.Services.AddScoped<IMinioStorageOperation, MinioStorageOperation>();
builder.Services.AddScoped<IHealthOperation, HealthOperation>();
builder.Services.AddScoped<ILoggingOperation, LoggingOperation>();
builder.Services.AddScoped<IPingHelper, PingHelper>();

//Context
builder.Services.AddScoped<IWebServiceContext, WebServiceContext>();

ConnectionSettings? connection = new ConnectionSettings(new Uri(elkEnvVariables.ElkHost)).
   DefaultIndex(elkEnvVariables.ElkDefaultIndex).
   ServerCertificateValidationCallback(CertificateValidations.AllowAll).
   ThrowExceptions(true).
   PrettyJson().
   RequestTimeout(TimeSpan.FromSeconds(300)).
   BasicAuthentication(elkEnvVariables.ElkUsername, elkEnvVariables.ElkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 

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
builder.Services.AddSingleton<IConnection>(rabbitConnection);

MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(minioEnvVariables.MinioHost)
                                    .WithCredentials(minioEnvVariables.MinioAccessKey, minioEnvVariables.MinioSecretKey)
                                    .WithSSL(false);
builder.Services.AddSingleton<IMinioClient>(minioClient);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
