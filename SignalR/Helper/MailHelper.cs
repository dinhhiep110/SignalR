using System.Net.Mail;
using System.Net;

namespace SignalR.Helper
{
    public class MailHelper
    {
        public async static void SendMail(string email, string body,string subject)
        {

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "hiepddhe153656@fpt.edu.vn",  // replace with valid value
                    Password = "*********"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                var message = new MailMessage();
                message.To.Add(email);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("shop@cart.com");
                await smtp.SendMailAsync(message);
            }
        }
        public async static void SendMail(string email, string body,string subject, Dictionary<Stream, string> files)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "hiepddhe153656@fpt.edu.vn",  // replace with valid value
                    Password = "*********"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                var message = new MailMessage();
                message.To.Add(email);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("shop@cart.com");


                try
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine(file.Value);
                        message.Attachments.Add(new Attachment(file.Key, file.Value));
                    }
                    await smtp.SendMailAsync(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
