using WebService.Models;

namespace WebService.DataAccessLayer.Interfaces
{
    public partial interface ILogOtherRepository
    {
        Task LogQueueOther(QueueLog queueLog);
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
