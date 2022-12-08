using DataLayer.DataAccess;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Models;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class LoggingService<TLogModel> : ILoggingService where TLogModel : class
    {
        private ILoggingRepository<TLogModel> _loggingRepository;

        public LoggingService(ILoggingRepository<TLogModel> loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task<bool> Log(string indexName, HttpRequest request, HttpResponse response)
        {
            RequestResponseLogModel model = new RequestResponseLogModel()
            {
                requestDate = DateTime.Now,
                requestContentType = request.ContentType,
                requestFileDetails = null,
                responseContentType = response.ContentType,
                responseDate = DateTime.Now,
                responseMessage = null,
                responseStatusCode = (short)response.StatusCode
            };

            return await _loggingRepository<RequestResponseLogModel>.IndexReqResAsync(indexName, model);
        }
    }
}
