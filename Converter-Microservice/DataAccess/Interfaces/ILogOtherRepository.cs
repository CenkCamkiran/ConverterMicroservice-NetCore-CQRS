using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ILogOtherRepository
    {
        Task LogConverterOther();
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
