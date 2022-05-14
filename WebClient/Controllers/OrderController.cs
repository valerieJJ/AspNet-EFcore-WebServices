using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.Utils; 
using Neverland.Web.ViewModels;
using Newtonsoft.Json;
using Neverland.Web.ViewModels;

namespace Neverland.WebClient.Controllers
{

    //[Area("OrderController")]
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
        [HttpGet]
        public ActionResult Index()
        {
            var orders = _context.Orders.ToListAsync().Result;
            var movies = _context.Movies.ToListAsync().Result;
            var users = _context.Users.ToListAsync().Result;
            foreach( var order in orders)
            {
                order.Movie = movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
                order.User = users.Where(u => u.Id == order.UserId).FirstOrDefault();
            }

            var vm = new OrderIndexViewModel
            {
                Orders = orders
            };


            return View(vm);
        }

        // GET: OrderController/Details/5
        [HttpGet]
        [Route("Order/Details/{id?}")]
        public IActionResult Details(int id)
        {
            //int oid = 
            var order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            var movie = _context.Movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
            var movieDetail = _context.MovieDetails.Where(m => m.MovieId == order.MovieId).FirstOrDefault();
            var user = _context.Users.Where(u => u.Id == order.UserId).FirstOrDefault();

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                Movie = movie,
                MovieDetail = movieDetail,
                User = user,
                Price = order.Price,
                Payment = order.Payment,
                OrderTime = order.OrderTime,
                PaymentType = order.PaymentType
            };
            return View(orderViewModel);
        }

        // GET: OrderController/Create
        //[TypeFilter(typeof(LoginActionFilter))]
        //[JsonResultFilter]
        [HttpGet]
        //[TypeFilter(typeof(LoginActionFilter))]
        public ActionResult Creating(int id)
        {
            Console.WriteLine("create order: movie id={0}", id);
            var userStr = HttpContext.Session.GetString("Login_User");
            if (userStr == null)
            {
                return Redirect("/User/Login");    //重定向
            }
            var user = JsonConvert.DeserializeObject<User>(userStr);
            var movie = _context.Movies.Where(m => m.Id == id).FirstOrDefault();
            var movieDetail = _context.MovieDetails.Where(m => m.MovieId == id).FirstOrDefault();

            //var user = _context.Users.Where(u => u.Id == userr.Id).FirstOrDefault();

            if(movieDetail==null)
            {
                movieDetail = new MovieDetail
                {
                    Description = "details not found",
                    MovieId = movie.Id,
                    Price = 0
                };
            }
            movie.MovieDetail = movieDetail;
            var orderViewModel = new OrderViewModel
            {
                Movie = movie,
                MovieDetail = movieDetail,
                User = user,
                Price = movieDetail.Price,
                Payment = 0.0,
                OrderTime = DateTime.Now.ToLocalTime(),
                PaymentType = PaymentType.wechat
            };
            var order = new Order
            {
                MovieId = movie.Id,
                UserId = user.Id,
                OrderTime = orderViewModel.OrderTime,
                Payment = orderViewModel.Payment,
                PaymentType = orderViewModel.PaymentType,
                Price = movie.MovieDetail.Price

            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            return View(orderViewModel);
        }

        // POST: OrderController/Create
        [HttpPost]
        //[TypeFilter(typeof(LoginActionFilter))]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel orderViewModel)
        {
            Console.WriteLine("post create {0}", orderViewModel.ToString());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Detail(int orderID)
        {
            var order = _context.Orders.Where(o => o.OrderId == orderID).FirstOrDefault();
            var movie = _context.Movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
            var movieDetail = _context.MovieDetails.Where(m => m.MovieId == order.MovieId).FirstOrDefault();

            var orderViewModel = new OrderViewModel
            {
                Movie = order.Movie,
                MovieDetail = order.Movie.MovieDetail,
                User = order.User,
                Price =order.Price,
                Payment = order.Payment,
                OrderTime = order.OrderTime,
                PaymentType = order.PaymentType
            };
            _context.Remove(order);
            _context.SaveChanges();
            return View(orderViewModel);
        }


        // GET: OrderController/Edit/5
        [HttpGet]
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
