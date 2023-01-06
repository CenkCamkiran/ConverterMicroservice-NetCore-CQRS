using Models;

namespace ServiceLayer.Interfaces
{
    public interface IHealthService
    {
        HealthResponse CheckHealthStatus();
    }
}
