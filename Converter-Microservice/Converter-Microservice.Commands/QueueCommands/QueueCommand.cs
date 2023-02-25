using MediatR;

namespace Converter_Microservice.Commands.QueueCommands
{
    public class QueueCommand<TMessage> : IRequest
    {
        public TMessage Message { get; set; }
        public string Queue { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public long MessageTTL { get; set; }

        public QueueCommand(TMessage message, string queue, string exchange, string routingKey, long messageTTL)
        {
            Message = message;
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
            MessageTTL = messageTTL;
        }
    }
}
