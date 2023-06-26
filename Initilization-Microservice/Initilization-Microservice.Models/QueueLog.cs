using System;

namespace Initilization_Microservice.Models
{
    public class QueueLog
    {
        public string OperationType { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}
