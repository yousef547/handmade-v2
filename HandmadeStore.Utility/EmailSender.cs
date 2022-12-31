using Mailjet.Client;
using Mailjet.Client.Resources;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailJetSettings _emailJetSettings;
        public EmailSender(IOptions<EmailJetSettings> emailJetSettings)
        {
            _emailJetSettings = emailJetSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //configure email
            //var emailToSend = new MimeMessage();
            //emailToSend.From.Add(MailboxAddress.Parse("info@store.com"));
            //emailToSend.To.Add(MailboxAddress.Parse(email));
            //emailToSend.Subject = subject;
            //emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            ////send email
            //using (var emailClient = new SmtpClient())
            //{
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("youaefmhamed481@gmail.com", "myrmlxsladsvkgog");
            //    emailClient.Send(emailToSend);
            //    emailClient.Disconnect(true);
            //}
            //return Task.CompletedTask;

            MailjetClient client = new (_emailJetSettings.ApiKey, _emailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", _emailJetSettings.SenderEmail},
        {"Name", "Handmode"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }
        }
       }
      }, {
       "Subject",
       subject
      }, {
       "HTMLPart",
       htmlMessage
      }
     }
             });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
