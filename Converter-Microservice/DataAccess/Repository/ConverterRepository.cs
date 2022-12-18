using DataAccess.Interfaces;
using Models;
using Xabe.FFmpeg;

namespace DataAccess.Repository
{
    public class ConverterRepository: IConverterRepository
    {
        private Log4NetRepository log = new Log4NetRepository();
        private LoggingOtherRepository logOtherRepository = new LoggingOtherRepository();

        public async Task ConvertMP4_to_MP3(string ConvertFromFilePath, string ConvertToFilePath)
        {
            try
            {
                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(ConvertFromFilePath, ConvertToFilePath);
                conversion.SetOverwriteOutput(false);

                await conversion.Start();

                ConverterLog converterLog = new ConverterLog()
                {
                    Info = "Conversion finished!"
                };
                OtherLog otherLog = new OtherLog()
                {
                    converterLog = converterLog
                };

                await logOtherRepository.LogConverterOther(otherLog);

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

                await logOtherRepository.LogConverterError(errorLog);

            }
        }
    }
}
