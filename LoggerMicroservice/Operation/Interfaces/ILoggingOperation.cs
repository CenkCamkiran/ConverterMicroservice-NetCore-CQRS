using Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface ILoggingOperation<TModel> where TModel : class
    {
        Task<bool> IndexDocAsync(string indexName, TModel model);
        Task LogOther(OtherLog otherLog);
        Task LogError(ErrorLog errorLog);

    }
}
