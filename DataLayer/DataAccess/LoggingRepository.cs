using DataLayer.Interfaces;
using Helpers;
using Models;
using Nest;
using Newtonsoft.Json;
using System.Net;

namespace DataLayer.DataAccess
{
    public class LoggingRepository : ILoggingRepository
    {
        private readonly IElasticClient _elasticClient;

        public LoggingRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<bool> IndexReqResAsync(string indexName, RequestResponseLogModel model)
        {
            try
            {
                IndexResponse indexDocument = await _elasticClient.IndexDocumentAsync(model);
                Console.WriteLine("Document Id: " + indexDocument.Id);
                Console.WriteLine("Index: " + indexDocument.Index);
                Console.WriteLine("Result: " + indexDocument.Result);
                Console.WriteLine("IsValid: " + indexDocument.IsValid);
                Console.WriteLine("ServerError: " + indexDocument.ServerError);
                Console.WriteLine("ApiCall.HttpStatusCode: " + indexDocument.ApiCall.HttpStatusCode);
                Console.WriteLine("ApiCall.OriginalException: " + indexDocument.ApiCall.OriginalException);
                Console.WriteLine("ApiCall.Success: " + indexDocument.ApiCall.Success);

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }

        public async Task<bool> IndexExceptionAsync(string indexName, ExceptionLogModel model)
        {
            try
            {
                IndexResponse indexDocument = await _elasticClient.IndexDocumentAsync(model);
                Console.WriteLine("Document Id: " + indexDocument.Id);
                Console.WriteLine("Index: " + indexDocument.Index);
                Console.WriteLine("Result: " + indexDocument.Result);
                Console.WriteLine("IsValid: " + indexDocument.IsValid);
                Console.WriteLine("ServerError: " + indexDocument.ServerError);
                Console.WriteLine("ApiCall.HttpStatusCode: " + indexDocument.ApiCall.HttpStatusCode);
                Console.WriteLine("ApiCall.OriginalException: " + indexDocument.ApiCall.OriginalException);
                Console.WriteLine("ApiCall.Success: " + indexDocument.ApiCall.Success);

                return indexDocument.IsValid;

            }
            catch (Exception exception)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }

    }
}
