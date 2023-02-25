using MediatR;
using WebService.Models;

namespace WebService.Commands.QueueCommands
{
    public class QueueCommand : IRequest<bool>
    {
        public string Queue { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public QueueMessage Message { get; set; }
        public long MessageTTL { get; set; }

        public QueueCommand(string queue, string exchange, string routingKey, QueueMessage message, long messageTTL)
        {
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
            Message = message;
            MessageTTL = messageTTL;
        }
    }
}
