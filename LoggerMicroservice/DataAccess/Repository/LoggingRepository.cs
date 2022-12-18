using DataAccess.Interfaces;
using Models;
using Nest;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class LoggingRepository<TModel> : ILoggingRepository<TModel> where TModel : class
    {
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IElasticClient _elasticClient;

        public LoggingRepository(ILog4NetRepository log4NetRepository, IElasticClient elasticClient)
        {
            _log4NetRepository = log4NetRepository;
            _elasticClient = elasticClient;
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
                _log4NetRepository.Error(exception.Message.ToString());

                return await Task.FromResult(true);
            }
        }

        public async Task LogOther(OtherLog log)
        {
            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogError(ErrorLog log)
        {
            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Error(logText);
            await Task.FromResult(true);
        }
    }
}
