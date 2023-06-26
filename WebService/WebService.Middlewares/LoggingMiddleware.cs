using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text.RegularExpressions;
using WebService.Commands.LogCommands;
using WebService.Common.Constants;
using WebService.Middlewares.Contexts;
using WebService.Middlewares.Contexts.Interfaces;
using WebService.Models;

namespace WebService.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IMediator _mediator, IWebServiceContext webServiceContext)
        {

            try
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
                        await _mediator.Send(new LogCommand(ProjectConstants.RequestResponseLogsIndex, request, response, requestDate));
                }
            }
            catch (Exception exception)
            {
                var response = httpContext.Response;
                response.ContentType = MediaTypeNames.Application.Json;

                WebServiceErrors error = JsonConvert.DeserializeObject<WebServiceErrors>(exception.Message.ToString());
                response.StatusCode = error.ErrorCode;

                await response.WriteAsync(JsonConvert.SerializeObject(error));
            }
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
