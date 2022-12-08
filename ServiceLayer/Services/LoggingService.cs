using DataLayer.DataAccess;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Models;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class LoggingService: ILoggingService
    {

        private ILoggingRepository _loggingRepository;

        public LoggingService(ILoggingRepository loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task<bool> Log(string indexName, HttpRequest request, HttpResponse response)
        {
            Stream originalResponseBody = response.Body;
            Stream originalRequestBody = response.Body;

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    response.Body = memStream;
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalResponseBody);

                    memStream.FlushAsync();

                    request.Body = memStream;
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalRequestBody);

                    request.Body.Position = 0;
                    response.Body.Position = 0;

                    StreamReader requestStream = new StreamReader(request.Body);
                    StreamReader responseStream = new StreamReader(response.Body);

                    var JSONRequestBody = await requestStream.ReadToEndAsync();
                    var JSONResponseBody = await responseStream.ReadToEndAsync();

                    RequestResponseLogModel model = new RequestResponseLogModel()
                    {
                        requestDate = DateTime.Now,
                        requestContentType = request.ContentType,
                        requestFileDetails = new FileDetails()
                        {
                            CreatedDate = DateTime.Now,
                            Length = "0",
                            Name = ""
                        },
                        responseContentType = response.ContentType,
                        responseDate = DateTime.Now,
                        responseMessage = "",
                        responseStatusCode = (short)response.StatusCode
                    };

                    return await _loggingRepository.IndexReqResAsync(indexName, model);

                }
            }
            finally
            {
                response.Body = originalResponseBody;
                request.Body = originalRequestBody; 
            }

        }
    }
}
