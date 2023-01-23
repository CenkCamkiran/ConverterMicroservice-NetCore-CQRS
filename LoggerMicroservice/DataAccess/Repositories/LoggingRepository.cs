using LoggerMicroservice.DataAccessLayer.Interfaces;
using LoggerMicroservice.Models;
using Nest;
using System;
using System.Threading.Tasks;

namespace LoggerMicroservice.DataAccessLayer.Repositories
{
    public class LoggingRepository<TModel> : ILoggingRepository<TModel> where TModel : class
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly Lazy<IQueueRepository<ErrorLog>> _queueErrorRepository;

        public LoggingRepository(IElasticClient elasticClient, ILog4NetRepository log4NetRepository, Lazy<IQueueRepository<ErrorLog>> queueErrorRepository)
        {
            _elasticClient = elasticClient;
            _log4NetRepository = log4NetRepository;
            _queueErrorRepository = queueErrorRepository;
        }

        public async Task<bool> IndexDocAsync(string indexName, TModel model)
        {
            try
            {
                //IndexResponse indexDocument = await _elasticClient.IndexDocumentAsync(model);
                //BulkResponse bulkDocuments = await _elasticClient.BulkAsync(elk => elk.Index(indexName).IndexMany(model));
                IndexResponse indexDocument = await _elasticClient.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";
                _log4NetRepository.Info(elkResponse);

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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                _log4NetRepository.Error(exception.Message.ToString());

                return false;
            }
        }
    }
}
