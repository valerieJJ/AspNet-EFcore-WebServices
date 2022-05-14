using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neverland.Domain;
using Microsoft.Extensions.Logging;

namespace Neverland.Data
{
    public class DataContext : DbContext
    {
        //使用web项目时
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Write your code here to configure the context
        }


        // 不适用web项目时
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLoggerFactory(ConsoleLoggerFactory)
        //        .UseSqlServer(
        //            "Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True"
        //        );
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Order>().HasKey(e => e.OrderId);
            modelBuilder.Entity<Favorite>().HasKey(e => new { e.UserId, e.MovieId });
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<Movie>()
                .HasOne(e => e.MovieDetail)
                .WithOne(e => e.Movie)
                .HasForeignKey<MovieDetail>(d => d.MovieId);

            modelBuilder.Entity<Movie>()
                .HasOne(e => e.MovieScore)
                .WithOne(e => e.Movie)
                .HasForeignKey<MovieScore>(d => d.MovieId);

        }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Director> Directors { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<MovieScore> MovieScores { get; set; }


        public DbSet<MovieDetail> MovieDetails { get; set; }

        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information)
            .AddConsole();
        });
    }
}
