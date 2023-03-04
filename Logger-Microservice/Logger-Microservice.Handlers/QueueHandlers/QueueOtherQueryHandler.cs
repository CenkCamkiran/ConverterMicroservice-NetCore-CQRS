using Logger_Microservice.Queries.QueueQueries;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.QueueHandlers
{
    public class QueueOtherQueryHandler : IRequestHandler<QueueOtherQuery>
    {

        private readonly IQueueRepository _queueRepository;

        public QueueOtherQueryHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task Handle(QueueOtherQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeOtherLogsQueue(request.Queue);

            await Task.CompletedTask;
        }
    }
}
