using Logger_Microservice.Queries.QueueQueries;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.QueueHandlers
{
    public class QueueErrorQueryHandler<TMessage> : IRequestHandler<QueueQuery> where TMessage : class
    {

        private readonly IQueueRepository<TMessage> _queueRepository;

        public QueueErrorQueryHandler(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public Task Handle(QueueQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeErrorLogsQueue(request.Queue);

            return Task.CompletedTask;
        }
    }
}
