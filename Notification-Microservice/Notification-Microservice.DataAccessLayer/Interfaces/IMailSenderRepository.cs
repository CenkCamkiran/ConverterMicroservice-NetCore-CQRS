namespace Interfaces
{
    public interface IMailSenderRepository
    {
        void SendMailToUser(string email, string attachmentFile, Stream attachmentFileStream);
    }
}
