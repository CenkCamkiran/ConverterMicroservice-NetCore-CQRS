using DataAccess.Repository;
using Models;
using Newtonsoft.Json;
using Operation.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
{
    public class Log4NetOperation: ILog4NetOperation
    {
        private Log4NetRepository log = new Log4NetRepository();

        public void ObjStorageFileLogging(ObjectStorageLog queueLog)
        {
            throw new NotImplementedException();
        }

        public void QueueFileLogging(QueueLog queueLog)
        {
            string logText = $"{JsonConvert.SerializeObject(queueLog)}";
            log.Info(logText);
        }
    }
}
