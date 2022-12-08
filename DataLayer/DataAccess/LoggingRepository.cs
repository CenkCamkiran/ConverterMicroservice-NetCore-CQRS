using DataLayer.Interfaces;
using Helpers.ErrorHelper;
using Microsoft.AspNetCore.Http;
using Models;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataAccess
{
    public class LoggingRepository<TLogModel> : ILoggingRepository<TLogModel> where TLogModel : class
    {
        private readonly IElasticClient _elasticClient;

        public LoggingRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<bool> IndexReqResAsync(string indexName, TLogModel model)
        {
            try
            {
                await _elasticClient.IndexDocumentAsync(model);
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
