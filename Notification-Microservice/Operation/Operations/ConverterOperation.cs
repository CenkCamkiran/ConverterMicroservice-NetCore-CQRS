using Operation.Interfaces;

namespace Operation.Operations
{
    public class ConverterOperation : IConverterOperation
    {
        private IConverterRepository _converterRepository;
        private IObjectStorageOperation _objectStorageOperation;
        private IQueueOperation<object> _queueOperation;
        private ILoggingOperation _loggingOperation;

        public ConverterOperation(IConverterRepository converterRepository, IObjectStorageOperation objectStorageOperation, IQueueOperation<object> queueOperation, ILoggingOperation loggingOperation)
        {
            _converterRepository = converterRepository;
            _objectStorageOperation = objectStorageOperation;
            _queueOperation = queueOperation;
            _loggingOperation = loggingOperation;
        }

        public async Task ConvertMP4_to_MP3(ObjectDataModel objDataModel, QueueMessage message)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();
                string ConvertToFilePath = Path.Combine(Path.GetTempPath(), guid + ".mp3");
                var conversionResult = _converterRepository.ConvertMP4_to_MP3(objDataModel.FileFullPath, ConvertToFilePath);
                await Task.WhenAll(conversionResult);

                using (FileStream fs = File.OpenRead(ConvertToFilePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        await _objectStorageOperation.StoreFileAsync("audios", guid, ms, "audio/mp3");

                        QueueMessage msg = new QueueMessage()
                        {
                            email = message.email,
                            fileGuid = guid
                        };

                        await _queueOperation.QueueMessageDirectAsync(msg, "notification", "notification_exchange.direct", "mp4_to_notif");
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

                await _loggingOperation.LogConverterError(errorLog);
            }
        }
    }
}
