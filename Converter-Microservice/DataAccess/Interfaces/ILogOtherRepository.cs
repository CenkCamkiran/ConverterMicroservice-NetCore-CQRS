using Models;

namespace DataAccess.Interfaces
{
    public interface ILogOtherRepository
    {
        Task LogErrors();
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
