using DataAccess.Interfaces;
using Models;
using Nest;
using System;
using System.Threading.Tasks;

namespace DataAccess.Repository
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
                IndexResponse indexDocument = await _elasticClient.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";
                _log4NetRepository.Info(elkResponse);

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                ConsumerExceptionModel error = new ConsumerExceptionModel();
                error.ErrorMessage = exception.Message.ToString();
                ErrorLog errorLog = new ErrorLog()
                {
                    exceptionModel = error
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                _log4NetRepository.Error(exception.Message.ToString());

                return false;
            }
        }
    }
}
