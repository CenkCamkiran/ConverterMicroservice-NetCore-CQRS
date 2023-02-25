using ConverterMicroservice.Models;
using MediatR;

namespace Converter_Microservice.Commands.ConverterCommands
{
    public class ConverterCommand : IRequest<QueueMessage>
    {
        public ObjectData ObjectData { get; set; }
        public QueueMessage QueueMessage { get; set; }

        public ConverterCommand(ObjectData objectData, QueueMessage queueMessage)
        {
            ObjectData = objectData;
            QueueMessage = queueMessage;
        }
    }
}
