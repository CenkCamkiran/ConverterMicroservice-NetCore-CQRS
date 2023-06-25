namespace Initilization_Microservice.Operation.Interfaces
{
    public interface IElkOperation
    {
        public Task<bool> ConfigureIndex(string indexName, int numberOfShards, int numberOfReplicas);
    }
}
