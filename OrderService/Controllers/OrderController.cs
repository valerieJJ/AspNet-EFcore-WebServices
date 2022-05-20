using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrderController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


    public readonly DataContext _context;
    private readonly IDistributedCache _distributed;
    private readonly ILogger<OrderController> _logger;

    public OrderController(DataContext context, IDistributedCache distributed, ILogger<OrderController> logger)
    {
        _context = context;
        _distributed = distributed;
        _logger = logger;
    }


    // GET: OrderController
    [HttpGet]
    public async Task<IEnumerable<Order>> Index()
    {
        var orders = _context.Orders.ToListAsync().Result;
        var movies = _context.Movies.ToListAsync().Result;
        var users = _context.Users.ToListAsync().Result;
        foreach (var order in orders)
        {
            order.Movie = movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
            order.User = users.Where(u => u.Id == order.UserId).FirstOrDefault();
        }

        return orders;
    }

    // GET: OrderController/Details/5
    [HttpGet]
    //[Route("Order/Details/{id?}")]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "user")]
    public Order Details(int id)
    {
        var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
        var movie = _context.Movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
        var user = _context.Users.Where(u => u.Id == order.UserId).FirstOrDefault();

        order.Movie = movie;
        order.User = user;
        
        return order;
    }

    [HttpPost]
    public ActionResult Creating(Order order)
    {
        _context.Add(order);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // POST: OrderController/Edit/5
    [HttpPost]
    public Order Edit(Order order)
    {
        return order;
    }

    // GET: OrderController/Delete/5
    [HttpGet]
    public ActionResult Delete(int id)
    {
        _context.Remove(_context.Orders.Where(o => o.OrderId == id).FirstOrDefault());
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}

