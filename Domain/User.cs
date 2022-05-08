using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neverland.Domain
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Role? Role { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }


        public Gender? Gender { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? Birthday { get; set; }

    }
}