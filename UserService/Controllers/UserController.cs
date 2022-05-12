using Microsoft.AspNetCore.Mvc;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.ViewModels;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;
    private readonly DataContext _context;

    public UserController(ILogger<UserController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    //[HttpGet(Name = "GetUsers")]
    [HttpGet]
    public IEnumerable<User> Get()
    {
        var users = _context.Users.ToArray();
        return users;
    }

    [HttpGet]
    public User detail(string username)
    {
        var user = _context.Users.Where(u => u.UserName == username).FirstOrDefault();
        return user;
    }


    [HttpPost]
    public User register([FromBody] RegisterViewModel registerViewModel)
    {
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
}

