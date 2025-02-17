using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class BorrowingModel
    {
        [Key]
        public int BorrowingId { get; set; }

        [Required(ErrorMessage = "BorrowDateRequired")]
        [Display(Name = "BorrowDate", ResourceType = typeof(resources.Resource))]
        [DataType(DataType.DateTime)]
        public DateTime BorrowDate { get; set; }

        //[Required(ErrorMessage = "CategoryNameRequired")]
        [Display(Name = "ReturnDate", ResourceType = typeof(resources.Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "StatusRequired")]
        [Display(Name = "Status", ResourceType = typeof(resources.Resource))]
        public string Status { get; set; }

        // Detaliile utilizatorului care a împrumutat cartea
        public UserModel User { get; set; } = new UserModel();

        // Detaliile cărții împrumutate
        public BookModel Book { get; set; } = new BookModel();
    }
}