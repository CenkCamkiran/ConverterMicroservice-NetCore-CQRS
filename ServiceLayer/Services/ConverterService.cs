using DataLayer.Interfaces;
using Models;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services
{
    public class ConverterService : IConverterService
    {
        private IConverterRepository _converterRepository;

        public ConverterService(IConverterRepository converterRepository)
        {
            _converterRepository = converterRepository;
        }

        public void QueueMessageDirect(QueueMessage message, string queue, string exchange, string routingKey)
        {
            _converterRepository.QueueMessageDirect(message, queue, exchange, routingKey);
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {
            await _converterRepository.StoreFileAsync(bucketName, objectName, stream, contentType);
        }
    }
}
