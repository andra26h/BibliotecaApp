using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class AuthorModel
    {
        [Key]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "FirstNameRequired")]
        [Display(Name = "FirstName", ResourceType = typeof(resources.Resource))]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        [Display(Name = "LastName", ResourceType = typeof(resources.Resource))]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}"; // Unificarea numelui

        // Lista cărților scrise de acest autor
        public List<BookModel> Books { get; set; } = new List<BookModel>();
    }
}
