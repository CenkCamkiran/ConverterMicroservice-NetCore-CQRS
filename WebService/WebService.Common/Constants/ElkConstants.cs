namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string ElkHost { get; set; } = Environment.GetEnvironmentVariable("ELK_HOST");
        public static string ElkDefaultIndexName { get; set; } = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX");
        public static string ElkUsername { get; set; } = Environment.GetEnvironmentVariable("ELK_USERNAME");
        public static string ElkPassword { get; set; } = Environment.GetEnvironmentVariable("ELK_PASSWORD");
        public static double ElkRequestTimeout { get; set; } = 300;
        public static bool ElkExceptions { get; set; } = true;
        public static string RequestResponseLogsIndex { get; set; } = "webservice_requestresponse_logs";
        public static string QueueLogsIndex { get; set; } = "webservice_queue_logs";
        public static string ObjectStorageLogsIndex { get; set; } = "webservice_objstorage_logs";
        public static string WebServiceErrorLogs { get; set; } = "webservice_error_logs";
    }
}
