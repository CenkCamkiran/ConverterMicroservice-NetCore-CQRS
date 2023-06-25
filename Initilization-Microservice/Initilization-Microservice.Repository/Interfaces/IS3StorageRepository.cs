namespace Initilization_Microservice.Repository.Interfaces
{
    public interface IS3StorageRepository
    {
        public Task<bool> ConfigureS3Async(string bucketname);
    }
}
