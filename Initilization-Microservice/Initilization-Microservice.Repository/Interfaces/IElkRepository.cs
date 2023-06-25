namespace Initilization_Microservice.Repository.Interfaces
{
    public interface IElkRepository
    {
        public Task<bool> ConfigureIndexAsync(string indexName, int numberOfShards, int numberOfReplicas);
    }
}
