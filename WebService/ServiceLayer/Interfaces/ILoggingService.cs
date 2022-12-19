using Microsoft.AspNetCore.Http;

namespace ServiceLayer.Interfaces
{
    public interface ILoggingService
    {
        Task<bool> LogFormDataAsync(string indexName, HttpRequest request, HttpResponse response, DateTime requestDate);
        //Task<bool> LogJsonBodyAsync(string indexName, HttpRequest request, HttpResponse response);
    }
}
