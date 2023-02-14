namespace ConverterMicroservice.Models
{
    public class OtherLog
    {
        public QueueLog? queueLog { get; set; }
        public ObjectStorageLog? storageLog { get; set; }
        public ConverterLog? converterLog { get; set; }
    }
}
