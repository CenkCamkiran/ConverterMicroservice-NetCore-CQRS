using NotificationMicroservice.Models;

namespace NotificationMicroservice.Configuration
{
    public class EnvVariablesHandler
    {

        public MinioConfiguration GetMinioEnvVariables()
        {
            string? minioHost = Environment.GetEnvironmentVariable("MINIO_HOST");
            string? minioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            string? minioSecretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");

            MinioConfiguration minioConfiguration = new MinioConfiguration()
            {
                MinioAccessKey = minioAccessKey,
                MinioSecretKey = minioSecretKey,
                MinioHost = minioHost
            };

            return minioConfiguration;
        }

        public ElkConfiguration GetElkEnvVariables()
        {
            string? elkHost = Environment.GetEnvironmentVariable("ELK_HOST");
            string? elkDefaultIndex = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX");
            string? elkUsername = Environment.GetEnvironmentVariable("ELK_USERNAME");
            string? elkPassword = Environment.GetEnvironmentVariable("ELK_PASSWORD");

            ElkConfiguration elkConfiguration = new ElkConfiguration()
            {
                ElkHost = elkHost,
                ElkUsername = elkUsername,
                ElkPassword = elkPassword,
                ElkDefaultIndex = elkDefaultIndex
            };

            return elkConfiguration;
        }

        public RabbitMqConfiguration GetRabbitEnvVariables()
        {
            string? rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
            string? rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
            string? rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
            string? rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

            RabbitMqConfiguration rabbitMqConfiguration = new RabbitMqConfiguration()
            {
                RabbitMqHost = rabbitmqHost,
                RabbitMqPassword = rabbitmqPassword,
                RabbitMqPort = rabbitmqPort,
                RabbitMqUsername = rabbitmqUsername
            };

            return rabbitMqConfiguration;
        }

        public SmtpConfiguration GetSmtpEnvVariables()
        {
            string? smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            string? smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");
            string? smtpMailFrom = Environment.GetEnvironmentVariable("SMTP_MAIL_FROM");
            string? smtpMailPassword = Environment.GetEnvironmentVariable("SMTP_MAIL_PASSWORD");
            string? smtpMailUsername = Environment.GetEnvironmentVariable("SMTP_MAIL_USERNAME");

            SmtpConfiguration smtpConfiguration = new SmtpConfiguration()
            {
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                SmtpMailFrom = smtpMailFrom,
                SmtpMailPassword = smtpMailPassword,
                SmtpMailUsername = smtpMailUsername
            };

            return smtpConfiguration;
        }

    }
}
