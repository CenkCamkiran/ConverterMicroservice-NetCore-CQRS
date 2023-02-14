using System.Threading.Tasks;

namespace Interfaces
{
    public interface ILoggingRepository<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
    }
}
