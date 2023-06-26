using Converter_Microservice.Queries.ObjectQueries;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using MediatR;

namespace Converter_Microservice.Handlers.ObjectHandlers
{
    public class ObjectQueryHandler : IRequestHandler<ObjectQuery, ObjectData>
    {

        private readonly IObjectStorageRepository _objectRepository;

        public ObjectQueryHandler(IObjectStorageRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<ObjectData> Handle(ObjectQuery request, CancellationToken cancellationToken)
        {
            return await _objectRepository.GetObjectAsync(request.BucketName, request.ObjectName);
        }
    }
}
