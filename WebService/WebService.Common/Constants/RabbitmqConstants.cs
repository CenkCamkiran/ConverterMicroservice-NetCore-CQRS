using RabbitMQ.Client;

namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string RabbitmqHost { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public static string RabbitmqPort { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        public static string RabbitmqUsername { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        public static string RabbitmqPassword { get; set; } = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

        public static int ExchangeTtl { get; set; } = 28800000;

        public static int ConverterExchangeTtl { get; set; } = 43200000;
        public static string ConverterServiceExchangeName { get; set; } = "converter_exchange.direct";
        public static string ConverterServiceQueueName { get; set; } = "converter";
        public static string ConverterServiceExchange_Type { get; set; } = ExchangeType.Direct;
        public static string ConverterServiceRoutingKey { get; set; } = "mp4_to_mp3";
    }
}
