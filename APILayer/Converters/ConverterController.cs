using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ConvertersModels;
using ServiceLayer.Interfaces;
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
        //[Consumes(MediaTypeNames.)]
        public UploadMp4Response UploadMP4Video([FromForm] IFormFile file)
        {
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
