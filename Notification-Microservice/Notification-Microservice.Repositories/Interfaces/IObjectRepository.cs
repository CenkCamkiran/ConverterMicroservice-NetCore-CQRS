using NotificationMicroservice.Models;

namespace Notification_Microservice.Repositories.Interfaces
{
    public interface IObjectRepository
    {
        Task<bool> StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetFileAsync(string bucketName, string objectName);
    }
}
