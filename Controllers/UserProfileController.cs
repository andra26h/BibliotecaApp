using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Controllers
{
    [Authorize(Roles = "Admin, ")]
    public class UserProfileController : Controller
    {
        private readonly UserRepository userRepository;
        private readonly BorrowRepository borrowRepository;
        private readonly ApplicationDbContext dbContext;

        // Injectarea dependențelor
        public UserProfileController(UserRepository userRepository, BorrowRepository borrowRepository, ApplicationDbContext dbContext)
        {
            this.userRepository = userRepository;
            this.borrowRepository = borrowRepository;
            this.dbContext = dbContext;
        }

        // Metoda pentru a vizualiza profilul utilizatorului
        public async Task<IActionResult> Index()
        {
            // Preia email-ul utilizatorului din sesiune
            string userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Message"] = "You must be logged in to view your profile.";
                return RedirectToAction("Index", "Books");
            }

            // Obține utilizatorul pe baza email-ului (inclusiv împrumuturile)
            var user = await dbContext.Users
                .Include(u => u.Borrowings) // Include împrumuturile utilizatorului
                .ThenInclude(b => b.Book) // Include informațiile despre carte
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("Index", "Books");
            }

            var userprofile = await dbContext.UsersProfiles.FirstOrDefaultAsync(up => up.UserId == user.UserId);

            // Extrage împrumuturile active (sau toate împrumuturile) ale utilizatorului
            var borrowings = user.Borrowings.Where(b => b.Status == "Active").ToList();

            // Creează lista de cărți împrumutate, inclusiv numele autorului
            var borrowedBooks = new List<BorrowingModel>();

            foreach (var borrowing in borrowings)
            {
                // Preia autorul pe baza AuthorId
                var author = await dbContext.Authors
                    .FirstOrDefaultAsync(a => a.AuthorId == borrowing.Book.AuthorId);

                // Creează modelul BorrowingModel
                var borrowingModel = new BorrowingModel
                {
                    BorrowingId = borrowing.BorrowingId,
                    BorrowDate = borrowing.BorrowDate,
                    ReturnDate = borrowing.ReturnDate,
                    Status = borrowing.Status,
                    Book = new BookModel
                    {
                        BookId = borrowing.Book.BookId,
                        Title = borrowing.Book.Title,
                        AuthorName = $"{author.FirstName} {author.LastName}", // Concatenează FirstName și LastName
                        CoverImage = borrowing.Book.CoverImage,
                        Description = borrowing.Book.Description
                    }
                };

                borrowedBooks.Add(borrowingModel);
            }

            // Creează modelul pentru vizualizarea profilului
            var userProfileModel = new UserProfileModel
            {
                User = new UserModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleId = user.RoleId
                },
                Address = userprofile?.Address,
                PhoneNumber = userprofile?.PhoneNumber,
                DateOfBirth = userprofile?.DateOfBirth ?? DateTime.MinValue
            };

            // Folosește ViewBag pentru a transmite lista de cărți împrumutate
            ViewBag.BorrowedBooks = borrowedBooks;

            return View(userProfileModel);
        }


    }
}
