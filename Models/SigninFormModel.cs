using System.ComponentModel.DataAnnotations;

namespace UmbracoLogin.Models
{
    public class SignInFormModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
