using Logger_Microservice.Common.Constants;
using Logger_Microservice.Common.Events;
using Logger_Microservice.Repositories.Interfaces;
using LoggerMicroservice.Models;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace Logger_Microservice.Repositories.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly Lazy<IQueueRepository> _queueErrorRepository;
        private ILogger<LogRepository> _logger;

        public LogRepository(IElasticClient elasticClient, Lazy<IQueueRepository> queueErrorRepository, ILogger<LogRepository> logger)
        {
            _elasticClient = elasticClient;
            _queueErrorRepository = queueErrorRepository;
            _logger = logger;
        }

        public async Task<bool> IndexDocAsync(string indexName, object model)
        {
            try
            {
                _logger.LogInformation(LogEvents.LogElkEvent, JsonConvert.SerializeObject(model));

                IndexResponse indexDocument = await _elasticClient.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                LoggerLog error = new LoggerLog();
                error.ErrorMessage = exception.Message.ToString();
                error.Date = DateTime.Now;
                ErrorLog errorLog = new ErrorLog()
                {
                    loggerLog = error
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);
                _logger.LogError(LogEvents.LogElkEvent, exception.Message.ToString());

                return false;
            }
        }
    }
}
