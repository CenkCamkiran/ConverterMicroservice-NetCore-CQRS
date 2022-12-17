using Models;

namespace DataLayer.Interfaces
{
    public partial interface ILogOtherRepository
    {
        Task LogQueueOther(QueueLog queueLog);
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
