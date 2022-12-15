using System.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xabe.FFmpeg;
using log4net;
using Models;
using Newtonsoft.Json;
using System.Collections;

namespace DataAccess
{
    public class ConverterHandler
    {
        private Logger log = new Logger();

        public async Task con()
        {
			try
			{
                string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo("");

                IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                    ?.SetCodec(VideoCodec.mpeg4);
                IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                    ?.SetCodec(AudioCodec.mp3);

                IConversionResult? obj = await FFmpeg.Conversions.New()
                    .AddStream(audioStream, videoStream)
                    .SetOutput(outputPath)
                    .Start();

                //ElkLogging<QueueLog> elkLogging = new ElkLogging<QueueLog>();
                //await elkLogging.IndexExceptionAsync("converter_queue_logs", queueLog);

                //string logText = $"Exchange: {exchange} - Queue: {queue} - Routing Key: {routingKey} - Message: (fileGuid: {message.fileGuid} && email: {message.email})";
                //log.Info(logText);
            }
			catch (Exception exception)
			{
                ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);
            }
        }
    }
}
