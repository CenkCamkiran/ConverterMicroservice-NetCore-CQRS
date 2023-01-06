using Helpers.Interfaces;
using Models;
using ServiceLayer.Interfaces;
using System.Net.NetworkInformation;

namespace ServiceLayer.Services
{
    public class HealthService : IHealthService
    {

        private IPingHelper _pingHelper;

        public HealthService(IPingHelper pingHelper)
        {
            _pingHelper = pingHelper;
        }

        public HealthResponse CheckHealthStatus()
        {

            PingReply hostHealth = _pingHelper.PingVMHost();

            HealthResponse healthResponse = new HealthResponse();

            healthResponse.HostStatus = hostHealth.Status == (int)IPStatus.Success ? "Host is working!" : "Host is not working!";

            return healthResponse;

        }
    }
}
