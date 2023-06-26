using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using WebService.Common.Events;
using WebService.Exceptions;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
{
    public class LogRepository<TModel> : ILogRepository<TModel> where TModel : class
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<LogRepository<TModel>> _logger;

        public LogRepository(IElasticClient elasticClient, ILogger<LogRepository<TModel>> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<bool> IndexDocAsync(string indexName, TModel model)
        {
            try
            {
                _logger.LogInformation(LogEvents.RequestReceived, JsonConvert.SerializeObject(model));
                IndexResponse indexDocument = await _elasticClient.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = LogEvents.FileUploadInternalServerError;

                _logger.LogError(error.ErrorCode, exception.Message.ToString());

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }
    }
}
