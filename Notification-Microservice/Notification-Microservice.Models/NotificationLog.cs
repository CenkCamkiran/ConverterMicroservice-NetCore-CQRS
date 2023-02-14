namespace NotificationMicroservice.Models
{
    public class NotificationLog
    {
        public string Error { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
