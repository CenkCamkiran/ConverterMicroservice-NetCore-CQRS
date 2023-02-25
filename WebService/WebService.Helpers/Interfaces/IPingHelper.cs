using System.Net.NetworkInformation;

namespace WebService.Helpers.Interfaces
{
    public interface IPingHelper
    {
        Task<PingReply> PingVMHost();
    }
}
