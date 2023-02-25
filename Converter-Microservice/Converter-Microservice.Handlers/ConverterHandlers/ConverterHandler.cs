using Converter_Microservice.Commands.ConverterCommands;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using MediatR;

namespace Converter_Microservice.Handlers.ConverterHandlers
{
    public class ConverterHandler : IRequestHandler<ConverterCommand, QueueMessage>
    {
        private readonly IConverterRepository _converterRepository;

        public ConverterHandler(IConverterRepository converterRepository)
        {
            _converterRepository = converterRepository;
        }

        public async Task<QueueMessage> Handle(ConverterCommand request, CancellationToken cancellationToken)
        {
            return await _converterRepository.ConvertMP4_to_MP3_Async(request.ObjectData, request.QueueMessage);
        }
    }
}
