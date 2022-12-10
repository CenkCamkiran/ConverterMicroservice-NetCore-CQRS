namespace Models
{
    public class HealthResponse
    {
        public string RabbitMQStatus { get; set; } = string.Empty;
        public string StorageStatus { get; set; } = string.Empty;
        public string ElasticSearchStatus { get; set; } = string.Empty;
    }
}
