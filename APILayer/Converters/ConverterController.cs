using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using System.Net.Mime;
using System.IO;
using Models;

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

        [HttpPost]
        public async Task<UploadMp4Response> UploadMP4Video([FromForm] IFormFile file)
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper();
            UploadMp4Response response = new UploadMp4Response();

            using (MemoryStream stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                bool objStorageResult = await _converterService.StoreFileAsync("videoBucket", guid, stream, file.ContentType);
            }

            response.Message = "test";
            response.ResponseCode = 200;

            return response;
        }
    }

    public class cenk{
        public IFormFile File { get; set; } 
    }
}
