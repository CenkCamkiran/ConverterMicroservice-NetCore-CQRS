using Configuration;
using DataAccess.Interfaces;
using Elasticsearch.Net;
using Models;
using Nest;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class LoggingRepository<TModel>: ILoggingRepository<TModel> where TModel : class
    {
        private Log4NetRepository log = new Log4NetRepository();

        private ElasticClient ConnectELK()
        {
            EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
            ElkConfiguration elkConfiguration = envVariablesHandler.GetElkEnvVariables();

            ConnectionSettings connection = new ConnectionSettings(new Uri(elkConfiguration.ElkHost)).
            DefaultIndex(elkConfiguration.ElkDefaultIndex).
            ServerCertificateValidationCallback(CertificateValidations.AllowAll).
            ThrowExceptions(true).
            PrettyJson().
            RequestTimeout(TimeSpan.FromSeconds(300)).
            BasicAuthentication(elkConfiguration.ElkUsername, elkConfiguration.ElkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 

            ElasticClient elasticClient = new ElasticClient(connection);

            return elasticClient;
        }

        public async Task<bool> IndexDocAsync(string indexName, TModel model)
        {
            try
            {

                ElasticClient elasticClient = ConnectELK();
                //IndexResponse indexDocument = await _elasticClient.IndexDocumentAsync(model);
                IndexResponse indexDocument = await elasticClient.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";
                log.Info(elkResponse);

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                log.Error(exception.Message.ToString());

                return await Task.FromResult(true);
            }
        }
    }
}
