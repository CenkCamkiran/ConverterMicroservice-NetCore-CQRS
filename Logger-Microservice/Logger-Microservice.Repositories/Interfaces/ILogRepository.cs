namespace Logger_Microservice.Repositories.Interfaces
{
    public interface ILogRepository
    {
        Task<bool> IndexDocAsync(string indexName, object model);
    }
}
