using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ServiceLayer.Interfaces;

namespace Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILoggingService loggingService)
        {
            await _next(httpContext);

            HttpRequest request = httpContext.Request;
            HttpResponse response = httpContext.Response;

            await loggingService.LogFormDataAsync("webservice_requestresponse_logs", request, response);
            //if (request.ContentType == "multipart/form-data")
            //    await loggingService.LogFormDataAsync("webservice_requestresponse_logs", request, response);

            if (request.ContentType == "application/json")
                await loggingService.LogJsonBodyAsync("webservice_requestresponse_logs", request, response);

            //Fonksiyonu tanımlayıp burada çağır. Params: HttpRequest request, HttpResponse response
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
