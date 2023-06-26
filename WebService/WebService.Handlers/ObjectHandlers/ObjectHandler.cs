using MediatR;
using WebService.Commands.ObjectCommands;
using WebService.Repositories.Interfaces;

namespace WebService.Handlers.ObjectHandlers
{
    public class ObjectHandler : IRequestHandler<ObjectCommand, bool>
    {

        private readonly IObjectStorageRepository _objectRepository;

        public ObjectHandler(IObjectStorageRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<bool> Handle(ObjectCommand request, CancellationToken cancellationToken)
        {
            return await _objectRepository.PutObjectAsync(request.BucketName, request.ObjectName, request.Stream, request.ContentType);
        }
    }
}
