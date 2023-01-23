using WebService.MiddlewareLayer.Contexts.Interfaces;

namespace WebService.MiddlewareLayer.Contexts
{
    public class WebServiceContext : IWebServiceContext
    {
        public DateTime RequestDate { get; set; }

        public WebServiceContext()
        {
            RequestDate = DateTime.Now;
        }

        public DateTime GetRequestDateContext()
        {
            return RequestDate;
        }

    }
}
