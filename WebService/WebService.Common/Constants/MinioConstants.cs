namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string MinioHost { get; set; } = Environment.GetEnvironmentVariable("MINIO_HOST");
        public static string MinioAccessKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
        public static string MinioSecretKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");
        public static string MinioBucketName { get; set; } = "videos";
    }
}
