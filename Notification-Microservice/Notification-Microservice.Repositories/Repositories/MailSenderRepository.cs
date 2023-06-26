using Microsoft.Extensions.Logging;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Common.Events;
using Notification_Microservice.Repositories.Interfaces;
using NotificationMicroservice.Models;
using System.Net;
using System.Net.Mail;

namespace Notification_Microservice.Repositories.Repositories
{
    public class MailSenderRepository : IMailSenderRepository
    {
        private readonly Lazy<IQueueRepository> _queueErrorRepository;
        private readonly ILogger<MailSenderRepository> _logger;

        public MailSenderRepository(Lazy<IQueueRepository> queueErrorRepository, ILogger<MailSenderRepository> logger)
        {
            _queueErrorRepository = queueErrorRepository;
            _logger = logger;
        }

        public void SendMailToUser(string email, string attachmentFile, Stream attachmentFileStream)
        {
            try
            {
                _logger.LogInformation(LogEvents.MailSenderEvent, LogEvents.MailSenderEventMessage);

                string body = $"<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Here is your cenverted file ({attachmentFile}) </p>";

                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                client.Port = Convert.ToInt32(ProjectConstants.SmtpPort);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Host = ProjectConstants.SmtpHost;
                client.Credentials = new NetworkCredential(ProjectConstants.SmtpMailUsername, ProjectConstants.SmtpMailPassword);
                mail.IsBodyHtml = true;
                mail.To.Add(email);
                mail.From = new MailAddress(ProjectConstants.SmtpMailUsername, ProjectConstants.SmtpMailFrom);
                mail.Subject = ProjectConstants.MailSubject;
                mail.Attachments.Add(new Attachment(attachmentFileStream, attachmentFile));
                mail.Body = body;

                client.Send(mail);

            }
            catch (Exception exception)
            {
                NotificationLog notificationLog = new NotificationLog()
                {
                    Error = exception.Message.ToString(),
                    Date = DateTime.Now
                };
                ErrorLog errorLog = new ErrorLog()
                {
                    notificationLog = notificationLog
                };
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                _logger.LogError(LogEvents.MailSenderEvent, exception.Message.ToString());
            }

        }

    }
}
