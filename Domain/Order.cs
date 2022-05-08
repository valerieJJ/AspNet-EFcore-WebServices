using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neverland.Domain
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Movie Movie { get; set; }    

        [Required]
        public DateTime OrderTime { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Payment { get; set; }
    }
}
