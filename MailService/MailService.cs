using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.MsmqIntegration;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using UniqueId = MailKit.UniqueId;
using Common.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Common.Interfaces;
using Common;

namespace MailService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class MailService : StatelessService
    {
        public MailService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await GetMails();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8859") };

        uint lastMailId = 0;
        private async Task GetMails()
        {


            string imapServer = "imap.gmail.com";
            int imapPort = 993;
            string imapUsername = "cloudstanica@gmail.com";
            string imapPassword = "vftxtnebqpwxypfg";

            ImapClient client = new ImapClient();

            client.Connect(imapServer, imapPort, SecureSocketOptions.SslOnConnect);

            client.Authenticate(imapUsername, imapPassword);


            IMailFolder inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            IList<UniqueId> uids = inbox.Search(SearchQuery.SubjectContains("Rezervacija"));

            foreach (UniqueId uid in uids.Where(x => x.Id > lastMailId))
            {
                MimeMessage message = inbox.GetMessage(uid);
                if (message.Subject.ToLower().Trim() == "rezervacija")
                {
                    try
                    {

                        Purchase purchase = await ParsePurchase(message.TextBody, message.From.ToString().Split('<', '>')[1]);

                        var transactionCoordinator = await ServiceFabricClientHelper.GetTransactionCoordinator();
                        if (await transactionCoordinator.InvokeWithRetryAsync(x => x.Channel.MakePurchaseFromMail(purchase)))
                        {
                            var stringPayload = JsonConvert.SerializeObject(purchase);
                            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                            await _httpClient.PostAsync("api/Reservation", httpContent);
                            lastMailId = uid.Id;
                        }

                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }

        }

        private async Task<Purchase> ParsePurchase(string content, string email)
        {
            string[] parts = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Purchase purchase = new Purchase();
            purchase.Username = email;
            purchase.Date = DateTime.Now;
            foreach (var row in parts)
            {
                string[] item = row.Split(';');
                purchase.TripIds.Add(Convert.ToInt64(item[0]));
                purchase.Quantities.Add(Convert.ToInt32(item[1]));
            }
            return purchase;
        }
    }
}
