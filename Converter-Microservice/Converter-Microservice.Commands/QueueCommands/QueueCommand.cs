using MediatR;

namespace Converter_Microservice.Commands.QueueCommands
{
    public class QueueCommand : IRequest
    {
        public object Message { get; set; }
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public long MessageTTL { get; set; }

        public QueueCommand(object message, string queue, string exchange, string routingKey, long messageTTL)
        {
            Message = message;
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
            MessageTTL = messageTTL;
        }
    }
}
