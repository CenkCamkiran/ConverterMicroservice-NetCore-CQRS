using MediatR;

namespace WebService.Commands.ObjectCommands
{
    public class ObjectCommand : IRequest<bool>
    {
        public string BucketName { get; set; }
        public string ObjectName { get; set; }
        public Stream Stream { get; set; }
        public string ContentType { get; set; }

        public ObjectCommand(string bucketName, string objectName, Stream stream, string contentType)
        {
            BucketName = bucketName;
            ObjectName = objectName;
            Stream = stream;
            ContentType = contentType;
        }
    }
}
