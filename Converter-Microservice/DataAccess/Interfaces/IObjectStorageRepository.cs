using Models;

namespace DataAccess.Interfaces
{
    public interface IObjectStorageRepository
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName);
    }
}
