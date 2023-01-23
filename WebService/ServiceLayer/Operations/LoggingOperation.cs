﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using WebService.DataAccessLayer.Interfaces;
using WebService.Models;
using WebService.OperationLayer.Interfaces;

namespace WebService.OperationLayer.Operations
{
    public class LoggingOperation : ILoggingOperation
    {

        private ILoggingRepository<RequestResponseLog> _loggingRepository;

        public LoggingOperation(ILoggingRepository<RequestResponseLog> loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task<bool> LogFormDataAsync(string indexName, HttpRequest request, HttpResponse response, DateTime requestDate)
        {
            response.Body.Position = 0;

            IFormCollection formData = await request.ReadFormAsync();
            IFormFile? file = formData.Files.GetFile("file");

            StreamReader responseStream = new StreamReader(response.Body);
            string JSONResponseBody = await responseStream.ReadToEndAsync();

            UploadMp4Response? responseObj = JsonConvert.DeserializeObject<UploadMp4Response>(JSONResponseBody);

            RequestResponseLog model = new RequestResponseLog()
            {
                requestDate = requestDate,
                requestContentType = request.ContentType,
                requestFileDetails = new FileDetails()
                {
                    CreatedDate = DateTime.Now,
                    Length = file != null ? file.Length.ToString() : string.Empty,
                    Name = file != null ? file.FileName : string.Empty,
                },
                responseContentType = response.ContentType,
                responseDate = DateTime.Now,
                responseMessage = response.StatusCode == (int)HttpStatusCode.OK ? responseObj.Message : responseObj.ErrorMessage,
                responseStatusCode = (short)response.StatusCode
            };

            return await _loggingRepository.IndexDocAsync(indexName, model);

        }

        //public async Task<bool> LogJsonBodyAsync(string indexName, HttpRequest request, HttpResponse response)
        //{
        //    Stream originalResponseBody = response.Body;
        //    Stream originalRequestBody = request.Body;

        //    try
        //    {
        //        using (MemoryStream memStream = new MemoryStream())
        //        {
        //            response.Body = memStream;
        //            memStream.Position = 0;
        //            await memStream.CopyToAsync(originalResponseBody);

        //            await memStream.FlushAsync();

        //            request.Body = memStream;
        //            memStream.Position = 0;
        //            await memStream.CopyToAsync(originalRequestBody);

        //            request.Body.Position = 0;
        //            response.Body.Position = 0;

        //            StreamReader requestStream = new StreamReader(request.Body);
        //            StreamReader responseStream = new StreamReader(response.Body);

        //            var JSONRequestBody = await requestStream.ReadToEndAsync();
        //            var JSONResponseBody = await responseStream.ReadToEndAsync();

        //            RequestResponseLogModel model = new RequestResponseLogModel()
        //            {
        //                requestDate = DateTime.Now,
        //                requestContentType = request.ContentType,
        //                requestFileDetails = new FileDetails()
        //                {
        //                    CreatedDate = DateTime.Now,
        //                    Length = "0",
        //                    Name = ""
        //                },
        //                responseContentType = response.ContentType,
        //                responseDate = DateTime.Now,
        //                responseMessage = "",
        //                responseStatusCode = (short)response.StatusCode
        //            };

        //            return await _loggingRepository.IndexReqResAsync(indexName, model);

        //        }
        //    }
        //    finally
        //    {
        //        response.Body = originalResponseBody;
        //        request.Body = originalRequestBody;
        //    }
        //}
    }
}