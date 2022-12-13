using Models;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
    }
}
