using Models;

namespace DataAccess.Interfaces
{
    public interface ILoggingRepository
    {
        Task LogStorageOther(OtherLog objectStorageLog);
        Task LogStorageError(ErrorLog errorLog);
        Task LogConverterError(ErrorLog errorLog);
        Task LogConverterOther(OtherLog otherLog);
        Task LogQueueOther(OtherLog otherLog);
        Task LogQueueError(ErrorLog errorLog);
    }
}
