using DataLayer.Interfaces;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Net;

namespace APILayer.Converters
{
    [Route("api/v1/main/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private IQueueRepository _queueRepository;
        private IMinioStorageRepository _minioStorageRepository;

        public ConverterController(IQueueRepository queueRepository, IMinioStorageRepository minioStorageRepository)
        {
            _queueRepository = queueRepository;
            _minioStorageRepository = minioStorageRepository;
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

            await _queueRepository.QueueMessageDirectAsync(message, "converter", "converter_exchange.direct", "mp4_to_mp3");

            var stream = file.OpenReadStream();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);

                    await _minioStorageRepository.StoreFileAsync("videos", guid, ms, file.ContentType);
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
