using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public partial interface ILogOtherRepository
    {
        Task LogQueueOther(QueueLog queueLog);
        Task LogStorageOther(ObjectStorageLog objectStorageLog);
    }
}
