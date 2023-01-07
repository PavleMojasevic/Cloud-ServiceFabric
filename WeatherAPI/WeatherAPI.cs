using Common.Interfaces;
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

namespace WeatherAPI
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class WeatherAPI : StatelessService
    {
        public WeatherAPI(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>(1)
            {
                new ServiceInstanceListener(context => this.Create_wcf_communication(context),"WeatherService")
            };
        }


        private ICommunicationListener Create_wcf_communication(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var end_point_config = context.CodePackageActivationContext.GetEndpoint("WeatherService");
            int port = end_point_config.Port;
            var scheme = end_point_config.Protocol.ToString();

            string uri = string.Format("net.{0}://{1}:{2}/WeatherService", scheme, host, port);

            var listener = new WcfCommunicationListener<IWeatherService>(
                serviceContext: context,
                wcfServiceObject: new WeatherServiceProvider(),
                listenerBinding: WcfUtility.CreateTcpListenerBinding(),
                address: new System.ServiceModel.EndpointAddress(uri));
            return listener;
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

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
