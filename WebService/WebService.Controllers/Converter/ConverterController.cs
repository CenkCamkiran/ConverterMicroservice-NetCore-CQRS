using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebService.Commands.ObjectCommands;
using WebService.Commands.QueueCommands;
using WebService.Common.Constants;
using WebService.Common.Events;
using WebService.Exceptions;
using WebService.Models;

namespace WebService.Controllers.Converter
{
    [Route("api/v1/main/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConverterController> _logger;

        public ConverterController(IMediator mediator, ILogger<ConverterController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<UploadMp4Response> UploadMP4Video([FromForm] IFormFile file, [FromForm] string email)
        {
            _logger.LogInformation(LogEvents.FileUploadingRequestReceived, LogEvents.FileUploadingRequestReceivedMessage);

            UploadMp4Response response = new UploadMp4Response();
            string guid = Guid.NewGuid().ToString();

            QueueMessage message = new QueueMessage()
            {
                email = email,
                fileGuid = guid
            };

            await _mediator.Send(new QueueCommand(ProjectConstants.ConverterServiceExchangeName, ProjectConstants.ConverterServiceRoutingKey, message, ProjectConstants.ConverterExchangeTtl));

            var stream = file.OpenReadStream();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);

                    await _mediator.Send(new ObjectCommand(ProjectConstants.MinioVideoBucket, guid, ms, file.ContentType));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.FileUploadInternalServerError, LogEvents.FileUploadInternalServerErrorMessage);

                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = LogEvents.FileUploadInternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            response.ResponseCode = LogEvents.FileUploadSuccess;
            response.Message = LogEvents.FileUploadSuccessMessage;

            return response;
        }
    }

}
