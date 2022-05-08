using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neverland.Domain
{
    public class Actor
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Country { get; set; }


        public Gender? Gender { get; set; }


        [Column(TypeName = "date")]
        public DateTime? birthday { get; set; }
    }
}
