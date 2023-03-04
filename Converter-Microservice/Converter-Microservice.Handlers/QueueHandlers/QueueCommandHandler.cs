using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.QueueHandlers
{
    public class QueueCommandHandler : IRequestHandler<QueueCommand >    
    {
        private readonly IQueueRepository _queueRepository;

        public QueueCommandHandler(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task Handle(QueueCommand request, CancellationToken cancellationToken)
        {
            _queueRepository.QueueMessageDirect(request.Message, request.Queue, request.Exchange, request.RoutingKey, request.MessageTTL);
            await Task.CompletedTask;
        }
    }
}
