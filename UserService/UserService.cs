using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace UserService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class UserService : StatefulService
    {
        public UserService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]{
                new ServiceReplicaListener(context => this.CreateWcfCommunication(context),"UserService")

                };
        }


        private ICommunicationListener CreateWcfCommunication(StatefulServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var end_point_config = context.CodePackageActivationContext.GetEndpoint("UserService");
            int port = end_point_config.Port;
            var scheme = end_point_config.Protocol.ToString();

            string uri = string.Format("net.{0}://{1}:{2}/UserService", scheme, host, port);

            var listener = new WcfCommunicationListener<IUserService>(
                serviceContext: context,
                wcfServiceObject: new UserServiceProvider(this.StateManager),
                listenerBinding: WcfUtility.CreateTcpListenerBinding(),
                address: new System.ServiceModel.EndpointAddress(uri));
            return listener;
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
            UserRepository repository = new UserRepository(StateManager);

            List<UserDB> usersDB = repository.RetrieveAll();

            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (var userDB in usersDB)
                {
                    User user = new User
                    {
                        Username = userDB.Username,
                        BankAccountNumber = userDB.BankAccountNumber,
                        Email = userDB.Email,
                        Password = userDB.Password,
                        Purchases = userDB.Purchases.Select(x => x.GetPurchase()).ToList()
                    };
                    await users.TryAddAsync(tx, user.Username, user);
                }
                tx.CommitAsync();
            }
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }
                await repository.SyncTable();

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
