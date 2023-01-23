namespace WebService.OperationLayer.Interfaces
{
    public interface IMinioStorageOperation
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
