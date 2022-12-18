using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface ILoggingOtherOperation
    {
        Task LogStorageOther(OtherLog objectStorageLog);
        Task LogStorageError(ErrorLog errorLog);
        Task LogConverterError(ErrorLog errorLog);
    }
}
