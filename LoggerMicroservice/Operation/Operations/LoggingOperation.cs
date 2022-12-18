using DataAccess.Interfaces;
using Models;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
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

        public async Task LogError(ErrorLog errorLog)
        {
            await _loggingRepository.LogError(errorLog);
        }

        public async Task LogOther(OtherLog otherLog)
        {
            await _loggingRepository.LogOther(otherLog);
        }
    }
}
