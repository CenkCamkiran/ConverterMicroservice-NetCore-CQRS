using Models;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository
    {
        Task<bool> IndexReqResAsync(string indexName, RequestResponseLogModel model);
        Task<bool> IndexProcessAsync(string indexName, RequestResponseLogModel model);
        Task<bool> IndexExceptionAsync(string indexName, ExceptionLogModel model)

    }
}
