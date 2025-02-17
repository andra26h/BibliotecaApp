using System;
using System.Collections.Generic;

namespace BibliotecaApp.Models.DBObjects;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int AvailableCopies { get; set; }

    public string? CoverImage { get; set; }

    public bool IsPopular { get; set; }

    public bool IsRecommended { get; set; }

    public int AuthorId { get; set; }

    public int CategoryId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
