using NotificationMicroservice.Models;

namespace Interfaces
{
    public interface IObjectStorageOperation
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetFileAsync(string bucketName, string objectName);
    }
}
