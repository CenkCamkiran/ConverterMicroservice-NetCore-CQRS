using Initilization_Microservice.Operation.Interfaces;
using Initilization_Microservice.Repository.Interfaces;

namespace Initilization_Microservice.Operation.Operations
{
    public class ElkOperation : IElkOperation
    {

        private readonly IElkRepository _elkRepository;

        public ElkOperation(IElkRepository elkRepository)
        {
            _elkRepository = elkRepository;
        }

        public Task<bool> ConfigureIndex(string indexName, int numberOfShards, int numberOfReplicas)
        {
            return _elkRepository.ConfigureIndexAsync(indexName, numberOfShards, numberOfReplicas);
        }
    }
}
