using MediatR;
using WebService.Models;

namespace WebService.Commands.QueueCommands
{
    public class QueueCommand : IRequest<bool>
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public QueueMessage Message { get; set; }
        public long MessageTTL { get; set; }

        public QueueCommand(string exchange, string routingKey, QueueMessage message, long messageTTL)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Message = message;
            MessageTTL = messageTTL;
        }
    }
}
