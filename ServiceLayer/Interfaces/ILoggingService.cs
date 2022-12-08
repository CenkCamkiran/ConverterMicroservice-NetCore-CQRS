using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ILoggingService
    {
        Task<bool> Log(string indexName, HttpRequest request, HttpResponse response);
    }
}
