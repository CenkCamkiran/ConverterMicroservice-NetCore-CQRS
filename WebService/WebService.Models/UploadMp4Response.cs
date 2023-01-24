namespace WebService.Models
{
    public class UploadMp4Response : WebServiceErrors
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
