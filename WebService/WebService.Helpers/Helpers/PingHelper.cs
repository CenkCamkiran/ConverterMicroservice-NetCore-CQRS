using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using WebService.Common.Constants;
using WebService.Exceptions;
using WebService.Helpers.Interfaces;
using WebService.Models;

namespace WebService.Helpers.Helpers
{
    public class PingHelper : IPingHelper
    {
        public async Task<PingReply> PingVMHost()
        {
            try
            {
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

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

        }

    }
}
