namespace NotificationMicroservice.Models
{
    public class SmtpConfiguration
    {
        public string SmtpHost { get; set; } = string.Empty;
        public string SmtpPort { get; set; } = string.Empty;
        public string SmtpMailFrom { get; set; } = string.Empty;
        public string SmtpMailPassword { get; set; } = string.Empty;
        public string SmtpMailUsername { get; set; } = string.Empty;
    }
}
