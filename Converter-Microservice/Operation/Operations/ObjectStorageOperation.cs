using ConverterMicroservice.DataAccessLayer.Interfaces;
using ConverterMicroservice.Models;
using ConverterMicroservice.OperationLayer.Interfaces;

namespace ConverterMicroservice.OperationLayer.Operations
{
    public class ObjectStorageOperation : IObjectStorageOperation
    {
        private IObjectStorageRepository _objectStorageRepository;

        public ObjectStorageOperation(IObjectStorageRepository objectStorageRepository)
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
