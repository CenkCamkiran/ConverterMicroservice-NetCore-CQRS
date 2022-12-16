using DataAccess.Interfaces;
using log4net;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class LogOtherRepository : ILogOtherRepository
    {
        private Logger log = new Logger();

        public Task LogConverterOther()
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
