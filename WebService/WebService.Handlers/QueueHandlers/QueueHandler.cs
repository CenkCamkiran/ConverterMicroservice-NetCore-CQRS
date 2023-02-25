using MediatR;
using WebService.Commands.QueueCommands;
using WebService.Repositories.Interfaces;

namespace WebService.Handlers.QueueHandlers
{
    public class QueueHandler : IRequestHandler<QueueCommand, bool>
    {
        private readonly IQueueRepository _queueRepository;

        public QueueHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<bool> Handle(QueueCommand request, CancellationToken cancellationToken)
        {
            return await _queueRepository.QueueMessageDirectAsync(request.Message, request.Queue, request.Exchange, request.RoutingKey, request.MessageTTL);
        }
    }
}
