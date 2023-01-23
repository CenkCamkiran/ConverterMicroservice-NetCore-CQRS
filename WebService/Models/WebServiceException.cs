namespace WebService.Models
{
    public class WebServiceException : Exception
    {
        public WebServiceException()
        {

        }

        public WebServiceException(string? message) : base(message)
        {

        }
    }
}
