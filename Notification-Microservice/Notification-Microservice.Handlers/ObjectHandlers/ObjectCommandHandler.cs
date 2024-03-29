﻿using MediatR;
using Notification_Microservice.Commands.ObjectCommands;
using Notification_Microservice.Repositories.Interfaces;

namespace Notification_Microservice.Handlers.ObjectHandlers
{
    public class ObjectCommandHandler : IRequestHandler<ObjectCommand>
    {
        private readonly IObjectStorageRepository _objectRepository;

        public ObjectCommandHandler(IObjectStorageRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task Handle(ObjectCommand request, CancellationToken cancellationToken)
        {
            await _objectRepository.StoreFileAsync(request.BucketName, request.ObjectName, request.Stream, request.ContentType);
        }
    }
}
