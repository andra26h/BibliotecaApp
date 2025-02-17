using System;
using System.Collections.Generic;

namespace BibliotecaApp.Models.DBObjects;

public partial class Review
{
    public int ReviewId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime ReviewDate { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
