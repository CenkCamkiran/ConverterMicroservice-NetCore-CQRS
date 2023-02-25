namespace WebService.Repositories.Interfaces
{
    public interface IObjectRepository
    {
        Task<bool> StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
