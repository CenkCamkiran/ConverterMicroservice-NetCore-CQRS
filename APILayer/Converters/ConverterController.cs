using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ConvertersModels;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using System.Net.Mime;

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
            File().Create();

            bool objStorageResult = await _converterService.StoreFileAsync("", "", guid, "", "", "");
            //bool queueResult = await _converterService.QueueMessageDirectAsync("", "", "", "");

            UploadMp4Response response = new UploadMp4Response();
            response.Message = "test";
            response.ResponseCode = 200;

            return response;
        }
    }

    public class cenk{
        public IFormFile File { get; set; } 
    }
}
