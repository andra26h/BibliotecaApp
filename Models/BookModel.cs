using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class BookModel
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "BookTitleRequired")]
        [Display(Name = "BookTitle", ResourceType = typeof(resources.Resource))]
        public string Title { get; set; } = string.Empty;

        //[Required(ErrorMessage = "BookTitleRequired")]
        [Display(Name = "Description", ResourceType = typeof(resources.Resource))]
        public string? Description { get; set; }

        [Required(ErrorMessage = "AvailableCopiesRequired")]
        [Display(Name = "AvailableCopies", ResourceType = typeof(resources.Resource))]
        public int AvailableCopies { get; set; }

        //[Required(ErrorMessage = "BookTitleRequired")]
        [Display(Name = "CoverImage", ResourceType = typeof(resources.Resource))]
        public string? CoverImage { get; set; }

        //[Required(ErrorMessage = "BookTitleRequired")]
        [Display(Name = "IsPopular", ResourceType = typeof(resources.Resource))]
        public bool IsPopular { get; set; }

        //[Required(ErrorMessage = "BookTitleRequired")]
        [Display(Name = "IsRecommended", ResourceType = typeof(resources.Resource))]
        public bool IsRecommended { get; set; }

        [Required(ErrorMessage = "AuthorNameRequired")]
        [Display(Name = "AuthorName", ResourceType = typeof(resources.Resource))]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "CategoryNameRequired")]
        [Display(Name = "CategoryName", ResourceType = typeof(resources.Resource))]
        public string CategoryName { get; set; }

        // Cheile externe
        public int AuthorId { get; set; }

        public int CategoryId { get; set; }

/*        // Obiecte asociate pentru autor și categorie
        public AuthorModel Author { get; set; } = new AuthorModel();

        public CategoryModel Category { get; set; } = new CategoryModel();*/

        // Recenzii legate de carte
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
        public bool AlreadyBorrowed { get; set; }
    }
}
