namespace Models
{
    public class QueueLog
    {
        public string QueueName { get; set; } = String.Empty;
        public string ExchangeName { get; set; } = String.Empty;
        public string RoutingKey { get; set; } = String.Empty;
        public QueueMessage? Message { get; set; } = null;
        public DateTime Date { get; set; }

    }
}
