

namespace Initilization_Microservice.Common
{
    public static partial class ProjectConstants
    {
        public static string MinioHost { get; set; } = Environment.GetEnvironmentVariable("MINIO_HOST");
        public static bool MinioUseSsl { get; set; } = Convert.ToBoolean(Environment.GetEnvironmentVariable("MINIO_USE_SSL"));
        public static string MinioAccessKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        public static string MinioSecretKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
        public static string MinioAudioBucket { get; set; } = Environment.GetEnvironmentVariable("MINIO_AUDIO_BUCKET_NAME");
        public static string MinioVideoBucketName { get; set; } = Environment.GetEnvironmentVariable("MINIO_VIDEO_BUCKET_NAME");
    }
}
