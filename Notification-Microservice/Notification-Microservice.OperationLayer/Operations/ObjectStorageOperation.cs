using Interfaces;
using NotificationMicroservice.Models;

namespace Operations
{
    public class ObjectStorageOperation : IObjectStorageOperation
    {
        private IObjectRepository _objectStorageRepository;

        public ObjectStorageOperation(IObjectRepository objectStorageRepository)
        {
            _objectStorageRepository = objectStorageRepository;
        }

        public async Task<ObjectData> GetFileAsync(string bucketName, string objectName)
        {
            return await _objectStorageRepository.GetFileAsync(bucketName, objectName);

        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await _objectStorageRepository.StoreFileAsync(bucketName, objectName, stream, contentType);
        }
    }
}
