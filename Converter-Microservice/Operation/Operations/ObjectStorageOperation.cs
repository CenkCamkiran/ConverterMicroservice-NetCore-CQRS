using DataAccess.Interfaces;
using DataAccess.Repository;
using Models;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
{
    public class ObjectStorageOperation : IObjectStorageOperation
    {
        private IObjectStorageRepository _objectStorageRepository;

        public ObjectStorageOperation(IObjectStorageRepository objectStorageRepository)
        {
            _objectStorageRepository = objectStorageRepository;
        }

        public async Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName)
        {
            return await _objectStorageRepository.GetFileAsync(bucketName, objectName); 

        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await _objectStorageRepository.StoreFileAsync(bucketName, objectName, stream, contentType);  
        }
    }
}
