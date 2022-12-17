using Minio.DataModel;

namespace Models
{
    public class ObjectDataModel
    {
        public ObjectStat? ObjectStats { get; set; }
        public string FileFullPath { get; set; } = string.Empty;
    }
}
