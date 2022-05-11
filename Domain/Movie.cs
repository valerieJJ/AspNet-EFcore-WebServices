using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neverland.Domain
{
    public class Movie
    {
        public Movie() {
            Actors = new List<Actor>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Language { get; set; }

        [MaxLength(100)]
        public MovieType? Type { get; set; }


        [Column(TypeName="date")]
        public DateTime? ShootDate { get; set; }

        public Director? Director { get; set; }

        public List<Actor>? Actors { get; set; }

        public MovieDetail? MovieDetail { get; set; }

        public MovieScore MovieScore { get; set; }
    }
}
