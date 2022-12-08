using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILoggingRepository
    {
        Task<bool> IndexDataAsync(string indexName, HttpRequest request, HttpResponse response);

    }
}
