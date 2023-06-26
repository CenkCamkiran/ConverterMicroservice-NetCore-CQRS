using Initilization_Microservice.Common.Events;
using Initilization_Microservice.Helpers;
using Initilization_Microservice.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;

namespace Initilization_Microservice.Repository.Repositories
{
    public class ElkRepository<TModel> : IElkRepository<TModel> where TModel : class
    {

        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElkRepository<TModel>> _logger;

        public ElkRepository(IElasticClient elasticClient, ILogger<ElkRepository<TModel>> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<bool> ConfigureIndexAsync(string indexName, int numberOfShards, int numberOfReplicas)
        {
            try
            {
                if (!_elasticClient.Indices.Exists(indexName).Exists)
                {
                    var response = await _elasticClient.Indices
                                .CreateAsync(indexName, s => s
                                    .Settings(se => se
                                        .NumberOfReplicas(numberOfReplicas)
                                        .NumberOfShards(numberOfShards)
                                        ).Map<TModel>(
                                                    x => x.AutoMap().DateDetection(true)
                                                ));

                    if (!response.IsValid)
                        throw new JobInitializerException(response.ServerError.Error.ToString() + " " + response.ServerError.Status.ToString());

                    _logger.LogInformation(LogEvents.ElkIndexCreationPhase, LogEvents.ElkIndexCreationPhaseMessage);

                    return await Task.FromResult(response.IsValid);
                }
                else
                    return await Task.FromResult(_elasticClient.Indices.Exists(indexName).Exists);

            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.ElkIndexCreationPhaseError, exception.Message.ToString());
                throw new JobInitializerException(exception.Message.ToString());
            }
        }
    }
}
