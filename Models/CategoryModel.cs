using BibliotecaApp.Models.DBObjects;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "CategoryRequired")]
        [Display(Name = "CategoryName", ResourceType = typeof(resources.Resource))]
        public string CategoryName { get; set; }

        // Lista cărților care aparțin acestei categorii
        public List<BookModel> Books { get; set; } = new List<BookModel>();
    }

}
