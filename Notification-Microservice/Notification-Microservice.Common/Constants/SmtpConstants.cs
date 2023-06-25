namespace Notification_Microservice.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string SmtpHost { get; set; } = Environment.GetEnvironmentVariable("SMTP_HOST");
        public static string SmtpPort { get; set; } = Environment.GetEnvironmentVariable("SMTP_PORT");
        public static string SmtpMailFrom { get; set; } = Environment.GetEnvironmentVariable("SMTP_MAIL_FROM");
        public static string SmtpMailPassword { get; set; } = Environment.GetEnvironmentVariable("SMTP_MAIL_PASSWORD");
        public static string SmtpMailUsername { get; set; } = Environment.GetEnvironmentVariable("SMTP_MAIL_USERNAME");
        public static string MailSubject { get; set; } = "Here is your file";
    }
}
