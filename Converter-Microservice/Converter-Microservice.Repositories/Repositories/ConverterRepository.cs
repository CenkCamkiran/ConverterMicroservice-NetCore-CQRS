using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Common.Constants;
using Converter_Microservice.Common.Events;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using MediatR;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace Converter_Microservice.Repositories.Repositories
{
    public class ConverterRepository : IConverterRepository
    {
        private readonly ILog4NetRepository _log4NetRepository;
        private readonly IObjectRepository _objectStorageRepository;
        private readonly IMediator _mediator;

        public ConverterRepository(ILog4NetRepository log4NetRepository, IObjectRepository objectStorageRepository, IMediator mediator)
        {
            _log4NetRepository = log4NetRepository;
            _objectStorageRepository = objectStorageRepository;
            _mediator = mediator;
        }

        public async Task<QueueMessage> ConvertMP4_to_MP3_Async(ObjectData objDataModel, QueueMessage message)
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
                        await _objectStorageRepository.StoreFileAsync(ProjectConstants.MinioAudiosBucket, guid, ms, "audio/mp3");

                        msg = new QueueMessage()
                        {
                            email = message.email,
                            fileGuid = guid
                        };

                        await _mediator.Send(new QueueCommand(msg, ProjectConstants.NotificationServiceQueueName, ProjectConstants.NotificationServiceExchangeName, ProjectConstants.NotificationServiceRoutingKey, ProjectConstants.NotificationServiceExchangeTtl));

                    }
                }

                if (File.Exists(Mp3FileFullPath))
                    File.Delete(Mp3FileFullPath);

                if (File.Exists(objDataModel.Mp4FileFullPath))
                    File.Delete(objDataModel.Mp4FileFullPath);

                ConverterLog converterLog = new ConverterLog()
                {
                    Info = LogEvents.ConversionEvent,
                    Date = DateTime.Now
                };
                OtherLog otherLog = new OtherLog()
                {
                    converterLog = converterLog
                };
                await _mediator.Send(new QueueCommand(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.OtherLogsServiceExchangeTtl));

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
                await _mediator.Send(new QueueCommand(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey, ProjectConstants.ErrorLogsServiceExchangeTtl));

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Error(logText);

                return msg;
            }
        }
    }
}
