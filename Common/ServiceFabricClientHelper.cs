using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Client;
using Common.Interfaces;

namespace Common
{
    public static class ServiceFabricClientHelper
    {
        public static async Task<ServicePartitionClient<WcfCommunicationClient<IBankService>>> GetBankService()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/BankService"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;
            //TODO: for
            ServicePartitionClient<WcfCommunicationClient<IBankService>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<IBankService>>(
                    new WcfCommunicationClientFactory<IBankService>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/BankService"),
                    new ServicePartitionKey(0));
             
            return servicePartitionClient;


        }
        public static async Task<ServicePartitionClient<WcfCommunicationClient<IUserService>>> GetUserService()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/UserService"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;
            //TODO: for
            ServicePartitionClient<WcfCommunicationClient<IUserService>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<IUserService>>(
                    new WcfCommunicationClientFactory<IUserService>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/UserService"),
                    new ServicePartitionKey(0));
             
            return servicePartitionClient;


        }
    }
}
