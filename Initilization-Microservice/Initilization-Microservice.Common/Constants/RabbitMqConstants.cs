using RabbitMQ.Client;

namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string RabbitmqHost { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public static string RabbitmqPort { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        public static string RabbitmqUsername { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_ConverterNAME");
        public static string RabbitmqPassword { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

        public static int ConverterServiceExchangeTtl { get; set; } = 43200000;
        public static string ConverterServiceExchangeName { get; set; } = "converter_exchange.direct";
        public static string ConverterServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool ConverterServiceExchangeIsDurable { get; set; } = true;
        public static bool ConverterServiceExchangeIsAutoDelete { get; set; } = false;
        public static string ConverterServiceQueueName { get; set; } = "converter";
        public static bool ConverterServiceQueueIsDurable { get; set; } = true;
        public static bool ConverterServiceQueueIsExclusive { get; set; } = false;
        public static bool ConverterServiceQueueIsAutoDelete { get; set; } = false;
        public static string ConverterServiceRoutingKey { get; set; } = "mp4_to_mp3";

        public static int LoggerServiceExchangeTtl { get; set; } = 0;
        public static string LoggerServiceExchangeName { get; set; } = "log_exchange.direct";
        public static string LoggerServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool LoggerServiceExchangeIsDurable { get; set; } = true;
        public static bool LoggerServiceExchangeIsAutoDelete { get; set; } = false;
        public static string ErrorLoggerServiceQueueName { get; set; } = "errorlogs";
        public static string OtherLoggerServiceQueueName { get; set; } = "otherlogs";
        public static bool LoggerServiceQueueIsDurable { get; set; } = true;
        public static bool LoggerServiceQueueIsExclusive { get; set; } = false;
        public static bool LoggerServiceQueueIsAutoDelete { get; set; } = false;
        public static string OtherLogsServiceRoutingKey { get; set; } = "other_log";
        public static string ErrorLogsServiceRoutingKey { get; set; } = "error_log";

        public static int NotificationServiceExchangeTtl { get; set; } = 3600000;
        public static string NotificationServiceExchangeName { get; set; } = "notification_exchange.direct";
        public static string NotificationServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static bool NotificationServiceExchangeIsDurable { get; set; } = true;
        public static bool NotificationServiceExchangeIsAutoDelete { get; set; } = false;
        public static string NotificationServiceQueueName { get; set; } = "notification";
        public static bool NotificationServiceQueueIsDurable { get; set; } = true;
        public static bool NotificationServiceQueueIsExclusive { get; set; } = false;
        public static bool NotificationServiceQueueIsAutoDelete { get; set; } = false;
        public static string NotificationServiceRoutingKey { get; set; } = "mp4_to_notif";
    }
}
