namespace Models
{
    public class QueueLog
    {
        public string OperationType { get; set; } = String.Empty;
        public string QueueName { get; set; } = String.Empty;
        public string ExchangeName { get; set; } = String.Empty;
        public string RoutingKey { get; set; } = String.Empty;
        public string? Message { get; set; } = null;
        public DateTime Date { get; set; }
        public string ExceptionMessage { get; set; } = String.Empty;
    }
}
