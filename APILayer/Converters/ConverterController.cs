using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ConvertersModels;

namespace APILayer.Converters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {

        [HttpPost]
        public UploadMp4Response UploadMP4Video()
        {
            UploadMp4Response response = new UploadMp4Response();
            response.Message = "test";
            response.ResponseCode = 200;

            return response;
        }
    }
}
