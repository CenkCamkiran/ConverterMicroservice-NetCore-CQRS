using Microsoft.AspNetCore.Mvc;
using WebService.Models;
using WebService.OperationLayer.Interfaces;

namespace WebService.APILayer.Controllers.Health
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private IHealthOperation _healthService;

        public HealthController(IHealthOperation healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public HealthResponse GetHealthStatus()
        {
            return _healthService.CheckHealthStatus();
        }
    }
}
