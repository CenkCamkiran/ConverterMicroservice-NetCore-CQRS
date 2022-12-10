using Models;

namespace DataLayer.Interfaces
{
    public interface IConverterRepository
    {
        void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey);
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
    }
}
