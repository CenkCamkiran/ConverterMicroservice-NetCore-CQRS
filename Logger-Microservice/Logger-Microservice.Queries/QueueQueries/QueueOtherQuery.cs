using MediatR;

namespace Logger_Microservice.Queries.QueueQueries
{
    public class QueueOtherQuery : IRequest
    {
        public string Queue { get; set; } = string.Empty;

        public QueueOtherQuery(string queue)
        {
            Queue = queue;
        }
    }
}
