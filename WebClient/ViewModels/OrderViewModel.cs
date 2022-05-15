using System.ComponentModel.DataAnnotations;
using Neverland.Domain;
namespace Neverland.Web.ViewModels
{
    public class OrderViewModel
    {

        public int OrderId { get; set; }

        //[Required]
        public User User { get; set; }

        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        //[Required]
        public Movie Movie { get; set; }

        public MovieDetail MovieDetail { get; set; }

        //[Required]
        public DateTime OrderTime { get; set; }

        //[Required]
        //public double Price { get; set; }

        //[Required]
        public double Payment { get; set; }

        //[Required]
        public PaymentType PaymentType { get; set; }


    }
}
