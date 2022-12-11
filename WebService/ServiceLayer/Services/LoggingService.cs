using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using System.Net.Http;

namespace ServiceLayer.Services
{
    public class LoggingService : ILoggingService
    {

        private ILoggingRepository _loggingRepository;

        public LoggingService(ILoggingRepository loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task<bool> LogFormDataAsync(string indexName, HttpRequest request, HttpResponse response)
        {
            response.Body.Position = 0;

            IFormCollection formData = await request.ReadFormAsync();
            IFormFile file = formData.Files.GetFile("file");

            StreamReader responseStream = new StreamReader(response.Body);
            string JSONResponseBody = await responseStream.ReadToEndAsync();
            UploadMp4Response? responseObj = JsonConvert.DeserializeObject<UploadMp4Response>(JSONResponseBody);

            RequestResponseLogModel model = new RequestResponseLogModel()
            {
                requestDate = DateTime.Now,
                requestContentType = request.ContentType,
                requestFileDetails = new FileDetails()
                {
                    CreatedDate = DateTime.Now,
                    Length = file.Length.ToString(),
                    Name = file.FileName
                },
                responseContentType = response.ContentType,
                responseDate = DateTime.Now,
                responseMessage = responseObj.Message,
                responseStatusCode = (short)responseObj.ResponseCode
            };

            return await _loggingRepository.IndexReqResAsync(indexName, model);

        }

        public async Task<bool> LogJsonBodyAsync(string indexName, HttpRequest request, HttpResponse response)
        {
            Stream originalResponseBody = response.Body;
            Stream originalRequestBody = request.Body;

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    response.Body = memStream;
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalResponseBody);

                    await memStream.FlushAsync();

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
