using RabbitMQ.Client;

namespace Notification_Microservice.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string RabbitmqHost { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public static string RabbitmqPort { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        public static string RabbitmqUsername { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        public static string RabbitmqPassword { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

        public static int ConverterExchangeTtl { get; set; } = 43200000;
        public static string ConverterServiceExchangeName { get; set; } = "converter_exchange.direct";
        public static string ConverterServiceQueueName { get; set; } = "converter";
        public static string ConverterServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static string ConverterServiceRoutingKey { get; set; } = "mp4_to_mp3";

        public static int NotificationServiceExchangeTtl { get; set; } = 3600000;
        public static string NotificationServiceExchangeName { get; set; } = "notification_exchange.direct";
        public static string NotificationServiceQueueName { get; set; } = "notification";
        public static string NotificationServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static string NotificationServiceRoutingKey { get; set; } = "mp4_to_notif";

        public static int OtherLogsServiceExchangeTtl { get; set; } = 0;
        public static string OtherLogsServiceExchangeName { get; set; } = "log_exchange.direct";
        public static string OtherLogsServiceQueueName { get; set; } = "otherlogs";
        public static string OtherLogsServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static string OtherLogsServiceRoutingKey { get; set; } = "other_log";

        public static int ErrorLogsServiceExchangeTtl { get; set; } = 0;
        public static string ErrorLogsServiceExchangeName { get; set; } = "log_exchange.direct";
        public static string ErrorLogsServiceQueueName { get; set; } = "errorlogs";
        public static string ErrorLogsServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static string ErrorLogsServiceRoutingKey { get; set; } = "error_log";
    }
}
