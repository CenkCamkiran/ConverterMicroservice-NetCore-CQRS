

namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string MinioHost { get; set; } = Environment.GetEnvironmentVariable("MINIO_HOST");
        public static bool MinioUseSsl { get; set; } = Convert.ToBoolean(Environment.GetEnvironmentVariable("MINIO_USE_SSL"));
        public static string MinioAccessKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        public static string MinioSecretKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
        public static string MinioBucketName { get; set; } = Environment.GetEnvironmentVariable("MINIO_BUCKET_NAME");
    }
}
