using WebService.DataAccessLayer.Interfaces;
using WebService.OperationLayer.Interfaces;

namespace WebService.OperationLayer.Operations
{
    public class MinioStorageOperation : IMinioStorageOperation
    {
        private IMinioStorageRepository _minioRepository;

        public MinioStorageOperation(IMinioStorageRepository minioRepository)
        {
            _minioRepository = minioRepository;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await _minioRepository.StoreFileAsync(bucketName, objectName, stream, contentType);
        }
    }
}
