using Configuration;
using DataAccess.Interfaces;
using Helper.Interfaces;
using Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace Helper.Helpers
{
    public class MailSenderRepository : IMailSenderRepository
    {
        private readonly Lazy<IQueueRepository<ErrorLog>> _queueErrorRepository;
        private readonly Lazy<ILog4NetRepository> _log4NetRepository;
        private EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler(); //Get Host, port, email smtp server 

        public MailSenderRepository(Lazy<IQueueRepository<ErrorLog>> queueErrorRepository, Lazy<ILog4NetRepository> log4NetRepository)
        {
            _queueErrorRepository = queueErrorRepository;
            _log4NetRepository = log4NetRepository;
        }

        public void SendMailToUser(string email, string AttachmentFile, Stream attachmentFileStream)
        {
            SmtpConfiguration smtpConfiguration = envVariablesHandler.GetSmtpEnvVariables();

            try
            {
                string body = $"<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Here is your cenverted file ({AttachmentFile}) </p>";

                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                client.Port = Convert.ToInt32(smtpConfiguration.SmtpPort);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl= true;    
                client.Host = smtpConfiguration.SmtpHost;
                client.Credentials = new NetworkCredential(smtpConfiguration.SmtpMailFrom, smtpConfiguration.SmtpMailPassword);
                mail.IsBodyHtml = true;
                mail.To.Add(email);
                //mail.CC.Add(new MailAddress(""));
                mail.From = (new MailAddress(smtpConfiguration.SmtpMailFrom, smtpConfiguration.SmtpMailUsername));
                mail.Subject = "About Your Converted File";
                mail.Attachments.Add(new Attachment(attachmentFileStream, AttachmentFile));
                mail.Body = body;
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                client.Send(mail);

            }
            catch (Exception exception)
            {
                NotificationLog notificationLog = new NotificationLog()
                {
                    Error = exception.ToString(),
                    Date = DateTime.Now
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    notificationLog = notificationLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, "errorlogs", "log_exchange.direct", "error_log");

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
                _log4NetRepository.Value.Error(logText);
            }

        }

    }
}
