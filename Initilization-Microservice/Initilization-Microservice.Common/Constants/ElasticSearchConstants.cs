namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string ElkHost { get; set; } = Environment.GetEnvironmentVariable("ELK_HOST");
        public static string ElkDefaultIndexName { get; set; } = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX_NAME");
        public static string ElkUsername { get; set; } = Environment.GetEnvironmentVariable("ELK_USERNAME");
        public static string ElkPassword { get; set; } = Environment.GetEnvironmentVariable("ELK_PASSWORD");
        public static double ElkRequestTimeout { get; set; } = 300;
        public static bool ElkExceptions { get; set; } = true;

        public static string LoggerServiceErrorLogsIndex { get; set; } = "loggerservice_errorlogs";
        public static string LoggerServiceOtherLogsIndex { get; set; } = "loggerservice_otherlogs";
        public static string WebServiceObjectStorageLogs { get; set; } = "webservice_objstorage_logs";
        public static string WebServiceRequestResponseLogs { get; set; } = "webservice_requestresponse_logs";
        public static string WebServiceQueueLogs { get; set; } = "webservice_queue_logs";


        public static int LoggerServiceErrorLogsNumberOfShards { get; set; } = 1;
        public static int LoggerServiceOtherLogsNumberOfShards { get; set; } = 1;
        public static int WebServiceObjectStorageNumberOfShards { get; set; } = 1;
        public static int WebServiceRequestResponseNumberOfShards { get; set; } = 1;
        public static int WebServiceQueueNumberOfShards { get; set; } = 1;

        public static int LoggerServiceErrorLogsNumberOfReplicas { get; set; } = 0;
        public static int LoggerServiceOtherLogsNumberOfReplicas { get; set; } = 0;
        public static int WebServiceObjectStorageNumberOfReplicas { get; set; } = 0;
        public static int WebServiceRequestResponseNumberOfReplicas { get; set; } = 0;
        public static int WebServiceQueueNumberOfReplicas { get; set; } = 0;


    }
}
