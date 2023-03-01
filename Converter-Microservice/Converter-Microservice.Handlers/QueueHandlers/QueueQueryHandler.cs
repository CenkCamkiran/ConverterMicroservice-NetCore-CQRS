using Converter_Microservice.Queries.QueueQueries;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.QueueHandlers
{
    public class QueueQueryHandler<TMessage> : IRequestHandler<QueueQuery, bool> where TMessage : class
    {

        private readonly IQueueRepository<TMessage> _queueRepository;

        public QueueQueryHandler(IQueueRepository<TMessage> queueRepository)
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
