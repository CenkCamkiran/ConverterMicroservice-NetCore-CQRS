namespace WebService.Exceptions
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
