using Elasticsearch.Net;
using Minio;
using Nest;
using RabbitMQ.Client;
using WebService.Commands.LogCommands;
using WebService.Commands.ObjectCommands;
using WebService.Commands.QueueCommands;
using WebService.Common.Constants;
using WebService.Common.Events;
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
builder.Services.AddScoped<IObjectStorageRepository, ObjectStorageRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();


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

ServiceProvider services = builder.Services.BuildServiceProvider();
ILogger<Program>? logger = services.GetService<ILogger<Program>>();

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");

logger.LogInformation(LogEvents.ServiceConfigurationPhase, LogEvents.ServiceConfigurationPhaseMessage);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_HOST: " + ProjectConstants.RabbitmqHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_PORT: " + ProjectConstants.RabbitmqPort);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_USERNAME: " + ProjectConstants.RabbitmqUsername);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "RABBITMQ_PASSWORD: " + ProjectConstants.RabbitmqPassword);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_HOST: " + ProjectConstants.ElkHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_DEFAULT_INDEX_NAME: " + ProjectConstants.ElkUsername);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_USERNAME: " + ProjectConstants.ElkPassword);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "ELK_PASSWORD " + ProjectConstants.ElkDefaultIndexName);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_HOST: " + ProjectConstants.MinioHost);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_USE_SSL: " + ProjectConstants.MinioUseSsl);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_ACCESS_KEY: " + ProjectConstants.MinioAccessKey);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_SECRET_KEY " + ProjectConstants.MinioSecretKey);
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "MINIO_VIDEO_BUCKET_NAME " + ProjectConstants.MinioVideoBucket);

logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");
logger.LogInformation(LogEvents.ServiceConfigurationPhase, "********************************************************************************");

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
