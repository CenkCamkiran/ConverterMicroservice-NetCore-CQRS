namespace Interfaces
{
    public interface ILoggingOperation<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);

    }
}
