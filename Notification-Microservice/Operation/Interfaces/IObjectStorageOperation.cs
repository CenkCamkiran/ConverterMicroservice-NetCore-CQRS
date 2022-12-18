namespace Operation.Interfaces
{
    public interface IObjectStorageOperation
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectDataModel> GetFileAsync(string bucketName, string objectName);
    }
}
