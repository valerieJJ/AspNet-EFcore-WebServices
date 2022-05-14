using Microsoft.EntityFrameworkCore;
using Neverland.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


IConfigurationRoot configuration = builder.Configuration;
//string mysqlconn = "server=localhost;user=root;password=20031230;database=mydb";
//string mysqlconn = "Database=mydb; Data Source=vj-azure-mysql.mysql.database.azure.com; User Id=vj@vj-azure-mysql; Password=1998123Jy";

//string mysqlconn = configuration.GetConnectionString("Azure-MySql");
string mysqlconn = configuration.GetConnectionString("ECS-MySql");
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql(mysqlconn, MySqlServerVersion.AutoDetect(mysqlconn))
    //options => options.UseSqlServer("Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True")
    );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

