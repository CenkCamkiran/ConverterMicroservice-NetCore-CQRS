using MediatR;
using Notification_Microservice.Queries.QueueQueries;
using Notification_Microservice.Repositories.Interfaces;

namespace Notification_Microservice.Handlers.QueueHandlers
{
    public class QueueQueryHandler : IRequestHandler<QueueQuery>
    {

        private readonly IQueueRepository _queueRepository;

        public QueueQueryHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task Handle(QueueQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeQueue(request.Queue, request.MessageTTL);
            await Task.CompletedTask;
        }
    }
}
