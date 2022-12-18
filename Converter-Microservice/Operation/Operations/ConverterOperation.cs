using DataAccess.Repository;
using Models;
using Nest;
using Operation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Operations
{
    public class ConverterOperation : IConverterOperation
    {
        private ConverterRepository converterRepository = new ConverterRepository();
        private ObjectStorageRepository objectStorageRepository = new ObjectStorageRepository();
        private QueueOperation<object> queueOperation = new QueueOperation<object>();
        private LoggingOtherOperation loggingOtherOperation = new LoggingOtherOperation();

        public async Task ConvertMP4_to_MP3(ObjectDataModel objDataModel, QueueMessage message)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();
                string ConvertToFilePath = Path.Combine(Path.GetTempPath(), guid + ".mp3");
                var conversionResult = converterRepository.ConvertMP4_to_MP3(objDataModel.FileFullPath, ConvertToFilePath);
                await Task.WhenAll(conversionResult);

                using (FileStream fs = File.OpenRead(ConvertToFilePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        await objectStorageRepository.StoreFileAsync("audios", guid, ms, "audio/mp3");

                        QueueMessage msg = new QueueMessage()
                        {
                            email = message.email,
                            fileGuid = guid
                        };

                        queueOperation.QueueMessageDirect(msg, "notification", "notification_exchange.direct", "mp4_to_notif");
                    }
                }
            }
            catch (Exception exception)
            {
                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    exceptionModel = exceptionModel,
                };

                await loggingOtherOperation.LogConverterError(errorLog);
            }
        }
    }
}
