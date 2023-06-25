namespace Initilization_Microservice.Common.Events
{
    public static class LogEvents
    {
        public static readonly int ProgramStarted = 0;

        public static readonly int ExchangeCreationPhase = 1;
        public static readonly int QueueCreationPhase = 2;
        public static readonly int StorageBucketCreationPhase = 3;
        public static readonly int ElkIndexCreationPhase = 4;
        public static readonly int QueueExchangeBindingPhase = 5;

        public static readonly int ExchangeCreationPhaseError = 11;
        public static readonly int QueueCreationPhaseError = 12;
        public static readonly int StorageBucketCreationPhaseError = 13;
        public static readonly int ElkIndexCreationPhaseError = 14;
        public static readonly int QueueExchangeBindingPhaseError = 15;

        public static string ElkIndexCreationPhaseMessage { get; set; } = "Index created in ELK";
        public static string ExchangeCreationPhaseMessage { get; set; } = "Exchange created in RabbitMQ";
        public static string QueueCreationPhaseMessage { get; set; } = "Queue created in RabbitMQ";
        public static string QueueExchangeBindingPhaseMessage { get; set; } = "Queue created in RabbitMQ";
    }
}