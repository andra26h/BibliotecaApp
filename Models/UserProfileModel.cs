using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BibliotecaApp.Models
{
    public class UserProfileModel
    {
        [Key]
        public int ProfileId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "AddressRequired")]
        [Display(Name = "Address", ResourceType = typeof(resources.Resource))]
        public string Address { get; set; }

        [Required(ErrorMessage = "DateOfBirthRequired")]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resources.Resource))]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "PhoneNumberRequired")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(resources.Resource))]
        public string PhoneNumber { get; set; }

        // Detalii despre utilizator
        public UserModel User { get; set; } = new UserModel();
    }
}
