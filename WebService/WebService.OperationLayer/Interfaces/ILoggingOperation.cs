using Microsoft.AspNetCore.Http;

namespace WebService.OperationLayer.Interfaces
{
    public interface ILoggingOperation
    {
        Task<bool> LogFormDataAsync(string indexName, HttpRequest request, HttpResponse response, DateTime requestDate);
        //Task<bool> LogJsonBodyAsync(string indexName, HttpRequest request, HttpResponse response);
    }
}
