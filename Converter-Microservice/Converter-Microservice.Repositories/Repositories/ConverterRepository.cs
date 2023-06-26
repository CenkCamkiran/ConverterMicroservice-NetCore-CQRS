using Converter_Microservice.Commands.QueueCommands;
using Converter_Microservice.Common.Constants;
using Converter_Microservice.Common.Events;
using Converter_Microservice.Repositories.Interfaces;
using ConverterMicroservice.Models;
using log4net.Core;
using log4net.Repository.Hierarchy;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace Converter_Microservice.Repositories.Repositories
{
    public class ConverterRepository : IConverterRepository
    {
        private readonly IObjectRepository _objectStorageRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<ConverterRepository> _logger;

        public ConverterRepository(IObjectRepository objectStorageRepository, IMediator mediator, ILogger<ConverterRepository> logger)
        {
            _objectStorageRepository = objectStorageRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<QueueMessage> ConvertMP4_to_MP3_Async(ObjectData objDataModel, QueueMessage message)
        {
            _logger.LogInformation(LogEvents.ConversionStartedEvent, LogEvents.ConversionStartedEventMessage);

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
                        await _objectStorageRepository.PutObjectAsync(ProjectConstants.MinioAudioBucket, guid, ms, "audio/mp3");

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
                    Info = LogEvents.ConversionFinishedEventMessage,
                    Date = DateTime.Now
                };
                OtherLog otherLog = new OtherLog()
                {
                    converterLog = converterLog
                };
                await _mediator.Send(new QueueCommand(otherLog, ProjectConstants.OtherLogsServiceQueueName, ProjectConstants.OtherLogsServiceExchangeName, ProjectConstants.OtherLogsServiceRoutingKey, ProjectConstants.OtherLogsServiceExchangeTtl));
                _logger.LogInformation(LogEvents.ConversionFinishedEvent, LogEvents.ConversionFinishedEventMessage);

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

                _logger.LogError(LogEvents.ConversionFailureEvent, LogEvents.ConversionFailureEventMessage);

                return msg;
            }
        }
    }
}
