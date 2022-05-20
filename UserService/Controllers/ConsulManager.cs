using System;
using Consul;

namespace UserService.Controllers
{
	public static class ConsulManager
	{
        public static IConsulClient _consulClient = null;

        public static void UseConsul(this WebApplication app)
        {

            IConfiguration config = app.Configuration;
            _consulClient = new ConsulClient(cc =>
            {
                cc.Address = new Uri("http://localhost:8500");
            });

            RegisterServer(config);
        }

        private static void RegisterServer(IConfiguration config)
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

