using Minio.DataModel;

namespace ConverterMicroservice.Models
{
    public class ObjectData
    {
        public ObjectStat? ObjectStats { get; set; }
        public string Mp4FileFullPath { get; set; } = string.Empty;
    }
}
