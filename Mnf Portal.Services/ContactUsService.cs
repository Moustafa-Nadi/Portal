using MailKit.Net.Smtp;
using MimeKit;
using Mnf_Portal.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnf_Portal.Services
{
    public class ContactUsService : IEmailService
    {
        private const string Gmail = "mazenkhtab123@gmail.com"; // fake email to send emails to (admin  الجامعه يعني)
        private const string AppPassword = "zjcj qoqb aeup eswf"; // will moving to json later

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Gmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(Gmail, AppPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
