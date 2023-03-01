using MediatR;

namespace Converter_Microservice.Queries.QueueQueries
{
    public class QueueQuery : IRequest<bool>
    {
        public string Queue { get; set; } = string.Empty;
        public long MessageTTL { get; set; }

        public QueueQuery(string queue, long messageTTL)
        {
            Queue = queue;
            MessageTTL = messageTTL;
        }
    }
}
