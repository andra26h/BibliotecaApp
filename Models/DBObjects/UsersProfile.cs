using System;
using System.Collections.Generic;

namespace BibliotecaApp.Models.DBObjects;

public partial class UsersProfile
{
    public int ProfileId { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public List<BorrowingModel> BorrowingBooks { get; set; }
}
