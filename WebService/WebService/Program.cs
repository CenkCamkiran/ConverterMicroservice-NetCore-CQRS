using Elasticsearch.Net;
using Minio;
using Nest;
using RabbitMQ.Client;
using WebService.Commands.LogCommands;
using WebService.Commands.ObjectCommands;
using WebService.Commands.QueueCommands;
using WebService.Common.Constants;
using WebService.Handlers.HealthHandlers;
using WebService.Handlers.LogHandlers;
using WebService.Handlers.ObjectHandlers;
using WebService.Handlers.QueueHandlers;
using WebService.Helpers.Helpers;
using WebService.Helpers.Interfaces;
using WebService.Middlewares;
using WebService.Middlewares.Contexts;
using WebService.Middlewares.Contexts.Interfaces;
using WebService.Queries.HealthQueries;
using WebService.Repositories.Interfaces;
using WebService.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    logging.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                            | ActivityTrackingOptions.TraceId
                                            | ActivityTrackingOptions.ParentId
                                            | ActivityTrackingOptions.Baggage
                                            | ActivityTrackingOptions.Tags;
    });
});


//Repository
builder.Services.AddScoped(typeof(ILogRepository<>), typeof(LogRepository<>));
builder.Services.AddScoped<IObjectRepository, ObjectRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<ILogOtherRepository, LogOtherRepository>();


builder.Services.AddScoped<IPingHelper, PingHelper>();

//Context
builder.Services.AddScoped<IWebServiceContext, WebServiceContext>();

ConnectionSettings? connection = new ConnectionSettings(new Uri(ProjectConstants.ElkHost)).
   DefaultIndex(ProjectConstants.ElkDefaultIndexName).
   ServerCertificateValidationCallback(CertificateValidations.AllowAll).
   ThrowExceptions(ProjectConstants.ElkExceptions).
   PrettyJson().
   RequestTimeout(TimeSpan.FromSeconds(ProjectConstants.ElkRequestTimeout)).
   BasicAuthentication(ProjectConstants.ElkUsername, ProjectConstants.ElkPassword);

ElasticClient? elasticClient = new ElasticClient(connection);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

var connectionFactory = new ConnectionFactory
{
    HostName = ProjectConstants.RabbitmqHost,
    Port = Convert.ToInt32(ProjectConstants.RabbitmqPort),
    UserName = ProjectConstants.RabbitmqUsername,
    Password = ProjectConstants.RabbitmqPassword
};
var rabbitConnection = connectionFactory.CreateConnection();
builder.Services.AddSingleton<RabbitMQ.Client.IConnection>(rabbitConnection);

MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(ProjectConstants.MinioHost)
                                    .WithCredentials(ProjectConstants.MinioAccessKey, ProjectConstants.MinioSecretKey)
                                    .WithSSL(ProjectConstants.MinioUseSsl);
builder.Services.AddSingleton<IMinioClient>(minioClient);

builder.Services.AddMediatR((MediatRServiceConfiguration configuration) =>
{

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

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/v1/main"), appBuilder =>  // The path must be started with '/'
{
    appBuilder.UseRequestValidationMiddleware();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
