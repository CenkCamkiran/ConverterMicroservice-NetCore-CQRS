using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class HealthResponse
    {
        public string RabbitMQStatus { get; set; } = string.Empty;
        public string StorageStatus { get; set; } = string.Empty;
        public string ElasticSearchStatus { get; set; } = string.Empty;
    }
}
