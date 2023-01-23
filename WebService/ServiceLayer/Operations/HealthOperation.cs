using System.Net.NetworkInformation;
using WebService.Helpers.Interfaces;
using WebService.Models;
using WebService.OperationLayer.Interfaces;

namespace WebService.OperationLayer.Operations
{
    public class HealthOperation : IHealthOperation
    {

        private IPingHelper _pingHelper;

        public HealthOperation(IPingHelper pingHelper)
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
