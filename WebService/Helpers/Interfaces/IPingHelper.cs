using System.Net.NetworkInformation;

namespace Helpers.Interfaces
{
    public interface IPingHelper
    {
        PingReply PingVMHost();
    }
}
