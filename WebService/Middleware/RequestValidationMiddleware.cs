using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using System.Net;

namespace Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;

            if (!request.HasFormContentType)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = "Content-Type should be multipart/form-data!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            IFormCollection formData = await httpContext.Request.ReadFormAsync();

            if (!formData.Files.Any())
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = "Request should contain form-data!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            IFormFile file = formData.Files.GetFile("file");

            if (file.Length == 0)
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = "File must be exists!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            if (file.ContentType != "video/mp4")
            {
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = "File format must be mp4!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            string emailFormData = httpContext.Request.Form["email"].ToString();
            EmailFormatHelper helper = new EmailFormatHelper();
            helper.ValidateEMail(emailFormData);

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}
