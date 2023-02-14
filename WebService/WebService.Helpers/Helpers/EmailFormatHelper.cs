using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using WebService.Models;

namespace WebService.Helpers.Helpers
{
    public class EmailFormatHelper
    {
        public void ValidateEMail(string emailAddress)
        {
            emailAddress = emailAddress.Trim();
            MailAddress EmailAddress;
            bool IsEmailValid = false;

            if (emailAddress.StartsWith("."))
            {
                IsEmailValid = false;
            }
            else
            {
                IsEmailValid = MailAddress.TryCreate(emailAddress, out EmailAddress);
            }

            if (!IsEmailValid)
            {
                UploadMp4Response error = new UploadMp4Response();
                error.ErrorMessage = "Email is not valid!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }
    }
}
