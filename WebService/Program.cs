using Elasticsearch.Net;
using Helpers.Interfaces;
using Helpers.PingHelper;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using RabbitMQ.Client;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using IConnection = RabbitMQ.Client.IConnection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IConverterService, ConverterService>();
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IPingHelper, PingHelper>();

//string? elkConn = Environment.GetEnvironmentVariable("elkConn");
//string? rabbitHost = Environment.GetEnvironmentVariable("rabbitHost");
//string? rabbitPort = Environment.GetEnvironmentVariable("rabbitPort");
//string? rabbitUsername = Environment.GetEnvironmentVariable("rabbitUsername");
//string? rabbitPassword = Environment.GetEnvironmentVariable("rabbitPassword");
//string? storageConn = Environment.GetEnvironmentVariable("storageConn");
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

//var connectionFactory = new ConnectionFactory
//{
//	HostName = rabbitHost,
//	Port = Convert.ToInt32(rabbitPort),
//	UserName = rabbitUsername,
//	Password = rabbitPassword
//};

//var rabbitConnection = connectionFactory.CreateConnection();
//builder.Services.AddSingleton<IConnection>(rabbitConnection);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
