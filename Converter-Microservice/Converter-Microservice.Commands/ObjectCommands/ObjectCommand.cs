using MediatR;

namespace Converter_Microservice.Commands.ObjectCommands
{
    public class ObjectCommand : IRequest<bool>
    {
        public string ObjectName { get; set; }
        public string BucketName { get; set; }
        public string ContentType { get; set; }
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
