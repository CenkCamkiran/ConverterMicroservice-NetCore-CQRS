using Converter_Microservice.Commands.ConverterCommands;
using Converter_Microservice.Commands.ObjectCommands;
using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Handlers.ConverterHandlers;
using Converter_Microservice.Handlers.LogHandlers;
using Converter_Microservice.Handlers.ObjectHandlers;
using Converter_Microservice.Handlers.QueueHandlers;
using Converter_Microservice.Queries.ObjectQueries;
using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using Converter_Microservice.Repositories.Providers;
using Converter_Microservice.Repositories.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using RabbitMQ.Client;
using WebService.Common.Constants;

var serviceProvider = new ServiceCollection();

MinioClient minioClient = new MinioClient()
                        .WithEndpoint(ProjectConstants.MinioHost)
                        .WithCredentials(ProjectConstants.MinioAccessKey, ProjectConstants.MinioSecretKey)
                        .WithSSL(false);
serviceProvider.AddSingleton<IMinioClient>(minioClient);


var connectionFactory = new ConnectionFactory
{
    HostName = ProjectConstants.RabbitmqHost,
    Port = Convert.ToInt32(ProjectConstants.RabbitmqPort),
    UserName = ProjectConstants.RabbitmqUsername,
    Password = ProjectConstants.RabbitmqPassword
};
IConnection rabbitConnection = connectionFactory.CreateConnection();

serviceProvider.AddSingleton(rabbitConnection);

//Repositories
serviceProvider.AddScoped(typeof(IQueueRepository), typeof(QueueRepository));
serviceProvider.AddScoped<IObjectRepository, ObjectRepository>();
serviceProvider.AddScoped<ILog4NetRepository, Log4NetRepository>();
serviceProvider.AddScoped<IConverterRepository, ConverterRepository>();

serviceProvider.AddMediatR((MediatRServiceConfiguration configuration) =>
{
    configuration.RegisterServicesFromAssemblies(
        typeof(ConverterHandler).Assembly,
        typeof(ObjectCommandHandler).Assembly,
        typeof(ObjectQueryHandler).Assembly,
        typeof(QueueCommandHandler).Assembly,
        typeof(QueueQueryHandler).Assembly,
        typeof(LogHandler).Assembly,
        typeof(ObjectCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(QueueCommand).Assembly,
        typeof(ConverterCommand).Assembly,
        typeof(ObjectQuery).Assembly,
        typeof(QueueQuery).Assembly
        );
});

serviceProvider.AddLazyResolution();
var builder = serviceProvider.BuildServiceProvider();

var _mediator = builder.GetService<IMediator>();
await _mediator.Send(new QueueQuery(ProjectConstants.ConverterServiceQueueName, ProjectConstants.ConverterExchangeTtl));
