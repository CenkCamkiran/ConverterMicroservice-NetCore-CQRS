using Minio.DataModel;

namespace NotificationMicroservice.Models
{
    public class ObjectData
    {
        public ObjectStat? ObjectStats { get; set; }
        public string Mp3FileFullPath { get; set; } = string.Empty;
    }
}
