using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailService
{
    public  class MailServiceProvider
    {
       /* static async Task SendMail()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("cloudstanica@gmail.com", "Example User");
            var subject = "Sending with SeSSSndGrid is Fun";
            var to = new EmailAddress("pavle.mojasevic@gmail.com", "Example User");
            var plainTextContent = "AAAAAAAAand easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }*/
    }
}
