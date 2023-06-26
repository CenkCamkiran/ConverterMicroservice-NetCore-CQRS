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

        public static string ConversionFinishedEventMessage { get; set; } = "Conversion is finished";
        public static string PutObjectEventMessage { get; set; } = "PutObjectAsync is finished";
        public static string GetObjectEventMessage { get; set; } = "GetObjectAsync is finished";
        public static string BasicPublishEventMessage { get; set; } = "Basic Publish is finished";
        public static string BasicConsumeEventMessage { get; set; } = "Basic Consume is started";
        public static string ConversionStartedEventMessage { get; set; } = "Conversion is started";
        public static string ConversionFailureEventMessage { get; set; } = "Conversion is failed";
        public static string ServiceConfigurationPhaseMessage { get; set; } = "Service is fetching configurations";
    }
}