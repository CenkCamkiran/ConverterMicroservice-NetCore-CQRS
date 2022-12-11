using Models;

namespace ServiceLayer.Interfaces
{
    public interface IHealthService
    {
        HealthResponse CheckHealthStatus(string RabbitMQHost, string ElasticHost, string StorageHost);
    }
}
