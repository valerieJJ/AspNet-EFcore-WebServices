using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
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
        [Authorize]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "user")]
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
                Payment = order.Payment,
                OrderTime = order.OrderTime,
                PaymentType = order.PaymentType
            };
            return View(orderViewModel);
        }

        // GET: OrderController/Create
        //[TypeFilter(typeof(LoginActionFilter))]
        //[JsonResultFilter]
        //[TypeFilter(typeof(LoginActionFilter))]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "user")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Creating(int id)
        {
            Console.WriteLine("create order: movie id={0}", id);
            //var userStr = HttpContext.Session.GetString("Login_User");
            //var user = JsonConvert.DeserializeObject<User>(userStr);
            var username = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync().Result;
            //if (userStr == null)
            //{
            //    return Redirect("/User/Login");    //重定向
            //}
            var movie = _context.Movies.Where(m => m.Id == id).FirstOrDefaultAsync().Result;
            var movieDetail = _context.MovieDetails.Where(m => m.MovieId == id).FirstOrDefaultAsync().Result;

            //var user = _context.Users.Where(u => u.Id == userr.Id).FirstOrDefault();

            if (movieDetail==null)
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
                Payment = 0.0,
                OrderTime = DateTime.Now.ToLocalTime(),
                PaymentType = PaymentType.wechat
            };
            return View(orderViewModel);
        }

        // POST: OrderController/Create
        //[TypeFilter(typeof(LoginActionFilter))]
        //[ValidateAntiForgeryToken]
        //public ActionResult Creating(IFormCollection form)

        [HttpPost]
        [Authorize]
        public ActionResult Creating(OrderViewModel orderViewModel)
        {
            var order = new Order
                {
                    MovieId = orderViewModel.Movie.Id,
                    UserId = orderViewModel.User.Id,

                    OrderTime = orderViewModel.OrderTime,
                    
                    Price = orderViewModel.MovieDetail.Price, //_context.MovieDetails.Where(m => m.MovieId == orderViewModel.Movie.Id).FirstOrDefault().Price,
                    
                    Payment = orderViewModel.Payment,
                    PaymentType = orderViewModel.PaymentType //Int32.Parse(form["paymenttype"])
                };

            _context.Add(order);
            _context.SaveChanges();

            //try
            //{
            //    var order = new Order
            //    {
            //        MovieId = Int32.Parse(form["movieid"]),
            //        Payment = Int32.Parse(form["payment"]),
            //        UserId = Int64.Parse(form["uid"]),
            //        OrderTime = DateTime.Parse(form["ordertime"]),

            //        Price = _context.MovieDetails.Where(m => m.MovieId == Int32.Parse(form["movieid"])).FirstOrDefault().Price,
            //        PaymentType = PaymentType.Alipay //Int32.Parse(form["paymenttype"])
            //    };
            //    _context.Add(order);
            //    _context.SaveChanges();
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //}
            return RedirectToAction(nameof(Index));
        }


        // GET: OrderController/Edit/5
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            _context.Remove(_context.Orders.Where(o => o.OrderId == id).FirstOrDefault());
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
