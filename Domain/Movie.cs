using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Movie
    {
        public Movie() {
            Actors = new List<Actor>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Language { get; set; }

        public MovieType Type { get; set; }

        public DateTime ShootDate { get; set; }

        public Director Director { get; set; }

        public List<Actor> Actors { get; set; }
    }
}
