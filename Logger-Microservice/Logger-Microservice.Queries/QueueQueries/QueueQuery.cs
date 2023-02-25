using MediatR;

namespace Logger_Microservice.Queries.QueueQueries
{
    public class QueueQuery : IRequest
    {
        public string Queue { get; set; } = string.Empty;

        public QueueQuery(string queue)
        {
            Queue = queue;
        }
    }
}
