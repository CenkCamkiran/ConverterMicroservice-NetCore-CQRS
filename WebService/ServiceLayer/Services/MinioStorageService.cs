using DataLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
