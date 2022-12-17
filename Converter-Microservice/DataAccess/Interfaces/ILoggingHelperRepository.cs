using Models;

namespace DataAccess.Interfaces
{
    public interface ILoggingHelperRepository
    {
        Task LogStorageOther(OtherLog objectStorageLog);
        Task LogStorageError(ErrorLog errorLog);
        Task LogConverterError(ErrorLog errorLog);
        //Task LogQueueOther(OtherLog otherLog);
        //Task LogQueueError(ErrorLog errorLog);
    }
}
