using MediatR;
using Notification_Microservice.Commands.ObjectCommands;
using Notification_Microservice.Repositories.Interfaces;

namespace Notification_Microservice.Handlers.ObjectHandlers
{
    public class ObjectCommandHandler : IRequestHandler<ObjectCommand, bool>
    {
        private readonly IObjectRepository _objectRepository;

        public ObjectCommandHandler(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<bool> Handle(ObjectCommand request, CancellationToken cancellationToken)
        {
            return await _objectRepository.StoreFileAsync(request.BucketName, request.ObjectName, request.Stream, request.ContentType);
        }
    }
}
