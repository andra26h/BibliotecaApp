using BibliotecaApp.Models;
using BibliotecaApp.Repository;
using BibliotecaApp.resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookRepository bookRepository;
        private readonly AuthorRepository authorRepository;
        private readonly CategoryRepository categoryRepository;
        private readonly UserRepository userRepository;
        private readonly BorrowRepository borrowRepository;

        public BooksController ()
        {
            bookRepository = new BookRepository();
            authorRepository = new AuthorRepository();
            categoryRepository = new CategoryRepository();
            userRepository = new UserRepository();
            borrowRepository = new BorrowRepository();
        }
        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Index", "Login");
            }

            var user =  userRepository.GetUserByEmail(userEmail);
            var books = bookRepository.GetAllBooks(); // Obține toate cărțile
            var model = new List<BookModel>();

            foreach (var book in books)
            {
                var alreadyBorrowed = await borrowRepository.HasUserBorrowedBookAsync(user.UserId, book.BookId);

                model.Add(new BookModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    AvailableCopies = book.AvailableCopies,
                    AuthorName = book.AuthorName,
                    CategoryName = book.CategoryName,
                    AlreadyBorrowed = alreadyBorrowed
                });
            }

            return View(model);
        }
        public IActionResult Create()
        {
            LoadAuthorsAndCategories();

            return PartialView("Create");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(BookModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Adăugăm cartea (logica de salvare în bază de date)
                   bookRepository.AddBook(model);

                    // Dacă totul este OK, returnăm un răspuns de succes
                    return Json(new { success = true, message = Resource.BookAddSuccess });
                }
                catch (Exception ex)
                {
                    // În cazul unei erori, returnăm un mesaj de eroare
                    return Json(new { success = false, message = Resource.BookAddError });
                }
            }

            // Dacă modelul nu este valid, returnăm un mesaj de eroare
            return Json(new { success = false, message = Resource.BookAddValidationError });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var book = bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();  // Dacă nu se găsește cartea, returnează eroare 404
            }
            LoadAuthorsAndCategories();

            return PartialView("Edit",book);  // Afișează formularul pentru editarea cărții
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, BookModel book)
        {
            if (id != book.BookId)
            {
                return BadRequest();  // Dacă id-ul nu se potrivește, returnează eroare 400
            }

            if (ModelState.IsValid)
            {
                bookRepository.UpdateBook(book);  // Actualizează informațiile cărții în baza de date
                return Json(new { success = true, message = Resource.UpdateBookSuccess }); // După actualizare, redirecționează la lista de cărți
            }

            return Json(new { success = false, message = @Resource.UpdateBookError, data = book });   // Dacă există erori, rămâne pe pagina de editare
        }

        public IActionResult Details(int id)
        {
            var book = bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();  // Dacă nu se găsește cartea, returnează eroare 404
            }
            return View(book);  // Afișează detaliile cărții
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int bookId)
        {
            // Căutăm cartea în baza de date
            var book = bookRepository.GetBookById(bookId); // Adaptează funcția GetBookById din repository pentru a căuta cartea

            if (book == null)
            {
                // Dacă nu găsim cartea, returnăm un mesaj de eroare
                return Json(new { success = false, message = "Cartea nu a fost găsită." });
            }

            try
            {
                // Ștergem cartea din baza de date
                bookRepository.DeleteBook(bookId);

                // Returnăm un mesaj de succes după ștergere
                return Json(new { success = true, message = "Cartea a fost ștearsă cu succes." });
            }
            catch (Exception ex)
            {
                // Dacă apare vreo eroare în timpul ștergerii, returnăm un mesaj de eroare
                return Json(new { success = false, message = "Eroare la ștergerea cărții: " + ex.Message });
            }
        }

        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View(new List<BookModel>());  // Returnează o listă goală dacă nu există termen de căutare
            }
            var books = bookRepository.SearchBooks(searchTerm);  // Căutare cărți după termen
            return View(books);  // Afișează rezultatele căutării
        }

        // Controller

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
