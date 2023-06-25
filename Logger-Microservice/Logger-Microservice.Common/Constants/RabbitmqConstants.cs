using RabbitMQ.Client;

namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string RabbitmqHost { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public static string RabbitmqPort { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        public static string RabbitmqUsername { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        public static string RabbitmqPassword { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

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
