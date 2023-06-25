namespace Initilization_Microservice.Repository.Interfaces
{
    public interface IQueueRepository
    {
        public bool ConfigureExchange(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments = null);
        public bool ConfigureQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments = null);
        public bool BindQueueToExchange(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null);
    }
}
