namespace WebService.Repositories.Interfaces
{
    public interface ILogRepository<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
    }
}
