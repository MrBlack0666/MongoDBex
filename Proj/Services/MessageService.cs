using mongoDB.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Services
{
    public class MessageService : IMessageService
    {
        private readonly string GLOBAL_EMAIL = "songdb.boss@gmail.com";
        private readonly string password = "haslo1234!";

        public bool SendEmail(string email, string message)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(GLOBAL_EMAIL);
                mail.To.Add(new MailAddress(email));
                mail.Subject = "UserDB - Wiadomość od administratora";
                mail.Body = message;
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(GLOBAL_EMAIL, password);
                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
