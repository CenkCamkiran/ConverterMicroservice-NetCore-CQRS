namespace Helper.Interfaces
{
    public interface IMailSenderRepository
    {
        Task<bool> SendMailToUser(string email, string AttachmentFile);
    }
}
