using MediatR;

namespace Logger_Microservice.Queries.QueueQueries
{
    public class QueueOtherQuery : IRequest
    {
        public string Queue { get; set; }

        public QueueOtherQuery(string queue)
        {
            Queue = queue;
        }
    }
}
