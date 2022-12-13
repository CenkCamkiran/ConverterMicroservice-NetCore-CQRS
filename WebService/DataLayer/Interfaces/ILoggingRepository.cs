using Models;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository<TModel> where TModel : class
    {
        //Task<bool> IndexReqResAsync(string indexName, RequestResponseLogModel model);
        //Task<bool> IndexProcessAsync(string indexName, RequestResponseLogModel model);
        //Task<bool> IndexExceptionAsync(string indexName, ExceptionLogModel model);
        Task<bool> IndexDocAsync(string indexName, TModel model);

    }
}
