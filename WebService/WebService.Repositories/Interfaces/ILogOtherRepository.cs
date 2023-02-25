using WebService.Models;

namespace WebService.Repositories.Interfaces
{
    public partial interface ILogOtherRepository
    {
        Task LogQueueOther(QueueLog queueLog);
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
