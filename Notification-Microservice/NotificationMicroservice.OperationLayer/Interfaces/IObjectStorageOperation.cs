using NotificationMicroservice.Models;

namespace NotificationMicroservice.OperationLayer.Interfaces
{
    public interface IObjectStorageOperation
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetFileAsync(string bucketName, string objectName);
    }
}
