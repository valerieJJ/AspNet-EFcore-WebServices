using Neverland.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neverland.Web.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name ="User ID")]
        public long Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)] 
        public string? Email { get; set; }

        [Display(Name = "Role")]
        public Role? Role { get; set; }

        [Display(Name = "Birthday")]
        [Column(TypeName = "Date")]
        public DateTime? Birthday { get; set; }

        //[Display(Name = "ConfirmPassword")]
        //public string ConfirmPassword { get; set; }

        //public bool RememberMe { get; set; }


    }
}
