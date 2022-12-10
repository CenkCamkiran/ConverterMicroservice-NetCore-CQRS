using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                var response = httpContext.Response;
                response.ContentType = MediaTypeNames.Application.Json;

                WebServiceErrors error = JsonConvert.DeserializeObject<WebServiceErrors>(exception.Message.ToString());
                response.StatusCode = error.ErrorCode;

                //HttpContext.Response.Headers.Add(HttpResponseHeader.ContentType.ToString(), MediaTypeNames.Application.Json);
                //HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

                await response.WriteAsync(JsonConvert.SerializeObject(error));
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
