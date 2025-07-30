using System.ComponentModel.DataAnnotations;

namespace Envirovement.Models.Authentication.SignUp
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "User Name Is Reuaired")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email  Is Reuaired")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password  Is Reuaired")]
        public string? Password { get; set; }
    }
}
