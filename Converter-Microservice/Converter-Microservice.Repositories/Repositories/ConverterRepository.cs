using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace Converter_Microservice.Repositories.Repositories
{
    public class ConverterRepository : IConverterRepository
    {
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IObjectRepository _objectStorageRepository;
        private readonly Lazy<IQueueRepository<QueueMessage>> _queueRepository;
        private readonly Lazy<IQueueRepository<ErrorLog>> _queueErrorRepository;
        private readonly Lazy<IQueueRepository<OtherLog>> _queueOtherRepository;

        public ConverterRepository(ILog4NetRepository log4NetRepository, IObjectRepository objectStorageRepository, Lazy<IQueueRepository<QueueMessage>> queueRepository, Lazy<IQueueRepository<ErrorLog>> queueErrorRepository, Lazy<IQueueRepository<OtherLog>> queueOtherRepository)
        {
            _log4NetRepository = log4NetRepository;
            _objectStorageRepository = objectStorageRepository;
            _queueRepository = queueRepository;
            _queueErrorRepository = queueErrorRepository;
            _queueOtherRepository = queueOtherRepository;
        }

        public async Task<QueueMessage> ConvertMP4_to_MP3_Async(ObjectData objDataModel, QueueMessage message) //string ConvertFromFilePath, string ConvertToFilePath
        {
            QueueMessage? msg = null;

            try
            {
                string guid = Guid.NewGuid().ToString();
                string Mp3FileFullPath = Path.Combine(Path.GetTempPath(), guid + ".mp3");

                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(objDataModel.Mp4FileFullPath, Mp3FileFullPath);
                conversion.SetOverwriteOutput(false);

                await conversion.Start();

                using (FileStream fs = File.OpenRead(Mp3FileFullPath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        await _objectStorageRepository.StoreFileAsync("audios", guid, ms, "audio/mp3");

                        msg = new QueueMessage()
                        {
                            email = message.email,
                            fileGuid = guid
                        };

                        _queueRepository.Value.QueueMessageDirect(msg, "notification", "notification_exchange.direct", "mp4_to_notif", 3600000);

                    }
                }

                if (File.Exists(Mp3FileFullPath))
                    File.Delete(Mp3FileFullPath);

                if (File.Exists(objDataModel.Mp4FileFullPath))
                    File.Delete(objDataModel.Mp4FileFullPath);

                ConverterLog converterLog = new ConverterLog()
                {
                    Info = "Conversion finished!",
                    Date = DateTime.Now
                };
                OtherLog otherLog = new OtherLog()
                {
                    converterLog = converterLog
                };
                _queueOtherRepository.Value.QueueMessageDirect(otherLog, "otherlogs", "log_exchange.direct", "other_log");

                string logText = $"{JsonConvert.SerializeObject(otherLog)}";
                _log4NetRepository.Info(logText);

                return msg;

            }
            catch (Exception exception)
            {
                ConverterLog exceptionModel = new ConverterLog()
                {
                    Error = exception.Message.ToString(),
                    Date = DateTime.Now
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    converterLog = exceptionModel
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return msg;
            }
        }
    }
}
