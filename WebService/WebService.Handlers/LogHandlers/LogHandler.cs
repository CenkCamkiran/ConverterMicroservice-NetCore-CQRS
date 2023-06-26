using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using WebService.Commands.LogCommands;
using WebService.Common.Events;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Handlers.LogHandlers
{
    public class LogHandler : IRequestHandler<LogCommand, bool>
    {
        private readonly ILogRepository<RequestResponseLog> _logRepository;

        public LogHandler(ILogRepository<RequestResponseLog> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<bool> Handle(LogCommand request, CancellationToken cancellationToken)
        {
            request.Response.Body.Position = 0;

            IFormCollection formData = await request.Request.ReadFormAsync();
            IFormFile? file = formData.Files.GetFile("file");

            StreamReader responseStream = new StreamReader(request.Response.Body);
            string JSONResponseBody = await responseStream.ReadToEndAsync();

            UploadMp4Response? responseObj = JsonConvert.DeserializeObject<UploadMp4Response>(JSONResponseBody);

            RequestResponseLog model = new RequestResponseLog()
            {
                requestDate = request.RequestDate,
                requestContentType = request.Request.ContentType,
                requestFileDetails = new FileDetails()
                {
                    CreatedDate = DateTime.Now,
                    Length = file != null ? file.Length.ToString() : string.Empty,
                    Name = file != null ? file.FileName : string.Empty,
                },
                responseContentType = request.Response.ContentType,
                responseDate = DateTime.Now,
                responseMessage = request.Response.StatusCode == (int)HttpStatusCode.OK ? responseObj.Message : responseObj.ErrorMessage,
                responseStatusCode = (short)request.Response.StatusCode
            };

            return await _logRepository.IndexDocAsync(request.IndexName, model);
        }
    }
}
