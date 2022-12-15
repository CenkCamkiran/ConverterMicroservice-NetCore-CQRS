using System;

namespace Models
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
