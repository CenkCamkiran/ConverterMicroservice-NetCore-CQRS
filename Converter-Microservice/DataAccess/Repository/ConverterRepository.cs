using Models;
using System.Resources;
using System;
using Xabe.FFmpeg;
using Nest;
using Newtonsoft.Json;
using System.Security.AccessControl;

namespace DataAccess.Repository
{
    public class ConverterRepository
    {
        private Log4NetRepository log = new Log4NetRepository();

        public async Task ConvertMP4_to_MP3(string ConvertFromFilePath, string ConvertToFilePath)
        {
            try
            {
                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(ConvertFromFilePath, ConvertToFilePath);
                conversion.SetOverwriteOutput(false);

                await conversion.Start();

            }
            catch (Exception exception)
            {
                ConverterLog exceptionModel = new ConverterLog()
                {
                    Error = exception.Message.ToString()
                };

                QueueRepository<ConverterLog> queueHandler = new QueueRepository<ConverterLog>();
                queueHandler.QueueMessageDirect(exceptionModel, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(exceptionModel)}";
                log.Error(logText);

            }   
        }
    }
}
