using System.Net.NetworkInformation;

namespace Helpers.Interfaces
{
    public interface IPingHelper
    {
        PingReply PingElasticSearch(string Host);
        PingReply PingRabbitMQ(string Host);
        PingReply PingStorage(string Host);
    }
}
