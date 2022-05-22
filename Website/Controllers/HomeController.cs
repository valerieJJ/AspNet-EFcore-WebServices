using System.Diagnostics;
using Consul;
using Interface;
using Microsoft.AspNetCore.Mvc;
using Neverland.Domain.ViewModels;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieService _movieService;
    private static IConsulClient _consulClient = null;


    public HomeController(ILogger<HomeController> logger, IMovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
        _consulClient = new ConsulClient(cc =>
        {
            cc.Address = new Uri("http://192.168.43.99:8500");
            cc.Datacenter = "cart-sevice";
        });
    }

    //public IActionResult Index()
    //{
    //    string getMid = _movieService.QueryMovie(2);
    //    Console.WriteLine($"\n\n_movieService.QueryMovie: {getMid}\n\n");
    //    int mid = 2;
    //    string url = $"http://192.168.43.99:2077/api/Movie/QueryMovie/{mid}"; //"http://192.168.43.99:2007/api/Movie/Index";
    //    var content = InvokeApi(url);
    //    var res = JsonConvert.DeserializeObject<MovieViewModel>(content);
    //    base.ViewBag.mvm = res;
    //    return View();
    //}


    public IActionResult Index()
    {
        int mid = 1;
        string url = $"http://MovieService/api/Movie/QueryMovie/{mid}";
        //string url = "http://MovieService/api/Movie/Index";

        var response = _consulClient.Agent.Services().Result.Response;

        Uri uri = new Uri(url);
        string groupName = uri.Host;
        AgentService agentService = null;
        var dictionary = response.Where(s => s.Value.Service.Equals(groupName, StringComparison.OrdinalIgnoreCase)).ToArray();
        {
            agentService = dictionary[0].Value;
        }

        url = $"{uri.Scheme}://{agentService.Address}:{agentService.Port}{uri.PathAndQuery}";
        string content = InvokeApi(url);
        var res = JsonConvert.DeserializeObject<MovieViewModel>(content);
        base.ViewBag.mvm = res;
        Console.WriteLine($"\n\nThis is {url} invoked.\n\n");
        return View();
    }

    private string InvokeApi(string url)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri(url);
            var result = httpClient.SendAsync(message).Result;
            string content = result.Content.ReadAsStringAsync().Result;

            return content;
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

