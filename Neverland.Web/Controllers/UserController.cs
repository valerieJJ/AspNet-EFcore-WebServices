using Microsoft.AspNetCore.Mvc;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.Models;
using Neverland.Web.ViewModels;
using System.Diagnostics;

namespace Neverland.Web.Controllers
{
    public class UserController : Controller
    {
        public readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Account(string username)
        {
            var user = _context.Users.Where(u=>u.UserName == username).FirstOrDefault();
            if(user == null)
            {
                return RedirectToAction(nameof(Error));
            }
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = username,
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
            return View(accountViewModel);
        }

        public IActionResult Management()
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
        public IActionResult Delete(string username)
        {
            var user = _context.Users.Where(u=>u.UserName == username).FirstOrDefault();
            _context.Users.Remove(user);
            int cnt = _context.SaveChanges();
            Console.WriteLine("changed {0} rows", cnt);
            return RedirectToAction(nameof(Management), new { });
        }

        [HttpGet]
        public IActionResult Edit()
        {
        //    if (username == null)
        //    {
        //        return RedirectToAction(nameof(Error));
        //    }
            
        //    User user = await _context.Users.FindAsync(username);

        //    if (user == null)
        //    {
        //        return RedirectToAction(nameof(Error));
        //    }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string username)
        {
            if (username==null)
            {
                return RedirectToAction(nameof (Error));
            }
            var userToUpdate = await _context.Users.FindAsync(username);

            if (await TryUpdateModelAsync<User>(
                  userToUpdate,
                  "",
                  u => u.UserName, u => u.Password, u => u.Email, u => u.Birthday))
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Account), new { username = username });
            }
            return RedirectToAction(nameof(Account), new { username = username });

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userViewModel)
        {
            Console.WriteLine("login: name={0}, pwd={1}", userViewModel.UserName, userViewModel.Password);

            //var user = await _context.Users.FindByNameAsync(userViewModel.UserName);
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
                return RedirectToAction(nameof(Account), new { username = user.UserName});
            }
            
        }


        [HttpGet]
        public IActionResult Detail(string username)
        {
            var user = _context.Users
                .Where(u => u.UserName == username)
                .FirstOrDefault();
                //.OrderBy(u=>u.Id)
                //.LastOrDefault(x => x.UserName==username);
            var vm = new UserViewModel
            {
                Id =  user.Id,
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
            if(registerViewModel == null || registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                return RedirectToAction(nameof(Register), new { });
            }
            var user = new User
            {
                UserName = registerViewModel.UserName,
                Password = registerViewModel.Password,
                Role = Role.user,
                Birthday =registerViewModel.Birthday,
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
            return RedirectToAction(nameof(Detail), new { username = user.UserName});
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
