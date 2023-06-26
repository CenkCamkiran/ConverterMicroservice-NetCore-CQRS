namespace Initilization_Microservice.Models
{
    public class RequestResponseLog
    {
        public DateTime requestDate { get; set; }
        public string requestContentType { get; set; } = string.Empty;
        public FileDetails? requestFileDetails { get; set; }
        public string responseContentType { get; set; } = string.Empty;
        public DateTime responseDate { get; set; } = DateTime.Now;
        public string responseMessage { get; set; } = string.Empty;
        public short responseStatusCode { get; set; } = default;

    }

    public class FileDetails
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Name { get; set; } = string.Empty;
        public string Length { get; set; } = string.Empty;
    }
}
