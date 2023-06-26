using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using WebService.Common.Constants;
using WebService.Common.Events;
using WebService.Exceptions;
using WebService.Helpers.Interfaces;
using WebService.Models;

namespace WebService.Helpers.Helpers
{
    public class PingHelper : IPingHelper
    {

        private readonly ILogger<PingHelper> _logger;

        public PingHelper(ILogger<PingHelper> logger)
        {
            _logger = logger;
        }

        public async Task<PingReply> PingVMHost()
        {
            try
            {
                _logger.LogInformation(LogEvents.HealthCheckRequestReceived, LogEvents.HealthCheckRequestReceivedMessage);
                Uri uri = new Uri(ProjectConstants.ElkHost);

                Ping ping = new Ping();
                var pingResult = await ping.SendPingAsync(uri.Host, 60000);

                return pingResult;
            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                _logger.LogError(error.ErrorCode, exception.Message.ToString());

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

        }

    }
}
