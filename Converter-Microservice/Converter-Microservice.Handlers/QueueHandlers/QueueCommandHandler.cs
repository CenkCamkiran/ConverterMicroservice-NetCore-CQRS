﻿using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.QueueHandlers
{
    public class QueueCommandHandler<TMessage> : IRequestHandler<QueueCommand<TMessage>> where TMessage : class
    {
        private readonly IQueueRepository<TMessage> _queueRepository;

        public QueueCommandHandler(IQueueRepository<TMessage> queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public Task Handle(QueueCommand<TMessage> request, CancellationToken cancellationToken)
        {
            _queueRepository.QueueMessageDirect(request.Message, request.Queue, request.Exchange, request.RoutingKey, request.MessageTTL);
            return Task.CompletedTask;
        }
    }
}
