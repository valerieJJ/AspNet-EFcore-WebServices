using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Director
    {
        public Director()
        {
            Movies = new List<Movie>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Country { get; set; }

        public Gender Gender { get; set; }

        public int birthyear { get; set; }

        public List<Movie> Movies { get; set; }
    }
}
