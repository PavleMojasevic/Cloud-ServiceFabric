using SendGrid;
using SendGrid.Helpers.Mail;
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
        public async Task SendMail(string mailTo, string content)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("cloudstanica@gmail.com", "Cloud stanica");
            var subject = "";
            var to = new EmailAddress("pavle.mojasevic@gmail.com", mailTo.Split('@')[0]);
            var plainTextContent = content;
            var htmlContent = content;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
        
    }
}
