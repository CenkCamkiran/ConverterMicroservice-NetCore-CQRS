namespace NotificationMicroservice.Models
{
    public class QueueMessage
    {
        public string fileGuid { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
