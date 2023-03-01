using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.QueueHandlers
{
    public class QueueQueryHandler : IRequestHandler<QueueQuery, bool>
    {
        private readonly IQueueRepository<object> _queueRepository;

        public QueueQueryHandler(IQueueRepository<object> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<bool> Handle(QueueQuery request, CancellationToken cancellationToken)
        {
            _queueRepository.ConsumeQueue(request.Queue, request.MessageTTL);
            return await Task.FromResult(true);
        }
    }
}
