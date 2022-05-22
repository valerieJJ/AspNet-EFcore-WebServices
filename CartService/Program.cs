using CartService.Controllers;
using Consul;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://192.168.43.99:8500");
}));
//builder.WebHost.UseUrls(new[] { "http://*:2010" });

builder.Services.AddHealthChecks();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseConsul();
app.UseHealthChecks("/api/health");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

