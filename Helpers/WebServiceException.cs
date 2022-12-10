namespace Helpers
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
