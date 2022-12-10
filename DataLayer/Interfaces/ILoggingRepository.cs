using Models;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository
    {
        Task<bool> IndexReqResAsync(string indexName, RequestResponseLogModel model);

    }
}
