namespace Initilization_Microservice.Operation.Interfaces
{
    public interface IQueueOperation
    {
        public Task<bool> ConfigureExchangeAndQueueAsync(string exchange, string exchangeType, bool exchangeDurable, bool exchangeAutoDelete, string queue, bool queueDurable, bool queueExclusive, bool queueAutoDelete, string routingKey, IDictionary<string, object> arguments = null);
    }
}
