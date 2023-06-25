namespace Initilization_Microservice.Operation.Interfaces
{
    public interface IS3StorageOperation
    {
        public Task<bool> ConfigureS3Async(string bucketName);
    }
}
