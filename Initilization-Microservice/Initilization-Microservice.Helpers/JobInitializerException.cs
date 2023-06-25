namespace Initilization_Microservice.Helpers
{
    public class JobInitializerException : Exception
    {
        public JobInitializerException()
        {
        }

        public JobInitializerException(string? message) : base(message)
        {
        }
    }
}
