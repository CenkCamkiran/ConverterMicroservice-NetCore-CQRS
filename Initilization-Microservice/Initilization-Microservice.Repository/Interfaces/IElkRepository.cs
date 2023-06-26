namespace Initilization_Microservice.Repository.Interfaces
{
    public interface IElkRepository<TModel> where TModel : class
    {
        public Task<bool> ConfigureIndexAsync(string indexName, int numberOfShards, int numberOfReplicas);
    }
}
