namespace Helper.Interfaces
{
    public interface IMailSenderRepository
    {
        Task SendMailToUser(string email, string AttachmentFile);
    }
}
