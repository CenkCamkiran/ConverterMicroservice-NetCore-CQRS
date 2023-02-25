using ConverterMicroservice.Models;

namespace Converter_Microservice.Repositories.Interfaces
{
    public interface IConverterRepository
    {
        Task<QueueMessage> ConvertMP4_to_MP3_Async(ObjectData objDataModel, QueueMessage message);
    }
}
