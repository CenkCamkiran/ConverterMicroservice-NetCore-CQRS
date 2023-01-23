namespace WebService.DataAccessLayer.Interfaces
{
    public interface ILoggingRepository<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
    }
}
