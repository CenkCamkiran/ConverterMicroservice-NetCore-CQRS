using MediatR;

namespace Notification_Microservice.Commands.ObjectCommands
{
    public class ObjectCommand : IRequest<bool>
    {
        public string ObjectName { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public Stream Stream { get; set; }

        public ObjectCommand(string objectName, string bucketName, string contentType, Stream stream)
        {
            ObjectName = objectName;
            BucketName = bucketName;
            ContentType = contentType;
            Stream = stream;
        }
    }
}
