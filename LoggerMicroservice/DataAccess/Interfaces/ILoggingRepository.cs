﻿using Models;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ILoggingRepository<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
        Task LogOther(OtherLog otherLog);
        Task LogError(ErrorLog errorLog);
    }
}