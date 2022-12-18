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
    public class LoggingOtherOperation: ILoggingOtherOperation
    {
        private LoggingOtherRepository loggingOtherRepository = new LoggingOtherRepository();

        public async Task LogConverterError(ErrorLog errorLog)
        {
            await loggingOtherRepository.LogConverterError(errorLog);
        }

        public async Task LogStorageError(ErrorLog errorLog)
        {
            await loggingOtherRepository.LogStorageError(errorLog);
        }

        public async Task LogStorageOther(OtherLog objectStorageLog)
        {
            await loggingOtherRepository.LogStorageOther(objectStorageLog);
        }
    }
}
