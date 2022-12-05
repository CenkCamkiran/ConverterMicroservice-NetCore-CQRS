using Models.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IHealthService
    {
        HealthResponse CheckHealthStatus(string RabbitMQHost, string ElasticHost, string StorageHost);
    }
}
