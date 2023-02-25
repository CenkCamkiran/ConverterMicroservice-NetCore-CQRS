using MediatR;
using Notification_Microservice.Queries.QueueQueries;
using Notification_Microservice.Repositories.Interfaces;

namespace Notification_Microservice.Handlers.QueueHandlers
{
    public class QueueQueryHandler<TMessage> : IRequestHandler<QueueQuery> where TMessage : class
    {

        private readonly IQueueRepository<TMessage> _queueRepository;

        public QueueQueryHandler(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public Task Handle(QueueQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeQueue(request.Queue, request.MessageTTL);
            return Task.CompletedTask;
        }
    }
}
