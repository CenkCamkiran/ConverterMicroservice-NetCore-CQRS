using MediatR;

namespace Logger_Microservice.Queries.QueueQueries
{
    public class QueueErrorQuery : IRequest
    {
        public string Queue { get; set; }

        public QueueErrorQuery(string queue)
        {
            Queue = queue;
        }
    }
}
