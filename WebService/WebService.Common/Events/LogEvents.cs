using System.Net;
using System.Net.Http;

namespace ChatAppStorageService.Common.Events
{
    public static class LogEvents
    {
        public static readonly int FileUploadInternalServerError = (int)HttpStatusCode.InternalServerError;
        public static readonly int FileUploadBadRequest = (int)HttpStatusCode.BadRequest;


        public static readonly int FileUploadSuccess = (int)HttpStatusCode.OK;
        public static readonly int RequestReceived = 1001;

        public static string FileUploadInternalServerErrorMessage { get; set; } = "Server Error";
        public static string FileUploadSuccessMessage { get; set; } = "File uploaded successfully";
        public static string FileUploadContentTypeMessage { get; set; } = "Content-Type should be multipart/form-data!";
        public static string FileUploadContentDataMessage { get; set; } = "Request should contain form-data";
        public static string FileUploadFileExistsMessage { get; set; } = "File must be exists!";
        public static string FileUploadFileFormatMessage { get; set; } = "File format must be mp4!";
        public static string FileUploadFileSizeMessage { get; set; } = "File length must be less than {0} byte!";
    }
}