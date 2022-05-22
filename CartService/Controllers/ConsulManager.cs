using System;
using Consul;

namespace CartService.Controllers
{
    public static class ConsulManager
    {


        public static IConsulClient _consulClient = null;

        public static void UseConsul(this WebApplication app)
        {
            IConfiguration config = app.Configuration;
            _consulClient = new ConsulClient(cc =>
            {
                cc.Address = new Uri("http://192.168.43.99:8500");
                cc.Datacenter = "cart-sevice";
            });

            RegisterServer(config);
        }
        private static void RegisterServer(IConfiguration config)
        {
            string ip = string.IsNullOrWhiteSpace(config["ip"])? "http://localhost:8500/": config["ip"];
            int port = int.Parse(config["port"]);
            int weight = string.IsNullOrWhiteSpace(config["weight"]) ? 1 : int.Parse(config["weight"]);

            string consulGroup = config["Consul:ConsulGroup"];
            var serviceID = $"{consulGroup} {ip} {port}";
            _consulClient.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serviceID,
                Name = consulGroup,
                Address = ip,
                Port = port

                //Tags = new string[] { weight.ToString() },
                //Check = new AgentServiceCheck()
                //{
                //    Interval = TimeSpan.FromSeconds(10),
                //    HTTP = $"http://{ip}:{port}/api/health",
                //    Timeout = TimeSpan.FromSeconds(50),
                //    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(100)
                //}
        });
        }

        private static void RegisterServer2(IConfiguration config)
        {
            string consulGroup = config["Consul:ConsulGroup"]; //"OrderService";
            string ip = config["ip"]; //config["Consul:ip"]; //"192.168.43.99";
            int port = Convert.ToInt32(config["port"]); //Convert.ToInt32(config["Consul:port"]);

            var serviceID = $"{consulGroup} {ip} {port}";

            var check = new AgentServiceCheck()
            {
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://{ip}:{port}/api/health",
                Timeout = TimeSpan.FromSeconds(500),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(6)
            };

            var register = new AgentServiceRegistration
            {
                Check = check,
                Address = ip,
                ID = serviceID,
                Name = consulGroup,
                Port = port
            };


            _consulClient.Agent.ServiceRegister(register);
        }
    }
}

