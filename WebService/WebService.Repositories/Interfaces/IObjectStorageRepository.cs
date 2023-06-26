namespace WebService.Repositories.Interfaces
{
    public interface IObjectStorageRepository
    {
        Task<bool> PutObjectAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
