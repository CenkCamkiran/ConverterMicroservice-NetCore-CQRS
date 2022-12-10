using Models;

namespace ServiceLayer.Interfaces
{
    public interface IConverterService
    {
        void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey);
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
