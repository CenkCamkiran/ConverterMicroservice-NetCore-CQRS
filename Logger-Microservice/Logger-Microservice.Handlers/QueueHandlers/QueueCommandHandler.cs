using Logger_Microservice.Commands.QueueCommands;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.QueueHandlers
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
