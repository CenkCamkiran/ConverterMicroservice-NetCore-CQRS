using Microsoft.AspNetCore.Http;

namespace ServiceLayer.Interfaces
{
    public interface ILoggingService
    {
        Task<bool> LogFormData(string indexName, HttpRequest request, HttpResponse response);
        Task<bool> LogJsonBody(string indexName, HttpRequest request, HttpResponse response);
    }
}
