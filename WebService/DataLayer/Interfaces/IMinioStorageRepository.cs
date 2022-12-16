namespace DataLayer.Interfaces
{
    public interface IMinioStorageRepository
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
