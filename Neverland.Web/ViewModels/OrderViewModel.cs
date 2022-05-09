using System.ComponentModel.DataAnnotations;
using Neverland.Domain;
namespace Neverland.Web.ViewModels
{
    public class OrderViewModel
    {

        [Key]
        public int OrderId { get; set; }

        [Required]
        public User User { get; set; }

        public String UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

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
