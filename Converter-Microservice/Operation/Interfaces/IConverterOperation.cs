using Models;

namespace Operation.Interfaces
{
    public interface IConverterOperation
    {
        Task ConvertMP4_to_MP3(ObjectDataModel objectDataModel, QueueMessage message);
    }
}
