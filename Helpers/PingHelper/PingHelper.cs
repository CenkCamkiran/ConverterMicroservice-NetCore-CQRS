using Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.PingHelper
{
    public class PingHelper: IPingHelper
    {

        public PingReply PingElasticSearch(string Host)
        {

            Ping ping = new Ping();

            Task<PingReply> pingResult = ping.SendPingAsync(Host, 15);

            return pingResult.Result;
        }

        public PingReply PingRabbitMQ(string Host)
        {

            Ping ping = new Ping();

            Task<PingReply> pingResult = ping.SendPingAsync(Host, 15);

            return pingResult.Result;
        }

        public PingReply PingStorage(string Host)
        {
            Ping ping = new Ping();

            Task<PingReply> pingResult = ping.SendPingAsync(Host, 15);

            return pingResult.Result;
        }
    }
}
