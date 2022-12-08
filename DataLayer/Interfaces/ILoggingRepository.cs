using Microsoft.AspNetCore.Http;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository
    {
        Task<bool> IndexReqResAsync(string indexName, RequestResponseLogModel model);

    }
}
