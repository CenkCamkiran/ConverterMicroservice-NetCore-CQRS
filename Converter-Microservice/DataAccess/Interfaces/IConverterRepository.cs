using Models;

namespace DataAccess.Interfaces
{
    public interface IConverterRepository
    {
        Task<QueueMessage> ConvertMP4_to_MP3_Async(ObjectDataModel objDataModel, QueueMessage message);
    }
}
