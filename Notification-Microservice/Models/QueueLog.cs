namespace NotificationMicroservice.Models
{
    public class QueueLog
    {
        public string OperationType { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public string? Message { get; set; } = null;
        public DateTime Date { get; set; }
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}
