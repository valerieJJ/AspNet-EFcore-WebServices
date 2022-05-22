using Consul;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using UserService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


IConfigurationRoot configuration = builder.Configuration;
string mysqlconn = configuration.GetConnectionString("ECS-MySql"); //"Azure-MySql"
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql(mysqlconn, MySqlServerVersion.AutoDetect(mysqlconn)) //options => options.UseSqlServer("Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True")
    );

//builder.Services.AddSingleton<IConsulClient>(c => new ConsulClient(
//    cc =>
//    {
//        cc.Address = new Uri("http://localhost:8500");
//    }));

string redisconn = configuration.GetConnectionString("ECS-Redis");//"Azure-Redis"
builder.Services.AddDistributedRedisCache(optioins =>
{
    optioins.Configuration = redisconn;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true; // 设为httponly
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 鉴权配置
{
    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
    {
        // 如果没有找到用户信息-->鉴权失败-->授权也失败了-->跳转到指定的Action
        option.LoginPath = "/api/User/Login";
        option.AccessDeniedPath = "/api/User/Login";
    });
}
//string mysqlconn = "server=localhost;user=root;password=20031230;database=mydb";
//string mysqlconn = "Database=mydb; Data Source=vj-azure-mysql.mysql.database.azure.com; User Id=vj@vj-azure-mysql; Password=1998123Jy";
//string mysqlconn = configuration.GetConnectionString("Azure-MySql");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseConsul();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

