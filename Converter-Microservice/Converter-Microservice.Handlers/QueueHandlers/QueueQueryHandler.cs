using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.QueueHandlers
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
