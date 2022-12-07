using Elasticsearch.Net;
using Helpers.Interfaces;
using Helpers.PingHelper;
using Microsoft.Extensions.DependencyInjection;
using Middleware;
using Nest;
using RabbitMQ.Client;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
//using MongoDB.Driver;
using ServiceLayer;
//using StackExchange.Redis;
using System.Data;
using IConnection = RabbitMQ.Client.IConnection;
using Minio;
using DataLayer.Interfaces;
using DataLayer.DataAccess;
using WebService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IConverterService, ConverterService>();
builder.Services.AddScoped<IConverterRepository, ConverterRepository>();
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IPingHelper, PingHelper>();

string? rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
string? rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
string? rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
string? rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
string? minioHost = Environment.GetEnvironmentVariable("MINIO_HOST");
string? minioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
string? minioSecretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");

//string? elkConn = Environment.GetEnvironmentVariable("elkConn");
//string? defaultIndex = Environment.GetEnvironmentVariable("default_index");
//string? elkUsername = Environment.GetEnvironmentVariable("elkUsername");
//string? elkPassword = Environment.GetEnvironmentVariable("elkPassword");

//ConnectionSettings? connection = new ConnectionSettings(new Uri(elkConn)).
//   DefaultIndex(defaultIndex).
//   ServerCertificateValidationCallback(CertificateValidations.AllowAll).
//   ThrowExceptions(true).
//   PrettyJson().
//   RequestTimeout(TimeSpan.FromSeconds(300)).
//   BasicAuthentication(elkUsername, elkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 

//ElasticClient? elasticClient = new ElasticClient(connection);
//builder.Services.AddSingleton<IElasticClient>(elasticClient);

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
                                    .WithSSL(false)
                                    .Build();
builder.Services.AddSingleton<IMinioClient>(minioClient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseReadableResponseStreamMiddleware();

app.UseLoggingMiddleware(); //If exception will happen, it wont enter ErrorHandlerMiddleware.

app.UseErrorHandlerMiddleware();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/v1/main"), appBuilder =>  // The path must be started with '/'
{
    appBuilder.UseUploadFileMiddleware();
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
