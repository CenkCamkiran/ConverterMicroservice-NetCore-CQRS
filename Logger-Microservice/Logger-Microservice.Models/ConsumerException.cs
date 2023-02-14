using System;

namespace LoggerMicroservice.Models
{
    public class ConsumerException : Exception
    {
        public ConsumerException()
        {
        }

        public ConsumerException(string message) : base(message)
        {
        }
    }
}
