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
        private ObjectStorageRepository objectStorageRepository = new ObjectStorageRepository();
        private QueueRepository<object> queueRepository = new QueueRepository<object>();

        public async Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName)
        {
            return await objectStorageRepository.GetFileAsync(bucketName, objectName); 

        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await objectStorageRepository.StoreFileAsync(bucketName, objectName, stream, contentType);  
        }
    }
}
