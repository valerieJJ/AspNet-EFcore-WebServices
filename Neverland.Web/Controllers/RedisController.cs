using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Neverland.Web.Controllers
{
    public class RedisController : Controller
    {
        private readonly IDatabase _redis;
        public RedisController(RedisHelper client)
        {
            _redis = client.GetDatabase();
        }

        public IActionResult Index()
        {
            _redis.StringSet("TestRedis", "11111");
            string temp = _redis.StringGet("TestRedis");
            return View();
        }
    }
}
