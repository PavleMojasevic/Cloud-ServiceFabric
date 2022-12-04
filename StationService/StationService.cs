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

namespace StationService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class StationService : StatefulService
    {
        public StationService(StatefulServiceContext context)
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
                new ServiceReplicaListener(context => this.CreateWcfCommunication(context),"StationService")

                };
        }


        private ICommunicationListener CreateWcfCommunication(StatefulServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var end_point_config = context.CodePackageActivationContext.GetEndpoint("StationService");
            int port = end_point_config.Port;
            var scheme = end_point_config.Protocol.ToString();

            string uri = string.Format("net.{0}://{1}:{2}/StationService", scheme, host, port);

            var listener = new WcfCommunicationListener<IStationService>(
                serviceContext: context,
                wcfServiceObject: new StationServiceProvider(this.StateManager),
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
            var trips = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, Trip>>("trips");
            using (var tx = this.StateManager.CreateTransaction())
            {
                for (int i = 0; i < 5; i++)
                { 
                    Trip trip = new Trip
                    {
                        Id = i + 1,
                        AvailableTickets = 10,
                        Depart = DateTime.Now.AddDays(10+i),
                        Price = i*100+1000,
                        Type = TripType.Train,
                        TotalTickets = 10
                    };
                    bool result=await trips.TryAddAsync(tx, trip.Id,trip);
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

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
