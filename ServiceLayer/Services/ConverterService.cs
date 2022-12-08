using DataLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ConverterService : IConverterService
    {
        private IConverterRepository _converterRepository;

        public ConverterService(IConverterRepository converterRepository)
        {
            _converterRepository = converterRepository;
        }

        public void QueueMessageDirect(string message, string queue, string exchange, string routingKey)
        {
             _converterRepository.QueueMessageDirect(message, queue, exchange, routingKey);   
        }

        public async Task<bool> StoreFileAsync(string bucketName, string location, string objectName, string fileName, string fileContent, string contentType)
        {
            return await _converterRepository.StoreFileAsync(bucketName, location, objectName, fileName, fileContent, contentType);  
        }
    }
}
