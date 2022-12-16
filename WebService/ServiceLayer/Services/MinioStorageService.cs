using DataLayer.Interfaces;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services
{
    public class MinioStorageService : IMinioStorageService
    {
        private IMinioStorageRepository _minioRepository;

        public MinioStorageService(IMinioStorageRepository minioRepository)
        {
            _minioRepository = minioRepository;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await _minioRepository.StoreFileAsync(bucketName, objectName, stream, contentType);
        }
    }
}
