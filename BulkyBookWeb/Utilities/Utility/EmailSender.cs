using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
			//var emailToSend = new MimeMessage();
			//emailToSend.From.Add(MailboxAddress.Parse("hoangtrungkien2002@gmail.com"));
			//emailToSend.To.Add(MailboxAddress.Parse(email));
			//emailToSend.Subject = subject;
			//emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text=htmlMessage};

			//// Send email
			//using(var emailClient = new SmtpClient())
			//{
			//    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
			//    emailClient.Authenticate("hoangtrungkien2002@gmail.com", "Kiendzpro25");
			//    emailClient.Send(emailToSend);
			//    emailClient.Disconnect(true);
			//}

			return Task.CompletedTask;
        }
    }
}
