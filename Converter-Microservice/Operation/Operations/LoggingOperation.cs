using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;

namespace Operation.Operations
{
    public class LoggingOperation : ILoggingOperation
    {
        private ILoggingRepository _loggingRepository;

        public LoggingOperation(ILoggingRepository loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task LogConverterError(ErrorLog errorLog)
        {
            await _loggingRepository.LogConverterError(errorLog);
        }

        public async Task LogConverterOther(OtherLog otherLog)
        {
            await _loggingRepository.LogConverterOther(otherLog);
        }

        public async Task LogQueueError(ErrorLog errorLog)
        {
            await _loggingRepository.LogQueueError(errorLog);
        }

        public async Task LogQueueOther(OtherLog otherLog)
        {
            await _loggingRepository.LogQueueOther(otherLog);
        }

        public async Task LogStorageError(ErrorLog errorLog)
        {
            await _loggingRepository.LogStorageError(errorLog);
        }

        public async Task LogStorageOther(OtherLog objectStorageLog)
        {
            await _loggingRepository.LogStorageOther(objectStorageLog);
        }
    }
}
