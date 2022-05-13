
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Neverland.Web;

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


string redisconn = configuration.GetConnectionString("Azure-Redis");
builder.Services.AddDistributedRedisCache(optioins =>
{
    optioins.Configuration = redisconn;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true; // 设为httponly
});


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

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
