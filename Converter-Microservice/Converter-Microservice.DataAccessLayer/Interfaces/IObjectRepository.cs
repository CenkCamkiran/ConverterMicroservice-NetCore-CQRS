using ConverterMicroservice.Models;

namespace Interfaces
{
    public interface IObjectRepository
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetFileAsync(string bucketName, string objectName);
    }
}
