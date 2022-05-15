
using System;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.ViewModels;
using Newtonsoft.Json;
using Neverland.WebClient.Models;
using Neverland.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Neverland.WebClient.Controllers
{
    public class UserController : Controller
    {
        public readonly DataContext _context;
        private readonly IDistributedCache _distributed;
        private readonly ILogger<UserController> _logger;

        public UserController(DataContext context, IDistributedCache distributed, ILogger<UserController> logger)
        {
            _context = context;
            _distributed = distributed;
            _logger = logger;
        }

        [HttpGet]
        //[TypeFilter(typeof(AsyncLoginActionFilter))]
        //[TypeFilter(typeof(LoginActionFilter))]
        //[TypeFilter(typeof(LoginActionFilter))] //或：[ServiceFilter(typeof(UserActionFilter))]
        //[UserResourceFilter]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        [Authorize]
        public async Task<IActionResult> Account()
        {
            //string userStr = HttpContext.Session.GetString("Login_User");
            //if (userStr == null)
            //{
            //    return RedirectToAction(nameof(Login));
            //}
            //var user = JsonConvert.DeserializeObject<User>(userStr);
            //_logger.LogInformation($"\n\n\nsession-id: {HttpContext.Session.Id}\n\n\n");

            var username = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                Birthday = user.Birthday,
                Email = user.Email,
                Gender = user.Gender,
                Role = user.Role
            };
            var accountViewModel = new AccountViewModel
            {
                UserViewModel = userViewModel
            };

            //ViewBag.User = user;
            return View(accountViewModel);
        }

        [HttpGet]
        //[TypeFilter(typeof(AsyncLoginActionFilter))]  //[TypeFilter(typeof(LoginActionFilter))]   //[TypeFilter(typeof(PermissionActionFilter))]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> Manage()
        {
            var users = _context.Users.ToList();
            Console.WriteLine("\n\nget users {0}\n\n", users.Count);

            var vm = new ManagementViewModel
            {
                Users = users
            };
            return View(vm);
        }

        [HttpGet]
        //[TypeFilter(typeof(LoginActionFilter))]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        public IActionResult Delete(string username)
        {
            var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();
            _context.Users.Remove(user);
            int cnt = _context.SaveChanges();
            Console.WriteLine("changed {0} rows", cnt);
            return RedirectToAction(nameof(Index), "Home", new { });
        }

        [HttpGet]
        //[TypeFilter(typeof(LoginActionFilter))]
        [Authorize]
        public async Task<IActionResult> Edit(int uid)
        {
            if (uid == null)
            {
                return RedirectToAction(nameof(Error));
            }

            User user = _context.Users.Where(_u => _u.Id == uid).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction(nameof(Error));
            }

            UserViewModel vm = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Role = user.Role
            };
            ViewBag.Gender = user.Gender;
            Console.WriteLine("Get user-id={0} name={1} gender={2}", user.Id, user.UserName, user.Gender);
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            string query = Request.Headers["id"];

            var requestbody = Request.Body;

            if (userViewModel == null)
            {
                return RedirectToAction(nameof(Error));
            }
            ViewBag.Gender = userViewModel.Gender;
            var userid = ViewBag.userid;
            var title = ViewData["Title"];

            var username = ViewBag.username;
            Console.WriteLine("Post gender {0}", userViewModel.Gender);

            User userToUpdate = null;
            userToUpdate = _context.Users.Where(u => u.UserName == userViewModel.UserName).FirstOrDefault();
            if (userToUpdate == null)
            {
                Console.WriteLine("user not found");
                return RedirectToAction(nameof(Register));
            }


            {
                userToUpdate.UserName = (userViewModel.UserName == null) ? userToUpdate.UserName : userViewModel.UserName;
                userToUpdate.Email = (userViewModel.Email == null) ? userToUpdate.Email : userViewModel.Email;
                userToUpdate.Gender = (userViewModel.Gender == null) ? userToUpdate.Gender : userViewModel.Gender;
                userToUpdate.Birthday = (userViewModel.Birthday == null) ? userToUpdate.Birthday : userViewModel.Birthday;
            }


            _context.Update(userToUpdate);
            //if (await TryUpdateModelAsync<User>(
            //      userToUpdate,
            //      "",
            //      u => u.UserName, u => u.Password, u => u.Email, u => u.Birthday))
            //{
            //    await _context.SaveChangesAsync();
            //    Console.WriteLine("Edit success");
            //    return RedirectToAction(nameof(Account), new { username = userViewModel.UserName });
            //}

            _context.SaveChanges();
            return RedirectToAction(nameof(Account), new { username = userViewModel.UserName });

        }

        [HttpGet]
        public IActionResult Login()
        {
            string userStr = HttpContext.User.Identity.Name;
            string userStr2 = HttpContext.Session.GetString("Login_User");
            _logger.LogInformation($"\n\nusername(redis) = {userStr2}, username(httpcontext) = {userStr} \n\n");
            if (string.IsNullOrEmpty(userStr2) || string.IsNullOrEmpty(userStr))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Account));
            }
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userViewModel)
        {
            Console.WriteLine("login: name={0}, pwd={1}", userViewModel.UserName, userViewModel.Password);

            var user = _context.Users
                .Where(_u => _u.UserName == userViewModel.UserName && _u.Password == userViewModel.Password)
                .FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("User Not Found: name={0}, pwd={1}", userViewModel.UserName, userViewModel.Password);

                return RedirectToAction(nameof(Login), new { });
            }
            else
            {
                string user_json = JsonConvert.SerializeObject(user);
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3000)); ;//设置过期时间
                byte[] bytes = Encoding.UTF8.GetBytes(user_json);
                _distributed.Set("user_" + user.UserName, bytes, options);


                HttpContext.Session.SetString("Login_User", user_json);

                //创建一个身份认证
                var claims = new List<Claim>()
                {
                    // Claim 是对被验证主体特征的一种表述
                    new Claim("UserId", user.Id.ToString())
                    ,new Claim(ClaimTypes.Sid, user.Id.ToString())
                    ,new Claim(ClaimTypes.Role, ((Role)user.Role).ToString())
                    //,new Claim(ClaimTypes.Role, "user")
                    ,new Claim(ClaimTypes.Name, user.UserName)
                    //,new Claim(ClaimTypes.Email, (user.Email),
                    //new Claim("password", user.Password),

                };
                //ClaimsIdentity的持有者就是 ClaimsPrincipal
                var identity = new ClaimsIdentity(claims, "LoginIdentity"); 
                //  一个ClaimsPrincipal可以持有多个ClaimsIdentity，就比如一个人既持有驾照，又持有护照.
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                                        , userPrincipal
                                        , new AuthenticationProperties{
                                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                                            IsPersistent = false,
                                            AllowRefresh = false}
                                        );

                var context_user = HttpContext.User;

                Console.WriteLine("distributed.Get: {0}", _distributed.Get("user_key"));

                return RedirectToAction(nameof(Account), new { username = user.UserName });
            }

        }

        [HttpGet]
        //[TypeFilter(typeof(LoginActionFilter))]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Login_User");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index), "Home", new { });
        }


        [HttpGet]
        [TypeFilter(typeof(LoginActionFilter))]
        public IActionResult Detail(string username)
        {
            var user = _context.Users
                .Where(u => u.UserName == username)
                .FirstOrDefault();
            //.OrderBy(u=>u.Id)
            //.LastOrDefault(x => x.UserName==username);
            var vm = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Email = user.Email,
                Role = user.Role
            };
            return View(vm);
        }


        [HttpGet]
        public IActionResult Register() { return View(); }

        [HttpPost]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (registerViewModel == null || registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                return RedirectToAction(nameof(Register), new { });
            }
            var user = new User
            {
                UserName = registerViewModel.UserName,
                Password = registerViewModel.Password,
                Role = Role.user,
                Birthday = registerViewModel.Birthday,
                Email = registerViewModel.Email,
                Gender = registerViewModel.Gender
            };

            _context.Users.Add(user);
            var count = _context.SaveChanges();
            return RedirectToAction(nameof(Account), new { username = user.UserName });
        }


        [HttpGet]
        public IActionResult Create() { return View(); }

        [HttpPost]
        public IActionResult Create(UserViewModel userViewModel)
        {
            var user = new User
            {
                UserName = userViewModel.UserName,
                Password = userViewModel.Password,
                Role = Role.user,
                Birthday = userViewModel.Birthday,
                Email = userViewModel.Email
            };

            _context.Users.Add(user);
            var count = _context.SaveChanges();
            return RedirectToAction(nameof(Detail), new { username = user.UserName });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

