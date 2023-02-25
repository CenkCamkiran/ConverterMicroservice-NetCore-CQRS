using WebService.Middlewares.Contexts.Interfaces;

namespace WebService.Middlewares.Contexts
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
