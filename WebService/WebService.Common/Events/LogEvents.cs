using System.Net;

namespace WebService.Common.Events
{
    public static class LogEvents
    {
        public static readonly int FileUploadInternalServerError = (int)HttpStatusCode.InternalServerError;
        public static readonly int FileUploadBadRequest = (int)HttpStatusCode.BadRequest;
        public static readonly int ServiceConfigurationPhase = 1;
        public static readonly int UserNotFound = (int)HttpStatusCode.NotFound;
        public static readonly int UserCreated = (int)HttpStatusCode.Created;
        public static readonly int UserDeleted = (int)HttpStatusCode.NoContent;
        public static readonly int LogoutSuccessful = (int)HttpStatusCode.NoContent;
        public static readonly int Ok = (int)HttpStatusCode.OK;
        public static readonly int ServerError = (int)HttpStatusCode.InternalServerError;
        public static readonly int Unauthorized = (int)HttpStatusCode.Unauthorized;
        public static readonly int BadRequest = (int)HttpStatusCode.BadRequest;
        public static readonly int UserAlreadyExists = (int)HttpStatusCode.Conflict;
        public static readonly int RequestReceived = 11;
        public static readonly int RequestBodyValidation = 12;
        public static readonly int FileUploadingRequestReceived = 13;
        public static readonly int HealthCheckRequestReceived = 14;
        public static readonly int PutObject = 15;
        public static readonly int QueuePublishing = 16;
        public static readonly int FileUploadSuccess = (int)HttpStatusCode.OK;

        public static string FileUploadInternalServerErrorMessage { get; set; } = "Server Error";
        public static string FileUploadSuccessMessage { get; set; } = "File uploaded successfully";
        public static string FileUploadContentTypeMessage { get; set; } = "Content-Type should be multipart/form-data";
        public static string FileUploadContentDataMessage { get; set; } = "Request should contain form-data";
        public static string FileUploadFileExistsMessage { get; set; } = "File must be exists";
        public static string FileUploadFileFormatMessage { get; set; } = "File format must be mp4";
        public static string FileUploadFileSizeMessage { get; set; } = "File length must be less than {0} byte";
        public static string ServiceConfigurationPhaseMessage { get; set; } = "Service is fetching configurations";
        public static string RequestReceivedMessage { get; set; } = "Request is Received";
        public static string RequestBodyValidationMessage { get; set; } = "Request Body Validation is started";
        public static string FileUploadingRequestReceivedMessage { get; set; } = "File Upload request is received";
        public static string HealthCheckRequestReceivedMessage { get; set; } = "Health Check request is received";
        public static string PutObjectMessage { get; set; } = "PutObject Message";
    }
}