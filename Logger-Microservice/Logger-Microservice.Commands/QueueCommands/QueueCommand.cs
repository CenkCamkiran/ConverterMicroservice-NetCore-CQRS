using MediatR;

namespace Logger_Microservice.Commands.QueueCommands
{
    public class QueueCommand : IRequest
    {
        public object Message { get; set; }
        public string Queue { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;

        public QueueCommand(object message, string queue, string exchange, string routingKey)
        {
            Message = message;
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
        }
    }
}
