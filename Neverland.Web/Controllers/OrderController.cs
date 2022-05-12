using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.Utils;
using Neverland.Web.ViewModels;
using Newtonsoft.Json;

namespace Neverland.Web.Controllers
{
    public class OrderController : Controller
    {

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
        public ActionResult Index(int id)
        {
            return View("OrderIndex");
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrderController/Create
        [TypeFilter(typeof(LoginActionFilter))]
        [JsonResultFilter]
        public ActionResult Create(int id)
        {
            Console.WriteLine("create order: movie id={0}", id);
            
            var userStr = HttpContext.Session.GetString("Login_User");
            User user = JsonConvert.DeserializeObject<User>(userStr);
            Movie movie = _context.Movies.Where(m=>m.Id==id).FirstOrDefault();
            movie.MovieDetail = _context.MovieDetails.Where(m=>m.MovieId==id).FirstOrDefault();

            OrderViewModel orderViewModel = new OrderViewModel
            {
                Movie = movie,
                User = user,
                UserName = user.UserName,
                Price = movie.MovieDetail.Price
                //,OrderTime = DateTime.Now
            };

            return View(orderViewModel);
        }

        // POST: OrderController/Create
        [HttpPost]
        [TypeFilter(typeof(LoginActionFilter))]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
