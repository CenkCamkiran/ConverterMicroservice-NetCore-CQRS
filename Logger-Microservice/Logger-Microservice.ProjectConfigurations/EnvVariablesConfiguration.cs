using LoggerMicroservice.Models;

namespace Logger_Microservice.ProjectConfigurations
{
    public class EnvVariablesConfiguration
    {

        public ElkConfiguration GetElkEnvVariables()
        {
            string elkHost = Environment.GetEnvironmentVariable("ELK_HOST");
            string elkDefaultIndex = Environment.GetEnvironmentVariable("ELK_DEFAULT_INDEX");
            string elkUsername = Environment.GetEnvironmentVariable("ELK_USERNAME");
            string elkPassword = Environment.GetEnvironmentVariable("ELK_PASSWORD");

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
            string rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
            string rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
            string rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
            string rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

            RabbitMqConfiguration rabbitMqConfiguration = new RabbitMqConfiguration()
            {
                RabbitMqHost = rabbitmqHost,
                RabbitMqPassword = rabbitmqPassword,
                RabbitMqPort = rabbitmqPort,
                RabbitMqUsername = rabbitmqUsername
            };

            return rabbitMqConfiguration;
        }
    }
}
