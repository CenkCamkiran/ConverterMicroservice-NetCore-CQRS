using Microsoft.AspNetCore.Mvc;
using Models;
using ServiceLayer.Interfaces;

namespace APILayer.Health
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        private IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public HealthResponse GetHealthStatus()
        {
            string RabbitMQHost = "";
            string ElasticHost = "";
            string StorageHost = "";

            return _healthService.CheckHealthStatus(RabbitMQHost, ElasticHost, StorageHost);
        }
    }
}
