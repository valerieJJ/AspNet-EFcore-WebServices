using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Neverland.Web.Models;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace Neverland.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
          
        
        public IActionResult Privacy()
        {
            /*string redisconn = "vj-azure.redis.cache.windows.net:6380,password=D8GfauBoTrW6L41wdNXEpSPIRqEYKNNv4AzCaDJx8TA=,ssl=True,abortConnect=False";*/

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}