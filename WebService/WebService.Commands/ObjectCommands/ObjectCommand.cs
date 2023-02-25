using MediatR;

namespace WebService.Commands.ObjectCommands
{
    public class ObjectCommand : IRequest<bool>
    {
        public string BucketName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public Stream Stream { get; set; }
        public string ContentType { get; set; } = string.Empty;

        public ObjectCommand(string bucketName, string objectName, Stream stream, string contentType)
        {
            BucketName = bucketName;
            ObjectName = objectName;
            Stream = stream;
            ContentType = contentType;
        }
    }
}
