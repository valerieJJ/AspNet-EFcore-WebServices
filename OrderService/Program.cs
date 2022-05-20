using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using OrderService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddSingleton<IConsulClient>(c => new ConsulClient(
//    cc =>
//    {
//        cc.Address = new Uri("http://localhost:8500");
//    }));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHealthChecks();


IConfigurationRoot configuration = builder.Configuration;
string mysqlconn = configuration.GetConnectionString("ECS-MySql"); //"Azure-MySql"
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql(mysqlconn, MySqlServerVersion.AutoDetect(mysqlconn))
    //options => options.UseSqlServer("Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True")
    );


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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseConsul();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("api/health",() =>
{
    return new OkResult();
});

//app.MapHealthChecks("/health");

app.Run();

