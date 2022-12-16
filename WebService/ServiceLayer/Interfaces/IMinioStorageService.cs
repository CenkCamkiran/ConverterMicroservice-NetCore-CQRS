namespace ServiceLayer.Interfaces
{
    public interface IMinioStorageService
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
