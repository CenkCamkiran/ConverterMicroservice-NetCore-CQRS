using RabbitMQ.Client;

namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string RabbitmqHost { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public static string RabbitmqPort { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        public static string RabbitmqUsername { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        public static string RabbitmqPassword { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
        public static int ExchangeTtl { get; set; } = 28800000;
        public static string UserServiceExchangeName { get; set; } = "chat_usersvc.direct";
        public static string UserServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool UserServiceExchangeIsDurable { get; set; } = true;
        public static bool UserServiceExchangeIsAutoDelete { get; set; } = false;
        public static string UserServiceQueueName { get; set; } = "chat_usersvc_logs";
        public static bool UserServiceQueueIsDurable { get; set; } = true;
        public static bool UserServiceQueueIsExclusive { get; set; } = false;
        public static bool UserServiceQueueIsAutoDelete { get; set; } = false;
        public static string UserServiceRoutingKey { get; set; } = "chat_user_service";

        public static string StorageServiceExchangeName { get; set; } = "chat_storagesvc.direct";
        public static string StorageServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool StorageServiceExchangeIsDurable { get; set; } = true;
        public static bool StorageServiceExchangeIsAutoDelete { get; set; } = false;
        public static string StorageServiceQueueName { get; set; } = "chat_storagesvc_logs";
        public static bool StorageServiceQueueIsDurable { get; set; } = true;
        public static bool StorageServiceQueueIsExclusive { get; set; } = false;
        public static bool StorageServiceQueueIsAutoDelete { get; set; } = false;
        public static string StorageServiceRoutingKey { get; set; } = "chat_storage_service";

        public static string ChatBrokerServiceExchangeName { get; set; } = "chat_brokersvc.direct";
        public static string ChatBrokerServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool ChatBrokerServiceExchangeIsDurable { get; set; } = true;
        public static bool ChatBrokerServiceExchangeIsAutoDelete { get; set; } = false;
        public static string ChatBrokerServiceQueueName { get; set; } = "chat_brokersvc_logs";
        public static bool ChatBrokerServiceQueueIsDurable { get; set; } = true;
        public static bool ChatBrokerServiceQueueIsExclusive { get; set; } = false;
        public static bool ChatBrokerServiceQueueIsAutoDelete { get; set; } = false;
        public static string ChatBrokerServiceRoutingKey { get; set; } = "chat_broker_service";
    }
}
