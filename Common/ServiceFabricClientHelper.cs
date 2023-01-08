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
            for (int i = 0; i < partitionNumber; i++)
            {
                ServicePartitionClient<WcfCommunicationClient<IBankService>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<IBankService>>(
                    new WcfCommunicationClientFactory<IBankService>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/BankService"),
                    new ServicePartitionKey(index % partitionNumber));
                index++;
                return servicePartitionClient;
            }
            return null;
        }
        public static async Task<ServicePartitionClient<WcfCommunicationClient<IUserService>>> GetUserService()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/UserService"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();
            int index = 0;
            for (int i = 0; i < partitionNumber; i++)
            {
                ServicePartitionClient<WcfCommunicationClient<IUserService>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<IUserService>>(
                    new WcfCommunicationClientFactory<IUserService>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/UserService"),
                    new ServicePartitionKey(index % partitionNumber));
                index++;
                return servicePartitionClient;
            }

            return null;
        }
        public static async Task<ServicePartitionClient<WcfCommunicationClient<IStationService>>> GetStationService()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/StationService"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;
            for (int i = 0; i < partitionNumber; i++)
            {

                ServicePartitionClient<WcfCommunicationClient<IStationService>> servicePartitionClient = new
                    ServicePartitionClient<WcfCommunicationClient<IStationService>>(
                        new WcfCommunicationClientFactory<IStationService>(clientBinding: binding),
                        new Uri("fabric:/CloudProjekat/StationService"),
                        new ServicePartitionKey(index % partitionNumber));
                index++;
                return servicePartitionClient;
            }
            return null;

        }
        public static async Task<ServicePartitionClient<WcfCommunicationClient<IWeatherService>>> GetWeatherService()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/WeatherAPI"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;
            for (int i = 0; i < partitionNumber; i++)
            {


                ServicePartitionClient<WcfCommunicationClient<IWeatherService>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<IWeatherService>>(
                    new WcfCommunicationClientFactory<IWeatherService>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/WeatherAPI"),
                        new ServicePartitionKey(index % partitionNumber));
                index++;
                return servicePartitionClient;
            }
            return null;

        }

        public static async Task<ServicePartitionClient<WcfCommunicationClient<ITransactionCoordinator>>> GetTransactionCoordinator()
        {
            FabricClient fabricClient = new FabricClient();

            int partitionNumber = (await fabricClient.QueryManager.GetApplicationListAsync(new Uri("fabric:/CloudProjekat/TransactionCoordinator"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;
            for (int i = 0; i < partitionNumber; i++)
            {
                ServicePartitionClient<WcfCommunicationClient<ITransactionCoordinator>> servicePartitionClient = new
                ServicePartitionClient<WcfCommunicationClient<ITransactionCoordinator>>(
                    new WcfCommunicationClientFactory<ITransactionCoordinator>(clientBinding: binding),
                    new Uri("fabric:/CloudProjekat/TransactionCoordinator"),
                        new ServicePartitionKey(index % partitionNumber));
                index++;
                return servicePartitionClient;
            }
            return null;

        }
    }
}
