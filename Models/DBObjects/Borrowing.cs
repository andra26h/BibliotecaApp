using System;
using System.Collections.Generic;

namespace BibliotecaApp.Models.DBObjects;

public partial class Borrowing
{
    public int BorrowingId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public int UserId { get; set; }

    public int BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
