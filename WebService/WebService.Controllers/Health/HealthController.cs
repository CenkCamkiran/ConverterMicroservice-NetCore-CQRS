using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebService.Common.Events;
using WebService.Models;
using WebService.Queries.HealthQueries;

namespace WebService.Controllers.Health
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IMediator mediator, ILogger<HealthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<HealthResponse> GetHealthStatus()
        {
            _logger.LogInformation(LogEvents.HealthCheckRequestReceived, LogEvents.HealthCheckRequestReceivedMessage);
            return await _mediator.Send(new HealthQuery());
        }
    }
}
