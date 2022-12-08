﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository<TLogModel>
    {
        Task<bool> IndexReqResAsync(string indexName, TLogModel model);

    }
}