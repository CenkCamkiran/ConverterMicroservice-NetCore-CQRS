namespace Models
{
    public class RequestResponseLogModel
    {
        public DateTime requestDate { get; set; }
        public string requestContentType { get; set; } = String.Empty;
        public FileDetails? requestFileDetails { get; set; }
        public string responseContentType { get; set; } = String.Empty;
        public DateTime responseDate { get; set; } = DateTime.Now;
        public string responseMessage { get; set; } = String.Empty;
        public short responseStatusCode { get; set; } = default(short);

    }

    public class FileDetails
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Name { get; set; } = string.Empty;
        public string Length { get; set; } = string.Empty;
    }
}
