namespace NotificationMicroservice.Models
{
    public class ErrorLog
    {
        public QueueLog? queueLog { get; set; }
        public ObjectStorageLog? storageLog { get; set; }
        public ConverterLog? converterLog { get; set; }
        public LoggerLog? loggerLog { get; set; }
        public NotificationLog? notificationLog { get; set; }
    }
}
