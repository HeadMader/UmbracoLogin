using System.ComponentModel.DataAnnotations;

namespace UmbracoLogin.Models
{
    public class SignupFormModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\s*(?:\+?(\d{1,3}))?[- (]*(\d{3})[- )]*(\d{3})[- ]*(\d{4})(?: *[x/#]{1}(\d+))?\s*$", ErrorMessage = "Invalid mobile number.")]
        [Phone(ErrorMessage = "Invalid mobile number.")]
        public required string Mobile { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int CountryID { get; set; }

        public int aID { get; set; }
        public string? SignupIP { get; set; }
    }
}
