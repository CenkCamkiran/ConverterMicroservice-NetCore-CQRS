using System;

namespace LoggerMicroservice.Models
{
    public class ObjectStorageLog
    {
        public string OperationType { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long ContentLength { get; set; }
        public DateTime Date { get; set; }
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}
