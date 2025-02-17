using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class UserModel
    {
        // Identificatorul utilizatorului
        [Key]
        public int UserId { get; set; }

        // Email-ul utilizatorului
        [Required(ErrorMessage = "EmailRequired")]
        [Display(Name = "Email", ResourceType = typeof(resources.Resource))]
        [EmailAddress(ErrorMessage = "EmailError")]
        public string Email { get; set; }

        // Parola utilizatorului
        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password", ResourceType = typeof(resources.Resource))]
        [MinLength(8, ErrorMessage = "MinLengthPassword")]
        [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}", ErrorMessage = "RegExError")]
        public string Password { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [Compare("Password", ErrorMessage = "PasswordMatch")]
        public string ConfirmPassword { get; set; }

        // Numele de utilizator
        [Required(ErrorMessage = "UsernameRequired")]
        [Display(Name = "Username", ResourceType = typeof(resources.Resource))]
        public string Username { get; set; }

        // Numele real al utilizatorului (opțional)
        [Required(ErrorMessage = "FirstNameRequired")]
        [Display(Name = "FirstName", ResourceType = typeof(resources.Resource))]
        public string? FirstName { get; set; }

        // Prenumele utilizatorului (opțional)
        [Required(ErrorMessage = "LastNameRequired")]
        [Display(Name = "LastName", ResourceType = typeof(resources.Resource))]
        public string? LastName { get; set; }

        // Rolul utilizatorului (ID-ul rolului)
        public int RoleId { get; set; }
    }
}
