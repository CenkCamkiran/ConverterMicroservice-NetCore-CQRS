using ChatAppStorageService.Common.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using WebService.Common.Constants;
using WebService.Exceptions;
using WebService.Helpers.Helpers;
using WebService.Models;

namespace WebService.Middlewares
{
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
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = LogEvents.FileUploadContentTypeMessage;
                error.ErrorCode = LogEvents.FileUploadBadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            IFormCollection formData = await httpContext.Request.ReadFormAsync();

            if (!formData.Files.Any())
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = LogEvents.FileUploadContentDataMessage;
                error.ErrorCode = LogEvents.FileUploadBadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            IFormFile file = formData.Files.GetFile("file");

            if (file.Length == 0)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = LogEvents.FileUploadFileExistsMessage;
                error.ErrorCode = LogEvents.FileUploadBadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            if (file.ContentType != "video/mp4")
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = LogEvents.FileUploadFileFormatMessage;
                error.ErrorCode = LogEvents.FileUploadBadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            long fileLength = Convert.ToInt64(ProjectConstants.FileUploadSizeLinit);
            if (file.Length == fileLength)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = string.Format(LogEvents.FileUploadFileSizeMessage, fileLength);
                error.ErrorCode = LogEvents.FileUploadBadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }

            string emailFormData = httpContext.Request.Form["email"].ToString();
            EmailFormatHelper helper = new EmailFormatHelper();
            helper.ValidateEMail(emailFormData);

            await _next(httpContext);
        }
    }

    public static class RequestValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}
