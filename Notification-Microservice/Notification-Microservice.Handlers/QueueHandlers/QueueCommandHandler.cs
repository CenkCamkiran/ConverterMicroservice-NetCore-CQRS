using MediatR;
using Notification_Microservice.Commands.QueueCommands;
using Notification_Microservice.Repositories.Interfaces;

namespace Notification_Microservice.Handlers.QueueHandlers
{
    public class QueueCommandHandler : IRequestHandler<QueueCommand>
    {
        private readonly IQueueRepository _queueRepository;

        public QueueCommandHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task Handle(QueueCommand request, CancellationToken cancellationToken)
        {
            _queueRepository.QueueMessageDirect(request.Message, request.Queue, request.Exchange, request.RoutingKey);
            await Task.CompletedTask;
        }
    }
}
