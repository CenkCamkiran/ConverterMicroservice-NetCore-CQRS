using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IConverterService {
        void QueueMessageDirect(string message, string queue, string exchange, string routingKey);
        Task<bool> StoreFileAsync(string bucketName, string location, string objectName, string fileName, string fileContent, string contentType);
    }
}
