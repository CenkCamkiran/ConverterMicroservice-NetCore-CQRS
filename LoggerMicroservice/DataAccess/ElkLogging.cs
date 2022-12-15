using Configuration;
using Elasticsearch.Net;
using Models;
using Nest;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ElkLogging<TModel> where TModel : class
    {
        private Logger log = new Logger();

        public ElasticClient ConnectELK()
        {
            EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
            ElkConfiguration elkConfiguration = envVariablesHandler.GetElkEnvVariables();

            ConnectionSettings? connection = new ConnectionSettings(new Uri(elkConfiguration.ElkHost)).
            DefaultIndex(elkConfiguration.ElkDefaultIndex).
            ServerCertificateValidationCallback(CertificateValidations.AllowAll).
            ThrowExceptions(true).
            PrettyJson().
            RequestTimeout(TimeSpan.FromSeconds(300)).
            BasicAuthentication(elkConfiguration.ElkUsername, elkConfiguration.ElkPassword); //.ApiKeyAuthentication("<id>", "<api key>"); 

            ElasticClient? elasticClient = new ElasticClient(connection);

            return elasticClient;
        }

        public async Task<bool> IndexExceptionAsync(string indexName, TModel model)
        {

            try
            {
                ElasticClient client = ConnectELK();
                IndexResponse indexDocument = await client.IndexAsync(model, elk => elk.Index(indexName));

                string elkResponse = $"Doc ID: {indexDocument.Id} - Index: {indexDocument.Index} - Result: {indexDocument.Result} - Is Valid: {indexDocument.IsValid} - ApiCall.HttpStatusCode: {indexDocument.ApiCall.HttpStatusCode} - ApiCall.Success: {indexDocument.ApiCall.Success}";
                log.Info(elkResponse);

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                ConsumerExceptionModel error = new ConsumerExceptionModel();
                error.ErrorMessage = exception.Message.ToString();

                log.Error(exception.Message.ToString());

                throw new ConsumerException(JsonConvert.SerializeObject(error));
            }
        }

        //public async Task<bool> IndexStatusesAsync(string indexName, object model)
        //{
        //    try
        //    {
        //        IndexResponse indexDocument = await _elasticClient.IndexDocumentAsync(model);
        //        Console.WriteLine("Document Id: " + indexDocument.Id);
        //        Console.WriteLine("Index: " + indexDocument.Index);
        //        Console.WriteLine("Result: " + indexDocument.Result);
        //        Console.WriteLine("IsValid: " + indexDocument.IsValid);
        //        Console.WriteLine("ServerError: " + indexDocument.ServerError);
        //        Console.WriteLine("ApiCall.HttpStatusCode: " + indexDocument.ApiCall.HttpStatusCode);
        //        Console.WriteLine("ApiCall.OriginalException: " + indexDocument.ApiCall.OriginalException);
        //        Console.WriteLine("ApiCall.Success: " + indexDocument.ApiCall.Success);

        //        return indexDocument.IsValid;

        //    }
        //    catch (Exception exception)
        //    {
        //        ConsumerExceptionModel error = new ConsumerExceptionModel();
        //        error.ErrorMessage = exception.Message.ToString();


        //    }
        //}
    }
}
