namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string ElkHost { get; set; } = Environment.GetEnvironmentVariable("ELK_HOST");
        public static string ElkDefaultIndexName { get; set; } = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX_NAME");
        public static string ElkUsername { get; set; } = Environment.GetEnvironmentVariable("ELK_USERNAME");
        public static string ElkPassword { get; set; } = Environment.GetEnvironmentVariable("ELK_PASSWORD");
        public static double ElkRequestTimeout { get; set; } = Convert.ToDouble(Environment.GetEnvironmentVariable("ELK_REQUEST_TIMEOUT"));
        public static bool ElkExceptions { get; set; } = true;
        public static string ElkUserServiceIndexName { get; set; } = "user_service";
        public static string ElkStorageServiceIndexName { get; set; } = "storage_service";
        public static string ElkChatBrokerServiceIndexName { get; set; } = "broker_service";
        public static int UserServiceNumberOfShards { get; set; } = 1;
        public static int StorageServiceNumberOfShards { get; set; } = 1;
        public static int ChatBrokerServiceNumberOfShards { get; set; } = 1;
        public static int UserServiceNumberOfReplicas { get; set; } = 0;
        public static int StorageServiceNumberOfReplicas { get; set; } = 0;
        public static int ChatBrokerServiceNumberOfReplicas { get; set; } = 0;
    }
}
