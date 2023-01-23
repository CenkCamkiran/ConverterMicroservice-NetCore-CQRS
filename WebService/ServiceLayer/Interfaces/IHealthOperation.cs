using WebService.Models;

namespace WebService.OperationLayer.Interfaces
{
    public interface IHealthOperation
    {
        HealthResponse CheckHealthStatus();
    }
}
