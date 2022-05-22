using Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IConfigurationRoot configuration = builder.Configuration;
//string mysqlconn = "server=localhost;user=root;password=20031230;database=mydb
//string mysqlconn = configuration.GetConnectionString("Azure-MySql");
string mysqlconn = configuration.GetConnectionString("ECS-MySql");
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql(mysqlconn, MySqlServerVersion.AutoDetect(mysqlconn))
    //options => options.UseSqlServer("Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True")
    );

//builder.Services.AddScoped<DbContext, DataContext>();


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

//builder.Services.AddEndpointsApiExplorer();

// 鉴权配置
{
    builder.Services.AddAuthentication(option =>
    {
        // 选择基于cookie的方式鉴权
        //option.DefaultAuthenticateScheme = "MyCookieAuthenticationScheme";
        //option.DefaultChallengeScheme = "MyCookieAuthenticationScheme";
        //option.DefaultSignInScheme = "MyCookieAuthenticationScheme";
        //option.DefaultForbidScheme = "MyCookieAuthenticationScheme";
        //option.DefaultSignOutScheme = "MyCookieAuthenticationScheme";
        option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
        {
            // 如果没有找到用户信息-->鉴权失败-->授权也失败了-->跳转到指定的Action
            option.LoginPath = "/User/Login";
            option.AccessDeniedPath = "/Home/AccessDenied";
            //option.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        });
}





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();// 鉴权，获取用户信息
app.UseAuthorization(); // 授权，判断是否放行请求

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

