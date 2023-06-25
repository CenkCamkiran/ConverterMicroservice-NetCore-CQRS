namespace Logger_Microservice.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string ElkHost { get; set; } = Environment.GetEnvironmentVariable("ELK_HOST");
        public static string ElkDefaultIndexName { get; set; } = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX");
        public static string ElkUsername { get; set; } = Environment.GetEnvironmentVariable("ELK_USERNAME");
        public static string ElkPassword { get; set; } = Environment.GetEnvironmentVariable("ELK_PASSWORD");
        public static double ElkRequestTimeout { get; set; } = 300;
        public static bool ElkExceptions { get; set; } = true;
        public static string LoggerServiceErrorLogsIndex { get; set; } = "loggerservice_errorlogs";
        public static string LoggerServiceOtherLogsIndex { get; set; } = "loggerservice_otherlogs";
    }
}
