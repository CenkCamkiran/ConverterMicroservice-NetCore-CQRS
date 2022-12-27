namespace Helper.Interfaces
{
    public interface IMailSenderRepository
    {
        void SendMailToUser(string email, string AttachmentFile, Stream attachmentFileStream);
    }
}
