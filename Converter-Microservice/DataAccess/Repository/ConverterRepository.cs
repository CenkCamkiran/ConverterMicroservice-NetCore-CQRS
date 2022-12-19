using DataAccess.Interfaces;
using Models;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace DataAccess.Repository
{
    public class ConverterRepository : IConverterRepository
    {
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IObjectStorageRepository _objectStorageRepository;

        public ConverterRepository(ILog4NetRepository log4NetRepository, IObjectStorageRepository objectStorageRepository)
        {
            _log4NetRepository = log4NetRepository;
            _objectStorageRepository = objectStorageRepository;
        }

        public async Task<QueueMessage> ConvertMP4_to_MP3(ObjectDataModel objDataModel, QueueMessage message) //string ConvertFromFilePath, string ConvertToFilePath
        {
            QueueMessage? msg = null;

            try
            {
                string guid = Guid.NewGuid().ToString();
                string ConvertToFilePath = Path.Combine(Path.GetTempPath(), guid + ".mp3");

                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(objDataModel.FileFullPath, ConvertToFilePath);
                conversion.SetOverwriteOutput(false);

                await conversion.Start();

                using (FileStream fs = File.OpenRead(ConvertToFilePath))
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

                    }
                }

                ConverterLog converterLog = new ConverterLog()
                {
                    Info = "Conversion finished!"
                };
                OtherLog otherLog = new OtherLog()
                {
                    converterLog = converterLog
                };
                string logText = $"{JsonConvert.SerializeObject(otherLog)}";
                _log4NetRepository.Info(logText);

                return msg;

            }
            catch (Exception exception)
            {
                ConverterLog exceptionModel = new ConverterLog()
                {
                    Error = exception.Message.ToString()
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    converterLog = exceptionModel
                };

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return msg;
            }
        }
    }
}
