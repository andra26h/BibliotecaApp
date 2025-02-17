using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class ReviewModel
    {
        [Key]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "CommentRequired")]
        [Display(Name = "Comment", ResourceType = typeof(resources.Resource))]
        public string Comment { get; set; }

        [Required(ErrorMessage = "RatingRequired")]
        [Display(Name = "Rating", ResourceType = typeof(resources.Resource))]
        public int Rating { get; set; }

        [Required(ErrorMessage = "ReviewDateRequired")]
        [Display(Name = "ReviewDate", ResourceType = typeof(resources.Resource))]
        public DateTime ReviewDate { get; set; }
        public int UserId { get; set; }

        // Numele utilizatorului care a lăsat recenzia
        [Required(ErrorMessage = "UsernameRequired")]
        [Display(Name = "Username", ResourceType = typeof(resources.Resource))]
        public string Username { get; set; }

        // Informațiile despre carte
        public BookModel Book { get; set; } = new BookModel();
    }
}