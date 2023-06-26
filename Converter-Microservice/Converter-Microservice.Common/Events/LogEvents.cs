namespace Converter_Microservice.Common.Events
{
    public static class LogEvents
    {
        public static readonly int ServiceConfigurationPhase = 1;
        public static readonly int ConversionStartedEvent = 2;
        public static readonly int ConversionFinishedEvent = 3;
        public static readonly int ConversionFailureEvent = 4;
        public static readonly int PutObjectEvent = 5;
        public static readonly int GetObjectEvent = 6;
        public static readonly int BasicPublishEvent = 7;
        public static readonly int BasicConsumeEvent = 8;

        public static string ConversionFinishedEventMessage { get; set; } = "Conversion";
        public static string PutObjectEventMessage { get; set; } = "PutObjectAsync";
        public static string GetObjectEventMessage { get; set; } = "GetObjectAsync";
        public static string BasicPublishEventMessage { get; set; } = "Basic Publish";
        public static string BasicConsumeEventMessage { get; set; } = "Basic Consume";
        public static string ConversionStartedEventMessage { get; set; } = "Conversion";
        public static string ConversionFailureEventMessage { get; set; } = "Conversion";
        public static string ServiceConfigurationPhaseMessage { get; set; } = "Service is fetching configurations";
    }
}