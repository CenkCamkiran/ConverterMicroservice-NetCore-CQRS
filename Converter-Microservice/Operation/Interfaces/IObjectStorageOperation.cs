using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface IObjectStorageOperation
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName);
    }
}
