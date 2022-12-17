using DataAccess.Interfaces;
using Models;
using Newtonsoft.Json;

namespace DataAccess.Repository
{
    public class LogOtherRepository : ILogOtherRepository
    {
        private Log4NetRepository log = new Log4NetRepository();

        public Task LogErrors()
        {
            throw new NotImplementedException();
        }

        public async Task LogStorageOther(ObjectStorageLog objectStorageLog)
        {
            QueueRepository<ObjectStorageLog> queueHandler = new QueueRepository<ObjectStorageLog>();
            queueHandler.QueueMessageDirect(objectStorageLog, "otherlogs", "log_exchange.direct", "other_log");

            string logText = $"{JsonConvert.SerializeObject(objectStorageLog)}";
            log.Info(logText);
            await Task.FromResult(true);
        }
    }
}
