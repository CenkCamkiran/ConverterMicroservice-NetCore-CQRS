using Logger_Microservice.Queries.QueueQueries;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.QueueHandlers
{
    public class QueueErrorQueryHandler : IRequestHandler<QueueErrorQuery>
    {

        private readonly IQueueRepository _queueRepository;

        public QueueErrorQueryHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task Handle(QueueErrorQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeErrorLogsQueue(request.Queue);

            await Task.CompletedTask;
        }
    }
}
