
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Neverland.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IConfigurationRoot configuration = builder.Configuration;
//string mysqlconn = "server=localhost;user=root;password=20031230;database=mydb";

//string mysqlconn = "Database=mydb; Data Source=vj-azure-mysql.mysql.database.azure.com; User Id=vj@vj-azure-mysql; Password=1998123Jy";
string mysqlconn = configuration.GetConnectionString("Azure-MySql");
builder.Services.AddDbContext<DataContext>(
    options=>options.UseMySql(mysqlconn, MySqlServerVersion.AutoDetect(mysqlconn))
    //options => options.UseSqlServer("Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True")
    );

//builder.Services.AddScoped<DbContext, DataContext>();

string redisconn = configuration.GetConnectionString("Azure-Redis");
string _instanceName = ""; //实例名称
int _defaultDB = 0; //默认数据库           
builder.Services.AddSingleton(new RedisHelper(redisconn, _instanceName, _defaultDB));


//builder.Services.AddDistributedRedisCache(options =>
//{
//    options.Configuration = redisconn;

//});

//builder.Services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);

//builder.Services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _defaultDB));

//builder.Services.AddDistributedRedisCache(options =>
//{
//    options.Configuration = _connectionString;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
