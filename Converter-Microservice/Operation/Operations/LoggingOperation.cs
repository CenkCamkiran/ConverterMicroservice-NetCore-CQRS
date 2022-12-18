using DataAccess.Interfaces;
using DataAccess.Repository;
using Models;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
{
    public class LoggingOperation: ILoggingOperation
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
