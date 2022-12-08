using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IConverterRepository
    {
        void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey);
        Task StoreFileAsync(string bucketName, string objectName, Stream fileStream, string contentType);
    }
}
