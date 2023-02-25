namespace Notification_Microservice.Repositories.Interfaces
{
    public interface IMailSenderRepository
    {
        void SendMailToUser(string email, string attachmentFile, Stream attachmentFileStream);
    }
}
