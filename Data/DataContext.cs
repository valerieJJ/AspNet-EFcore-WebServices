using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Data
{
    public class DataContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
               "Data Source=localhost;Initial Catalog=MyDB;Integrated Security=True"
                );
        }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<User> Users { get; set; }


        public DbSet<Director> Directors { get; set; }

        public DbSet<Order> Orders { get; set; }


    }
}
