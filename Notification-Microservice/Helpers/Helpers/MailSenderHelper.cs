using Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helper.Helpers
{
    public class MailSenderHelper: IMailSenderHelper
    {
        //public void SendMailToDepartments(string ID, string Message, BsonDocument receipt)
        //{

        //    if (receipt == null)
        //    {

        //        var body = "";

        //        body = "<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Manim verisinde ID alanı (_id) </p>"
        //                + $"<p style=\"color: rgb(17, 0, 255); font-size: 16px; font-weight: 600;\">{ID}</p>"
        //                + "<p style=\"color: rgb(0, 0, 0); font-size: 16px;\"> olan Manim verisi Manim Receipt In servisinde hataya düştü.</p>"
        //                + $"<p style=\"color: rgb(255, 0, 0); font-size: 16px; font-weight: 600;\" > Hata içeriği: {Message}</p>";

        //        MailMessage mail = new MailMessage();
        //        SmtpClient client = new SmtpClient();

        //        client.Port = 25;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.UseDefaultCredentials = false;
        //        client.Host = "uzmanposta.gridtelekom.com";
        //        client.Credentials = new NetworkCredential("reminder@gridtelekom.com", "Rem54321!");
        //        mail.IsBodyHtml = true;
        //        mail.To.Add("yazilim@gridtelekom.com");
        //        mail.CC.Add(new MailAddress("satisdestek@gridtelekom.com"));
        //        mail.CC.Add(new MailAddress("muhasebe@gridtelekom.com"));
        //        mail.From = (new MailAddress("reminder@gridtelekom.com", "GridTelekom Reminder"));
        //        mail.Subject = "Manim Receipt In Service Report (High Priority)";
        //        mail.Body = body;

        //        try
        //        {
        //            client.Send(mail);
        //        }

        //        catch (Exception ex)
        //        {
        //            string directory = Directory.GetCurrentDirectory();
        //            string path = directory + "\\ManimReceiptInOutMailSender.txt"; // Exceptions.txt dosyasının oluşturulacağı path
        //            if (!File.Exists(path))
        //            {
        //                StreamWriter DosyaCreate = File.CreateText(path); // Exceptions.txt oluşturuldu, eğer yoksa
        //                DosyaCreate.Close();
        //            }
        //            StreamWriter Dosya = File.AppendText(path);
        //            Dosya.WriteLine(ex + ". " + DateTime.Now + "." + "\n"); // Dosya.WriteLine ile dosyaya exeption ı yazdık
        //            Dosya.Close();
        //            //Console.WriteLine("Hata Oluştu : {0}", ex.Message);
        //        }

        //    }
        //    else
        //    {

        //        string JSONDirectory = Directory.GetCurrentDirectory();
        //        string JSONPath = JSONDirectory + $"\\{receipt["_id"]}.json"; // Exceptions.txt dosyasının oluşturulacağı path
        //        if (!File.Exists(JSONPath))
        //        {
        //            StreamWriter DosyaCreate = File.CreateText(JSONPath); // Exceptions.txt oluşturuldu, eğer yoksa
        //            DosyaCreate.Close();
        //        }

        //        System.IO.File.WriteAllText(JSONPath, receipt.ToJson());

        //        var body = "";

        //        body = "<p style=\"color: rgb(0, 0, 0); font-size: 16px;\">Manim verisinde ID alanı (_id) </p>"
        //                   + $"<p style=\"color: rgb(17, 0, 255); font-size: 16px; font-weight: 600;\">{ID}</p>"
        //                   + "<p style=\"color: rgb(0, 0, 0); font-size: 16px;\"> olan Manim verisi Manim Receipt In servisinde hataya düştü.</p>"
        //                   + $"<p style=\"color: rgb(255, 0, 0); font-size: 16px;\"> Hata içeriği: {Message}</p>";

        //        MailMessage mail = new MailMessage();
        //        SmtpClient client = new SmtpClient();

        //        client.Port = 25;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.UseDefaultCredentials = false;
        //        client.Host = "uzmanposta.gridtelekom.com";
        //        client.Credentials = new NetworkCredential("reminder@gridtelekom.com", "Rem54321!");
        //        mail.IsBodyHtml = true;
        //        mail.To.Add("yazilim@gridtelekom.com");
        //        mail.CC.Add(new MailAddress("satisdestek@gridtelekom.com"));
        //        mail.CC.Add(new MailAddress("muhasebe@gridtelekom.com"));
        //        mail.From = (new MailAddress("reminder@gridtelekom.com", "GridTelekom Reminder"));
        //        mail.Subject = "Manim Receipt In Service Report (High Priority)";
        //        mail.Attachments.Add(new Attachment(JSONPath));
        //        mail.Body = body;

        //        try
        //        {
        //            client.Send(mail);
        //        }

        //        catch (Exception ex)
        //        {
        //            string directory = Directory.GetCurrentDirectory();
        //            string path = directory + "\\ManimReceiptInOutMailSender.txt"; // Exceptions.txt dosyasının oluşturulacağı path
        //            if (!File.Exists(path))
        //            {
        //                StreamWriter DosyaCreate = File.CreateText(path); // Exceptions.txt oluşturuldu, eğer yoksa
        //                DosyaCreate.Close();
        //            }
        //            StreamWriter Dosya = File.AppendText(path);
        //            Dosya.WriteLine(ex + ". " + DateTime.Now + "." + "\n"); // Dosya.WriteLine ile dosyaya exeption ı yazdık
        //            Dosya.Close();
        //            //Console.WriteLine("Hata Oluştu : {0}", ex.Message);
        //        }

        //    }

        //}

        //public void SendMailToYazilim(string Message)
        //{

        //    var body = "";

        //    body = "<p style=\"color: rgb(0, 0, 0); font-size: 16px;\"> MongoDB'ye bağlanırken hata meydana geldi. 37.77.18.18:27017'deki MongoDB'ye erişilemiyor.</p>"
        //            + $"<p style=\"color: rgb(255, 0, 0); font-size: 16px; font-weight: 600;\" > Hata içeriği: {Message}</p>";

        //    MailMessage mail = new MailMessage();
        //    SmtpClient client = new SmtpClient();

        //    client.Port = 25;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    client.Host = "uzmanposta.gridtelekom.com";
        //    client.Credentials = new NetworkCredential("reminder@gridtelekom.com", "Rem54321!");
        //    mail.IsBodyHtml = true;
        //    mail.To.Add("yazilim@gridtelekom.com");
        //    //mail.CC.Add(new MailAddress("sistem@gridtelekom.com"));
        //    //mail.CC.Add(new MailAddress("satisdestek@gridtelekom.com"));
        //    //mail.CC.Add(new MailAddress("muhasebe@gridtelekom.com"));
        //    mail.From = (new MailAddress("reminder@gridtelekom.com", "GridTelekom Reminder"));
        //    mail.Subject = "Manim Receipt In Service Report (High Priority)";
        //    mail.Body = body;

        //    try
        //    {
        //        client.Send(mail);
        //    }

        //    catch (Exception ex)
        //    {
        //        string directory = Directory.GetCurrentDirectory();
        //        string path = directory + "\\ManimReceiptInOutMailSender.txt"; // Exceptions.txt dosyasının oluşturulacağı path
        //        if (!File.Exists(path))
        //        {
        //            StreamWriter DosyaCreate = File.CreateText(path); // Exceptions.txt oluşturuldu, eğer yoksa
        //            DosyaCreate.Close();
        //        }
        //        StreamWriter Dosya = File.AppendText(path);
        //        Dosya.WriteLine(ex + ". " + DateTime.Now + "." + "\n"); // Dosya.WriteLine ile dosyaya exeption ı yazdık
        //        Dosya.Close();
        //        //Console.WriteLine("Hata Oluştu : {0}", ex.Message);
        //    }

        //}
    }
}
