using Common.Interfaces;
using System.ServiceModel;

namespace Common
{
    public static class WcfHelper
    {
        public static IUserService GetUserService()
        { 
            return GetService<IUserService>("UserService", 53852);
        }
        public static IWeatherService GetWeatherService()
        { 
            return GetService<IWeatherService>("WeatherService", 53854);
        }
        public static ITransactionCoordinator GetTransactionCoordinator()
        { 
            return GetService<ITransactionCoordinator>("TransactionCoordinator", 53850);
        }
        public static IStationService GetStationService()
        { 
            return GetService<IStationService>("StationService", 53851);   
        }
        private static T GetService<T>(string serviceName, int port)
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            var endpoint = new EndpointAddress($"net.tcp://localhost:{port}/{serviceName}");
            ChannelFactory<T> channelFactory = new ChannelFactory<T>(binding, endpoint);
            var proxy = channelFactory.CreateChannel();
            return proxy;
        }
    }
}
