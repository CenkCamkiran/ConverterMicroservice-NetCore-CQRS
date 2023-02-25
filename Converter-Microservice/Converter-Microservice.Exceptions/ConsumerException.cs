namespace Converter_Microservice.Exceptions
{
    public class ConsumerException : Exception
    {
        public ConsumerException()
        {
        }

        public ConsumerException(string? message) : base(message)
        {
        }
    }
}
