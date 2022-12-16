using Minio.DataModel;
using Models;
using Xabe.FFmpeg;

namespace DataAccess.Repository
{
    public class ConverterRepository
    {
        private Logger log = new Logger();

        public async Task ConvertMP4_to_MP3(MemoryStream ms, string guid)
        {
            try
            {
                string fileName = guid + ".mp4";

                using (var fileStream = File.Create(Path.Combine(System.IO.Path.GetTempPath(), fileName)))
                {
                    await fileStream.CopyToAsync(ms);
                }

                string outputPath = Path.Combine(System.IO.Path.GetTempPath(), fileName);
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(fileName);

                IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                    ?.SetCodec(VideoCodec.mpeg4);
                IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                    ?.SetCodec(AudioCodec.mp3);

                IConversionResult? conversionResult = await FFmpeg.Conversions.New()
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
                //Başka bir queue'ya log at.
                //Filelogging devam et.

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };


                string logText = $"Exception: {exception.Message.ToString()}";
                log.Info(logText);

            }
            finally
            {

                //ObjectStorageLog objectStorageLog = new ObjectStorageLog()
                //{
                //    OperationType = nameof(minioClient.SelectObjectContentAsync),
                //    BucketName = bucketName,
                //    ContentLength = responseStream.Stats.BytesReturned,
                //    ObjectName = objectName,
                //    Date = DateTime.Now
                //};

                //QueueHandler<ObjectStorageLog> queueHandler = new QueueHandler<ObjectStorageLog>();
                //queueHandler.QueueMessageDirectAsync(objectStorageLog, "otherlogs", "log_exchange.direct", "other_log");

                //string logText = $"{JsonConvert.SerializeObject(objectStorageLog)}";
                //log.Info(logText);
            }
        }
    }
}
