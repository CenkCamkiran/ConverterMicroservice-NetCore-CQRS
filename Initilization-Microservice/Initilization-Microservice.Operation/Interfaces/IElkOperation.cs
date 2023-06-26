namespace Initilization_Microservice.Operation.Interfaces
{
    public interface IElkOperation<TModel> where TModel : class
    {
        public Task<bool> ConfigureIndex(string indexName, int numberOfShards, int numberOfReplicas);
    }
}
