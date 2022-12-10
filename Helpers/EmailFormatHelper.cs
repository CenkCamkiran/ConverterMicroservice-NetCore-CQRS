using Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace Helpers
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
                WebServiceErrors error = new WebServiceErrors();
                error.ErrorMessage = "Email is not valid!";
                error.ErrorCode = (int)HttpStatusCode.BadRequest;

                throw new WebServiceException(JsonConvert.SerializeObject(error));
            }
        }
    }
}
