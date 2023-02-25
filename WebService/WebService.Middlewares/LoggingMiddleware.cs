using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using WebService.Commands.LogCommands;
using WebService.Middlewares.Contexts.Interfaces;

namespace WebService.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IMediator _mediator, IWebServiceContext webServiceContext)
        {
            DateTime requestDate = webServiceContext.GetRequestDateContext();
            await _next(httpContext);

            HttpRequest request = httpContext.Request;
            HttpResponse response = httpContext.Response;

            Regex formRegex = new Regex("form-data");
            Regex jsonRegex = new Regex("json");

            if (!string.IsNullOrEmpty(request.ContentType))
            {
                if (formRegex.Match(request.ContentType).Success)
                    await _mediator.Send(new LogCommand("webservice_requestresponse_logs", request, response, requestDate));

                //if (jsonRegex.Match(request.ContentType).Success)
                //    await loggingService.LogJsonBodyAsync("webservice_requestresponse_logs", request, response);
            }

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
