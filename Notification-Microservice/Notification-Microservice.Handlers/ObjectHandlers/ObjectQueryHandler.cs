using MediatR;
using Notification_Microservice.Queries.ObjectQueries;
using Notification_Microservice.Repositories.Interfaces;
using NotificationMicroservice.Models;

namespace Notification_Microservice.Handlers.ObjectHandlers
{
    public class ObjectQueryHandler : IRequestHandler<ObjectQuery, ObjectData>
    {

        private readonly IObjectRepository _objectRepository;

        public ObjectQueryHandler(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public async Task<ObjectData> Handle(ObjectQuery request, CancellationToken cancellationToken)
        {
            return await _objectRepository.GetFileAsync(request.BucketName, request.ObjectName);
        }
    }
}
