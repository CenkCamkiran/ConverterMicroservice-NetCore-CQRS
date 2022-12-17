namespace Models
{
    public class ErrorLog
    {
        public QueueLog? queueLog { get; set; }
        public ObjectStorageLog? storageLog { get; set; }
        public ConverterLog? converterLog { get; set; }
        public ConsumerExceptionModel? exceptionModel { get; set; }
    }
}
