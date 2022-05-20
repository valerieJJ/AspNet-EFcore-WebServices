using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Domain.ViewModels;
using Newtonsoft.Json;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
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
    public IEnumerable<User> Get()
    {
        var users = _context.Users.ToArray();
        return users;
    }
    

    [HttpPost]
    public User register(User user)
    {
        _context.Users.Add(user);
        var count = _context.SaveChanges();

        return user;
    }



    [HttpGet]
    //[Authorize]
    public async Task<AccountViewModel> Account(string username)
    {
        //var username = HttpContext.User.Identity.Name;
        var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();

        if (user == null)
        {
            return null;
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

        return accountViewModel;
    }

    [HttpGet]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
    public async Task<ManagementViewModel> Manage()
    {
        var users = _context.Users.ToList();
        Console.WriteLine("\n\nget users {0}\n\n", users.Count);

        var vm = new ManagementViewModel
        {
            Users = users
        };
        return vm;
    }

    [HttpGet]
    //[TypeFilter(typeof(LoginActionFilter))]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
    public IActionResult Delete(string username)
    {
        var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();
        _context.Users.Remove(user);
        int cnt = _context.SaveChanges();
        Console.WriteLine("changed {0} rows", cnt);
        return RedirectToAction(nameof(Index), "Home", new { });
    }

    [HttpGet]
    //[Authorize]
    public async Task<UserViewModel> Edit(int uid)
    {
        if (uid == null)
        {
            return null;
        }

        User user = _context.Users.Where(_u => _u.Id == uid).FirstOrDefault();

        if (user == null)
        {
            return null;
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
        Console.WriteLine("Get user-id={0} name={1} gender={2}", user.Id, user.UserName, user.Gender);
        return vm;
    }

    [HttpPost]
    //[Authorize]
    public async Task<UserViewModel> Edit(UserViewModel userViewModel)
    {
        string query = Request.Headers["id"];

        var requestbody = Request.Body;

        if (userViewModel == null)
        {
            return null;
        }
        //ViewBag.Gender = userViewModel.Gender;
        //var userid = ViewBag.userid;
        //var title = ViewData["Title"];

        //var username = ViewBag.username;
        Console.WriteLine("Post gender {0}", userViewModel.Gender);

        User userToUpdate = null;
        userToUpdate = _context.Users.Where(u => u.UserName == userViewModel.UserName).FirstOrDefault();
        if (userToUpdate == null)
        {
            Console.WriteLine("user not found");
            return null;
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
        return userViewModel;

    }


    [HttpPost]
    public async Task<User> Login(UserViewModel userViewModel)
    {
        var user = _context.Users
            .Where(_u => _u.UserName == userViewModel.UserName && _u.Password == userViewModel.Password)
            .FirstOrDefault();
        if (user == null)
        {
            Console.WriteLine("User Not Found: name={0}, pwd={1}", userViewModel.UserName, userViewModel.Password);

            return null;
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
                                    , new AuthenticationProperties
                                    {
                                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                                        IsPersistent = false,
                                        AllowRefresh = false
                                    }
                                    );

            var context_user = HttpContext.User;

            Console.WriteLine("distributed.Get: {0}", _distributed.Get("user_key"));

            return user;
        }

    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("Login_User");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index), "Home", new { });
    }



    [HttpGet]
    public UserViewModel Detail(string username)
    {
        var user = _context.Users
            .Where(u => u.UserName == username)
            .FirstOrDefault();
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
        return vm;
    }


    [HttpPost]
    public User Register(RegisterViewModel registerViewModel)
    {
        if (registerViewModel == null || registerViewModel.Password != registerViewModel.ConfirmPassword)
        {
            return null;
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
        return user;
    }



    [HttpPost]
    public User Create(UserViewModel userViewModel)
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
        return user;
    }


}

