using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ObjectDataModel
    {
        public ObjectStat? ObjectStats { get; set; }
        public string FileFullPath { get; set; } = String.Empty;
    }
}
