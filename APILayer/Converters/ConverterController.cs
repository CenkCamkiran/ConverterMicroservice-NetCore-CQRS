using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using System.IO;
using System.Net;

namespace APILayer.Converters
{
    [Route("api/v1/main/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private IConverterService _converterService;

        public ConverterController(IConverterService converterService)
        {
            _converterService = converterService;
        }

        //    [HttpPost]
        //    public async Task<OkResult> UploadMP4Video([FromForm] IFormFile file, [FromForm] string email)
        //    {
        //        string guid = Guid.NewGuid().ToString("N").ToUpper();

        //        QueueMessage message = new QueueMessage()
        //        {
        //            email = email,
        //            fileGuid = guid
        //        };

        //        _converterService.QueueMessageDirect(message, "converter", "converter_exchange.direct", "mp4_to_mp3");

        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);

        //            await _converterService.StoreFileAsync("videos", guid, stream, file.ContentType);
        //        }

        //        return Ok();
        //    }
        //}

        [HttpPost]
        public async Task<UploadMp4Response> UploadMP4Video([FromForm] IFormFile file, [FromForm] string email)
        {
            UploadMp4Response response = new UploadMp4Response();
            string guid = Guid.NewGuid().ToString("N").ToUpper();

            QueueMessage message = new QueueMessage()
            {
                email = email,
                fileGuid = guid
            };

            _converterService.QueueMessageDirect(message, "converter", "converter_exchange.direct", "mp4_to_mp3");

            //try
            //{
            //    byte[]? fileFromBytes = System.IO.File.ReadAllBytes(file.FileName); //pass fileFromBytes to MemoryStream
            //}
            //catch (Exception)
            //{
            //}

            //Stream? stream = null;
            //try
            //{
            //    using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\VideoFiles", FileMode.Create))
            //    {
            //        stream = file.OpenReadStream();
            //        await stream.CopyToAsync(fs);

            //        await _converterService.StoreFileAsync("videos", guid, fs, file.ContentType);
            //    }
            //}
            //catch (Exception exception)
            //{

            //    WebServiceErrors error = new WebServiceErrors();
            //    error.ErrorMessage = exception.Message.ToString();
            //    error.ErrorCode = (int)HttpStatusCode.InternalServerError;

            //    throw new WebServiceException(JsonConvert.SerializeObject(error));
            //}

            response.ResponseCode = (int)HttpStatusCode.OK;
            response.Message = "success";

            return response;
        }
    }

}
