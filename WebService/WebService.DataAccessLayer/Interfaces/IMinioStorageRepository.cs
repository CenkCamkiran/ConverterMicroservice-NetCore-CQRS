namespace WebService.DataAccessLayer.Interfaces
{
    public interface IMinioStorageRepository
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
