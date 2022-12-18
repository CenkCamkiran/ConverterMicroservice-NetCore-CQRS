using DataAccess.Interfaces;
using log4net;
using Models;
using Newtonsoft.Json;

namespace DataAccess.Repository
{
    public class LoggingRepository : ILoggingRepository
    {
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IQueueRepository<ErrorLog> _errorLogQueueRepository;
        private readonly IQueueRepository<OtherLog> _otherLogQueueRepository;

        public LoggingRepository(ILog4NetRepository log4NetRepository, IQueueRepository<ErrorLog> errorLogQueueRepository, IQueueRepository<OtherLog> otherLogQueueRepository)
        {
            _log4NetRepository = log4NetRepository;
            _errorLogQueueRepository = errorLogQueueRepository;
            _otherLogQueueRepository = otherLogQueueRepository;
        }

        public async Task LogStorageOther(OtherLog log)
        {
            _otherLogQueueRepository.QueueMessageDirect(log, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogStorageError(ErrorLog log)
        {
            _errorLogQueueRepository.QueueMessageDirect(log, "errorlogs", "log_exchange.direct", "error_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Error(logText);
            await Task.FromResult(true);
        }

        public async Task LogConverterError(ErrorLog log)
        {
            _errorLogQueueRepository.QueueMessageDirect(log, "errorlogs", "log_exchange.direct", "error_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Error(logText);
            await Task.FromResult(true);
        }

        public async Task LogConverterOther(OtherLog log)
        {
            _otherLogQueueRepository.QueueMessageDirect(log, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogQueueOther(OtherLog log)
        {
            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogQueueError(ErrorLog log)
        {
            string logText = $"{JsonConvert.SerializeObject(log)}";
            _log4NetRepository.Error(logText);
            await Task.FromResult(true);
        }

    }
}
