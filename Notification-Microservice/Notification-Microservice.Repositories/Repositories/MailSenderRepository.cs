using Newtonsoft.Json;
using Notification_Microservice.Common.Constants;
using Notification_Microservice.Repositories.Interfaces;
using NotificationMicroservice.Models;
using System.Net;
using System.Net.Mail;

namespace Notification_Microservice.Repositories.Repositories
{
    public class MailSenderRepository : IMailSenderRepository
    {
        private readonly Lazy<IQueueRepository> _queueErrorRepository;

        public MailSenderRepository(Lazy<IQueueRepository> queueErrorRepository)
        {
            _queueErrorRepository = queueErrorRepository;
        }

        public void SendMailToUser(string email, string attachmentFile, Stream attachmentFileStream)
        {
            try
            {
                string body = $"<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Here is your cenverted file ({attachmentFile}) </p>";

                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                client.Port = Convert.ToInt32(ProjectConstants.SmtpPort);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Host = ProjectConstants.SmtpHost;
                client.Credentials = new NetworkCredential(ProjectConstants.SmtpMailFrom, ProjectConstants.SmtpMailPassword);
                mail.IsBodyHtml = true;
                mail.To.Add(email);
                mail.From = new MailAddress(ProjectConstants.SmtpMailFrom, ProjectConstants.SmtpMailUsername);
                mail.Subject = ProjectConstants.MailSubject;
                mail.Attachments.Add(new Attachment(attachmentFileStream, attachmentFile));
                mail.Body = body;

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
                _queueErrorRepository.Value.QueueMessageDirect(errorLog, ProjectConstants.ErrorLogsServiceQueueName, ProjectConstants.ErrorLogsServiceExchangeName, ProjectConstants.ErrorLogsServiceRoutingKey);

                string logText = $"Exception: {JsonConvert.SerializeObject(errorLog)}";
            }

        }

    }
}
