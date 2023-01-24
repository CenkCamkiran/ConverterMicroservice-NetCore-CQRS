namespace WebService.Models
{
    public class QueueLog
    {
        public string QueueName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public QueueMessage? Message { get; set; } = null;
        public DateTime Date { get; set; }

    }
}
