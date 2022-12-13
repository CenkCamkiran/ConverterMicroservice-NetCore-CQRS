using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IMinioStorageService
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
