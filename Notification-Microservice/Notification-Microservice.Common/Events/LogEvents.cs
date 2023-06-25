namespace Notification_Microservice.Common.Events
{
    public static class LogEvents
    {
        public static string ConversionEvent { get; set; } = "Conversion finished!";
        public static string PubObjectEvent { get; set; } = "PutObjectAsync";
        public static string GetObjectEvent { get; set; } = "GetObjectAsync";
        public static string BasicPublishEvent { get; set; } = "BasicPublish";
        public static string BasicConsumeEvent { get; set; } = "BasicConsume";
    }
}