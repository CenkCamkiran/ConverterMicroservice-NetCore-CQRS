namespace Models
{
    public class RequestResponseLogModel
    {
        public DateTime requestDate { get; set; }
        public string requestContentType { get; set; } = String.Empty;
        public FileDetails? requestFileDetails { get; set; }
        public string responseContentType { get; set; } = String.Empty;
        public DateTime responseDate { get; set; }
        public string responseMessage { get; set; } = String.Empty;
        public short responseStatusCode { get; set; }

    }

    public class FileDetails
    {
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Length { get; set; }
    }
}
