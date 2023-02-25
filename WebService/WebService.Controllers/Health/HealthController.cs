using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;
using WebService.Queries.HealthQueries;

namespace WebService.Controllers.Health
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<HealthResponse> GetHealthStatus()
        {
            return await _mediator.Send(new HealthQuery());
        }
    }
}
