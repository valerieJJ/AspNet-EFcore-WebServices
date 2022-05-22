//using System;
//using Consul;
//using Ocelot.ServiceDiscovery.Providers;
//using Ocelot.Values;

//namespace CartService.Controllers
//{
//    public class ConsulServiceProvider : IServiceDiscoveryProvider
//    {
//        public Task<List<Service>> Get()
//        {
//            var consuleClient = new ConsulClient(consulConfig =>
//            {
//                consulConfig.Address = new Uri("http://192.168.43.99t:8500");
//            });

//            var queryResult = consuleClient.Health.Service("Service", string.Empty, true);
//            var result = new List<string>();
//            foreach (var serviceEntry in queryResult.Response)
//            {
//                result.Add(serviceEntry.Service.Address + ":" + serviceEntry.Service.Port);
//            }
//            return result;
//        }

//        public async Task<List<string>> GetServicesAsync()
//        {
//            var consuleClient = new ConsulClient(consulConfig =>
//            {
//                consulConfig.Address = new Uri("http://localhost:8500");
//            });
//            var queryResult = await consuleClient.Health.Service("Service", string.Empty, true);
//            var result = new List<string>();
//            foreach (var serviceEntry in queryResult.Response)
//            {
//                result.Add(serviceEntry.Service.Address + ":" + serviceEntry.Service.Port);
//            }
//            return result;
//        }
//    }
//}


