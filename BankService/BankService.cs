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
using System.Threading;
using System.Threading.Tasks;

namespace BankService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BankService : StatefulService
    {
        public BankService(StatefulServiceContext context)
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
                new ServiceReplicaListener(context =>
                {
                    return new  WcfCommunicationListener<IBankService>(context,
                            new BankServiceProvider(this.StateManager),
                            WcfUtility.CreateTcpListenerBinding(),
                            "BankService"

                        );
                },"BankService")

                };
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

            var accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            BankRepository bankRepository = new BankRepository(StateManager);
            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (BankAccountDB bank in bankRepository.RetrieveAll())
                {
                    BankAccount bankAccount = new BankAccount { AccountNumber = bank.AccountNumber, AvailableFunds = bank.AvailableFunds, Username = bank.Username };
                    await  accounts.AddOrUpdateAsync(tx,bankAccount.AccountNumber, bankAccount, (k, v) => v);
                }
                await tx.CommitAsync();
            }
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    await tx.CommitAsync();
                }

                await bankRepository.SyncTable();
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
