using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface ILog4NetOperation
    {
        void QueueFileLogging(QueueLog queueLog);
        void ObjStorageFileLogging(ObjectStorageLog queueLog);
    }
}
