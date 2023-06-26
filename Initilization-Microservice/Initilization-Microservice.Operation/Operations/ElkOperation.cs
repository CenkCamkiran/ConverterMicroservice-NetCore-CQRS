using Initilization_Microservice.Operation.Interfaces;
using Initilization_Microservice.Repository.Interfaces;

namespace Initilization_Microservice.Operation.Operations
{
    public class ElkOperation<TModel> : IElkOperation<TModel> where TModel : class
    {

        private readonly IElkRepository<TModel> _elkRepository;

        public ElkOperation(IElkRepository<TModel> elkRepository)
        {
            _elkRepository = elkRepository;
        }

        public Task<bool> ConfigureIndex(string indexName, int numberOfShards, int numberOfReplicas)
        {
            return _elkRepository.ConfigureIndexAsync(indexName, numberOfShards, numberOfReplicas);
        }
    }
}
