using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
    public interface IPingHelper
    {
        PingReply PingElasticSearch(string Host);
        PingReply PingRabbitMQ(string Host);
        PingReply PingStorage(string Host);
    }
}
