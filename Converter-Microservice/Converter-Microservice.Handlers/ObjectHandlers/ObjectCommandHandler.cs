using Converter_Microservice.Commands.ObjectCommands;
using Converter_Microservice.Repositories.Interfaces;
using MediatR;

namespace Converter_Microservice.Handlers.ObjectHandlers
{
    public class ObjectCommandHandler : IRequestHandler<ObjectCommand, bool>
    {
        private readonly IObjectStorageRepository _objectRepository;

        public ObjectCommandHandler(IObjectStorageRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<bool> Handle(ObjectCommand request, CancellationToken cancellationToken)
        {
            return await _objectRepository.PutObjectAsync(request.BucketName, request.ObjectName, request.Stream, request.ContentType);
        }
    }
}
