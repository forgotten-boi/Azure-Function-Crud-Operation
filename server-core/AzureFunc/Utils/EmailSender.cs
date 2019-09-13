using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunc
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string email, string subject, string message)
        {
            //try
            //{
                string toEmail = string.IsNullOrEmpty(email)
                                 ? FunctionsSettings.ToEmail
                                 : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(FunctionsSettings.UsernameEmail, "Technical Lead")
                };
                mail.To.Add(new MailAddress(toEmail));
                //mail.CC.Add(new MailAddress(FunctionsSettings.CcEmail));
                mail.From = new MailAddress(FunctionsSettings.UsernameEmail, "Technical Lead");

                mail.Subject = "FilmDb - " + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //mail.Attachments.Add(new Attachment(Server.MapPath("~/myimage.jpg")));

                using (SmtpClient smtp = new SmtpClient(FunctionsSettings.PrimaryDomain, FunctionsSettings.PrimaryPort))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(FunctionsSettings.UsernameEmail, FunctionsSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    //do something here
            //}
        }
    }

  
}
