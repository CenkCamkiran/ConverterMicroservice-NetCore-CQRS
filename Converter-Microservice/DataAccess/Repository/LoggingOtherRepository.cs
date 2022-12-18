using DataAccess.Interfaces;
using Models;
using Newtonsoft.Json;

namespace DataAccess.Repository
{
    public class LoggingOtherRepository : ILoggingOtherRepository
    {
        private Log4NetRepository log = new Log4NetRepository();

        public async Task LogStorageOther(OtherLog otherLog)
        {
            QueueRepository<OtherLog> queueHandler = new QueueRepository<OtherLog>();
            queueHandler.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            log.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogStorageError(ErrorLog errorLog)
        {
            QueueRepository<ErrorLog> queueHandler = new QueueRepository<ErrorLog>();
            queueHandler.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            log.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogConverterError(ErrorLog errorLog)
        {
            QueueRepository<ErrorLog> queueHandler = new QueueRepository<ErrorLog>();
            queueHandler.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            log.Info(logText);
            await Task.FromResult(true);
        }

        public async Task LogConverterOther(OtherLog otherLog)
        {
            QueueRepository<OtherLog> queueHandler = new QueueRepository<OtherLog>();
            queueHandler.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(log)}";
            log.Info(logText);
            await Task.FromResult(true);
        }

        //public async Task LogQueueOther(OtherLog otherLog)
        //{
        //    QueueRepository<OtherLog> queueHandler = new QueueRepository<OtherLog>();
        //    queueHandler.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

        //    string logText = $"{JsonConvert.SerializeObject(log)}";
        //    log.Info(logText);
        //    await Task.FromResult(true);
        //}

        //public async Task LogQueueError(ErrorLog errorLog)
        //{
        //    QueueRepository<ErrorLog> queueHandler = new QueueRepository<ErrorLog>();
        //    queueHandler.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

        //    string logText = $"{JsonConvert.SerializeObject(log)}";
        //    log.Info(logText);
        //    await Task.FromResult(true);
        //}
    }
}
