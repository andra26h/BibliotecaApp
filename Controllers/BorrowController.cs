using BibliotecaApp.Models;
using BibliotecaApp.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BibliotecaApp.Models.DBObjects;
using BibliotecaApp.resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaApp.Controllers
{
    [Authorize(Roles = "Admin, ")]
    public class BorrowController : Controller
    {
        private readonly BorrowRepository borrowRepository;
        private readonly BookRepository bookRepository;
        private readonly UserRepository userRepository; // Presupun că ai un UserRepository pentru a căuta utilizatorul în baza de date
        private readonly AuthorRepository authorRepository;
        private readonly CategoryRepository categoryRepository;

        public BorrowController()
        {
            borrowRepository = new BorrowRepository();
            bookRepository = new BookRepository();
            userRepository = new UserRepository(); // Injectăm UserRepository
            authorRepository = new AuthorRepository();
            categoryRepository = new CategoryRepository();
        }

        // Metoda care deschide modalul de împrumut
        [HttpGet]
        public IActionResult Index(int bookId)
        {
            // Obține cartea din repository pe baza ID-ului
            var book = bookRepository.GetBookById(bookId);

            // Verifică dacă există o sesiune activă pentru email-ul utilizatorului
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Dacă email-ul nu este salvat în sesiune, redirecționează utilizatorul către pagina de login
                return RedirectToAction("Index", "Login");
            }

            // Obține utilizatorul pe baza email-ului
            var user = userRepository.GetUserByEmail(userEmail);  // Metodă pentru a obține detaliile utilizatorului după email

            // Preia autorul cărții pe baza AuthorId
            var author = authorRepository.GetAuthorById(book.AuthorId);  // Presupunând că ai această metodă în repository

            // Crează un model de împrumut și adaugă detaliile cărții și utilizatorului
            var borrowingModel = new BorrowingModel
            {
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7),
                Status = "Pending",
                Book = book,
                User = user,  // Populează cu informațiile utilizatorului
            };

            // Adaugă informațiile despre autor în BookModel (sau BorrowingModel, dacă vrei să le afișezi direct acolo)
            borrowingModel.Book.AuthorName = $"{author.FirstName} {author.LastName}"; // Concatenează FirstName și LastName

            // Încarcă categoriile și autorii pentru a le utiliza în view, dacă este necesar
            LoadAuthorsAndCategories();

            // Returnează partial view-ul cu modelul de împrumut
            return PartialView("Borrow", borrowingModel);
        }


        // Metoda care scade o copie disponibilă și adaugă un împrumut
        [HttpPost]
        public async Task<IActionResult> Borrow(BorrowingModel borrowingModel)
        {
            // Obținem cartea după ID
            var book = await bookRepository.GetBookByIdAsync(borrowingModel.Book.BookId);

            // Dacă cartea nu există, returnăm eroare
            if (book == null)
            {
                return NotFound();
            }

            // Obținem email-ul utilizatorului din sesiune
            string userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Dacă nu avem email-ul în sesiune, returnăm un mesaj de eroare
                TempData["Message"] = Resource.ErrorOnBorrow;
                return RedirectToAction("Index", "Books");
            }

            // Căutăm utilizatorul în baza de date pe baza email-ului
            var user = await userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                TempData["Message"] = Resource.UserNotFound;
                return RedirectToAction("Index", "Books");
            }

            // Verificăm dacă utilizatorul a împrumutat deja cartea
            var activeBorrow = await borrowRepository.GetActiveBorrowingAsync(user.UserId, borrowingModel.Book.BookId);

            if (activeBorrow != null)
            {
                // Dacă utilizatorul are deja un împrumut activ pentru această carte, afișăm un mesaj
                TempData["Message"] = Resource.AlreadyBorrowedError;
                return RedirectToAction("Index", "Books");
            }

            // Setăm utilizatorul în modelul de împrumut
            borrowingModel.User = new UserModel
            {
                UserId = user.UserId,  // Asociem UserId-ul din utilizatorul curent
                Username = user.Username // Setăm și UserName (sau alte informații relevante)
            };

            // Verificăm dacă există copii disponibile pentru împrumut
            if (book.AvailableCopies > 0)
            {
                // Scădem o copie disponibilă
                book.AvailableCopies -= 1;

                // Actualizăm cartea în baza de date
                await bookRepository.UpdateBookAsync(book);

                // Adăugăm împrumutul
                borrowingModel.BorrowDate = DateTime.Now;
                borrowingModel.Status = "Borrowed";
                await borrowRepository.AddBorrowAsync(borrowingModel);

                // Mesaj de succes
                TempData["Message"] = Resource.BorrowSuccess;
            }
            else
            {
                // Mesaj de eroare pentru lipsa copiilor disponibile
                TempData["Message"] = Resource.NoAvailableCopiesError;
            }

            // Redirect la pagina de cărți
            return RedirectToAction("Index", "Books");
        }

        public void LoadAuthorsAndCategories()
        {
            // Obținem autorii și categoriile folosind repository-urile
            var authors = authorRepository.getAllAuthors();
            var categories = categoryRepository.GetAllCategories();

            // Mapează autorii în SelectListItem
            ViewBag.Authors = authors.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.FirstName + " " + a.LastName // Concatenăm FirstName și LastName
            }).ToList();

            // Mapează categoriile în SelectListItem
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
        }
    }
}
