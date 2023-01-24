using System.Net.NetworkInformation;

namespace WebService.Helpers.Interfaces
{
    public interface IPingHelper
    {
        PingReply PingVMHost();
    }
}
