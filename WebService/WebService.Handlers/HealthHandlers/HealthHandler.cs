using MediatR;
using System.Net.NetworkInformation;
using WebService.Helpers.Interfaces;
using WebService.Models;
using WebService.Queries.HealthQueries;

namespace WebService.Handlers.HealthHandlers
{
    public class HealthHandler : IRequestHandler<HealthQuery, HealthResponse>
    {

        private readonly IPingHelper _pingHelper;

        public HealthHandler(IPingHelper pingHelper)
        {
            _pingHelper = pingHelper;
        }

        public async Task<HealthResponse> Handle(HealthQuery request, CancellationToken cancellationToken)
        {
            PingReply hostHealth = await _pingHelper.PingVMHost();
            HealthResponse healthResponse = new HealthResponse();
            healthResponse.HostStatus = hostHealth.Status == (int)IPStatus.Success ? "Host is working!" : "Host is not working!";

            return healthResponse;
        }
    }
}
