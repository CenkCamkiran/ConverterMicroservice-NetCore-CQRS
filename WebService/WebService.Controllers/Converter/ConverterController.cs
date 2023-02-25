﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using WebService.Commands.ObjectCommands;
using WebService.Commands.QueueCommands;
using WebService.Exceptions;
using WebService.Models;

namespace WebService.Controllers.Converter
{
    [Route("api/v1/main/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConverterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<UploadMp4Response> UploadMP4Video([FromForm] IFormFile file, [FromForm] string email)
        {
            UploadMp4Response response = new UploadMp4Response();
            string guid = Guid.NewGuid().ToString();

            QueueMessage message = new QueueMessage()
            {
                email = email,
                fileGuid = guid
            };

            await _mediator.Send(new QueueCommand("converter", "converter_exchange.direct", "mp4_to_mp3", message, 43200000));

            var stream = file.OpenReadStream();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);

                    await _mediator.Send(new ObjectCommand("videos", guid, ms, file.ContentType));
                }
            }
            catch (Exception exception)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = exception.Message.ToString();
                error.ErrorCode = (int)HttpStatusCode.InternalServerError;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            response.ResponseCode = (int)HttpStatusCode.OK;
            response.Message = "File uploaded!";

            return response;
        }
    }

}