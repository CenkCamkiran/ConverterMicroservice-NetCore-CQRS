namespace Initilization_Microservice.Models
{
    public class ServiceError
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorDescription { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public string StackName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
