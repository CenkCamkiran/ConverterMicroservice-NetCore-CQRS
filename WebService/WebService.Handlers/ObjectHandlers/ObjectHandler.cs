using MediatR;
using WebService.Commands.ObjectCommands;
using WebService.Repositories.Interfaces;

namespace WebService.Handlers.ObjectHandlers
{
    public class ObjectHandler : IRequestHandler<ObjectCommand, bool>
    {

        private readonly IObjectRepository _objectRepository;

        public ObjectHandler(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<bool> Handle(ObjectCommand request, CancellationToken cancellationToken)
        {
            return await _objectRepository.StoreFileAsync(request.BucketName, request.ObjectName, request.Stream, request.ContentType);
        }
    }
}
