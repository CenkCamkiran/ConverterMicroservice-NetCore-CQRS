using LoggerMicroservice.DataAccessLayer.Interfaces;
using LoggerMicroservice.OperationLayer.Interfaces;

namespace LoggerMicroservice.OperationLayer.Operations
{
    public class LoggingOperation<TModel> : ILoggingOperation<TModel> where TModel : class
    {
        ILoggingRepository<TModel> _loggingRepository;

        public LoggingOperation(ILoggingRepository<TModel> loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task<bool> IndexDocAsync(string indexName, TModel model)
        {
            return await _loggingRepository.IndexDocAsync(indexName, model);
        }
    }
}
