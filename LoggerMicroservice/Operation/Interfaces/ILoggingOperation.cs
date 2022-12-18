using Models;

namespace Operation.Interfaces
{
    public interface ILoggingOperation<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
        Task LogOther(OtherLog otherLog);
        Task LogError(ErrorLog errorLog);

    }
}
