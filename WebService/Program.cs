using DataLayer.DataAccess;
using DataLayer.Interfaces;
using Elasticsearch.Net;
using Helpers;
using Helpers.Interfaces;
using Middleware;
using Minio;
using Nest;
using RabbitMQ.Client;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
//using MongoDB.Driver;
//using StackExchange.Redis;
using IConnection = RabbitMQ.Client.IConnection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IConverterService, ConverterService>();
builder.Services.AddScoped<IConverterRepository, ConverterRepository>();
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IPingHelper, PingHelper>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<ILoggingRepository, LoggingRepository>();

string? rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
string? rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
string? rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
string? rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

string? minioHost = Environment.GetEnvironmentVariable("MINIO_HOST");
string? minioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
string? minioSecretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");

string? elkHost = Environment.GetEnvironmentVariable("ELK_HOST");
string? elkDefaultIndex = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX");
string? elkUsername = Environment.GetEnvironmentVariable("ELK_USERNAME");
string? elkPassword = Environment.GetEnvironmentVariable("ELK_PASSWORD");

ConnectionSettings? connection = new ConnectionSettings(new Uri(elkHost)).
   DefaultIndex(elkDefaultIndex).
   ServerCertificateValidationCallback(CertificateValidations.AllowAll).
   ThrowExceptions(true).
   PrettyJson().
   RequestTimeout(TimeSpan.FromSeconds(300)).
   BasicAuthentication(elkUsername, elkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 

ElasticClient? elasticClient = new ElasticClient(connection);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

var connectionFactory = new ConnectionFactory
{
    HostName = rabbitmqHost,
    Port = Convert.ToInt32(rabbitmqPort),
    UserName = rabbitmqUsername,
    Password = rabbitmqPassword
};
var rabbitConnection = connectionFactory.CreateConnection();
builder.Services.AddSingleton<IConnection>(rabbitConnection);

MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(minioHost)
                                    .WithCredentials(minioAccessKey, minioSecretKey)
                                    .WithSSL(false);
builder.Services.AddSingleton<IMinioClient>(minioClient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
