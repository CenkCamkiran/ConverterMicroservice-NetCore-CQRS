using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface ILoggingOperation
    {
        Task LogStorageOther(OtherLog objectStorageLog);
        Task LogStorageError(ErrorLog errorLog);
        Task LogConverterError(ErrorLog errorLog);
        Task LogConverterOther(OtherLog otherLog);
        Task LogQueueOther(OtherLog otherLog);
        Task LogQueueError(ErrorLog errorLog);
    }
}
