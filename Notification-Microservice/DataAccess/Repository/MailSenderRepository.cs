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

        public Task<bool> SendMailToUser(string email, string AttachmentFilePath)
        {
            try
            {
                string body = $"<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Here is your cenverted file ({Path.GetFileName(AttachmentFilePath)}) </p>";

                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "";
                client.Credentials = new NetworkCredential("", "");
                mail.IsBodyHtml = true;
                mail.To.Add(email);
                //mail.CC.Add(new MailAddress(""));
                mail.From = (new MailAddress("mail@mail.com", "Name Surname"));
                mail.Subject = "";
                mail.Attachments.Add(new Attachment(AttachmentFilePath));
                mail.Body = body;

                var senderTask = client.SendMailAsync(mail);

                if (senderTask.Wait(60000) & senderTask.IsCompletedSuccessfully)
                    return Task.FromResult(true);

                return Task.FromResult(false);

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

                return Task.FromResult(false);
            }

        }

    }
}
