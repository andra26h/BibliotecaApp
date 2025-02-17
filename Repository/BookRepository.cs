using BibliotecaApp.Data;
using BibliotecaApp.Models.DBObjects;
using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Repository
{
    public class BookRepository
    {
        private ApplicationDbContext dbContext;

        public BookRepository()
        {
            this.dbContext = new ApplicationDbContext();
        }

        //lista tuturor cartilor
        public List<BookModel> GetAllBooks()
        {
            var dbBooks = dbContext.Books
                .Include(b => b.Author) // Încarcă autorul
                .Include(b => b.Category) // Încarcă categoria
                .ToList();

            // Mapăm fiecare obiect Book într-un BookModel
            var bookModels = dbBooks.Select(MapDbObjectToModel).ToList();

            return bookModels;
        }


        //returneaza carte dupa id-ul acesteia
        public BookModel GetBookById(int id)
        {
            // Căutăm cartea în baza de date
            var dbBook = dbContext.Books.FirstOrDefault(b => b.BookId == id);

            // Verificăm dacă cartea există
            if (dbBook == null)
            {
                // Poți să arunci o excepție, să returnezi null sau să returnezi un mesaj de eroare, depinde de logica aplicației tale
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            // Mapează obiectul Book din DB într-un BookModel și returnează-l
            return MapDbObjectToModel(dbBook);
        }


        //returneaza cartile dupa autor
        public List<BookModel> GetBooksByAuthor(int authorId)
        {
            return dbContext.Books.Where(b => b.AuthorId == authorId).Select(b => MapDbObjectToModel(b)).ToList();
        }

        //returneaza lista cartilor populare
        public List<BookModel> GetPopularBooks()
        {
            var popularBooks = dbContext.Books.Where(b => b.IsPopular).ToList();

            var popularBookModels = popularBooks.Select(b => MapDbObjectToModel(b)).ToList();

            return popularBookModels;
        }

        //returneaza lista cartilor recomandate
        public List<BookModel> GetRecommendedBooks()
        {
            // Obținem cărțile recomandate din DB
            var recommendedBooks = dbContext.Books.Where(b => b.IsRecommended).ToList();

            // Mapează fiecare carte într-un BookModel
            var recommendedBookModels = recommendedBooks.Select(b => MapDbObjectToModel(b)).ToList();

            return recommendedBookModels;
        }

        //adaugare carte
        public void AddBook(BookModel bookModel)
        {
            dbContext.Books.Add(MapModelToDbObject(bookModel));
            dbContext.SaveChanges();
        }

        //actualizare carte
        public void UpdateBook(BookModel bookModel)
        {
            dbContext.Books.Update(MapModelToDbObject(bookModel));
            dbContext.SaveChanges();
        }

        public async Task<BookModel> GetBookByIdAsync(int bookId)
        {
            var bookDbObject = await dbContext.Books
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (bookDbObject == null)
                return null;

            return new BookModel
            {
                BookId = bookDbObject.BookId,
                Title = bookDbObject.Title,
                Description = bookDbObject.Description ?? "No description available",  // Asigură-te că descrierea nu este null
                AvailableCopies = bookDbObject.AvailableCopies,
                IsPopular = bookDbObject.IsPopular,
                IsRecommended = bookDbObject.IsRecommended,
                CoverImage = bookDbObject.CoverImage,
                AuthorId = bookDbObject.AuthorId,
                CategoryId = bookDbObject.CategoryId,
                AuthorName = bookDbObject.Author?.FirstName + bookDbObject.Author?.LastName, // Adăugăm numele autorului
                CategoryName = bookDbObject.Category?.CategoryName
            };
        }
        // Metoda pentru actualizarea detaliilor unei cărți
        public async Task UpdateBookAsync(BookModel book)
        {
            var bookDbObject = dbContext.Books.FirstOrDefault(b => b.BookId == book.BookId);
            if (bookDbObject != null)
            {
                bookDbObject.AvailableCopies = book.AvailableCopies; // Actualizează numărul de copii disponibile
                dbContext.Books.Update(bookDbObject);
                await dbContext.SaveChangesAsync();
            }
        }

        public void DeleteBook(int bookId)
        {
            var book = dbContext.Books.FirstOrDefault(b => b.BookId == bookId); // presupunând că _context este contextul bazei de date

            if (book != null)
            {
                dbContext.Books.Remove(book);  // Șterge cartea din colecția Books
                dbContext.SaveChanges();  // Salvează modificările în baza de date
            }
            else
            {
                throw new Exception("Cartea nu a fost găsită.");
            }
        }


        public List<Book> SearchBooks(string searchTerm)
        {
            var books = dbContext.Books
                .Include(b => b.Author)
                .Where(b => b.Title.Contains(searchTerm) || b.Author.FirstName.Contains(searchTerm) | b.Author.LastName.Contains(searchTerm))
                .ToList();

            return books;
        }
        public BookModel MapDbObjectToModel(Book dbBook)
        {
            if (dbBook == null)
            {
                throw new ArgumentNullException(nameof(dbBook), "Book cannot be null");
            }

            // Crează BookModel din Book
            BookModel bookModel = new BookModel
            {
                BookId = dbBook.BookId,
                Title = dbBook.Title,
                Description = dbBook.Description ?? "No description available",  // Asigură-te că descrierea nu este null
                AvailableCopies = dbBook.AvailableCopies,
                IsPopular = dbBook.IsPopular,
                IsRecommended = dbBook.IsRecommended,
                CoverImage = dbBook.CoverImage,
                AuthorId = dbBook.AuthorId,
                CategoryId = dbBook.CategoryId,
                AuthorName = dbBook.Author?.FirstName + " " + dbBook.Author?.LastName, // Adăugăm numele complet al autorului
                CategoryName = dbBook.Category?.CategoryName // Adăugăm numele categoriei
            };

            // Mapează recenziile (dacă există)
            if (dbBook.Reviews != null && dbBook.Reviews.Any())
            {
                bookModel.Reviews = dbBook.Reviews.Select(r => new ReviewModel
                {
                    ReviewId = r.ReviewId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate,
                    UserId = r.UserId,
                    Username = r.User?.Username // Mapează doar proprietăți relevante ale utilizatorului
                }).ToList();
            }
            else
            {
                bookModel.Reviews = new List<ReviewModel>();  // Dacă nu există recenzii, setează o listă goală
            }

            return bookModel;
        }
        public Book MapModelToDbObject(BookModel bookModel)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "BookModel cannot be null");
            }

            // Crează obiectul Book din BookModel
            Book dbBook = new Book
            {
                BookId = bookModel.BookId,
                Title = bookModel.Title,
                Description = bookModel.Description ?? "No description available",  // Asigură-te că descrierea nu este null
                AvailableCopies = bookModel.AvailableCopies,
                IsPopular = bookModel.IsPopular,
                IsRecommended = bookModel.IsRecommended,
                CoverImage = bookModel.CoverImage,
                AuthorId = bookModel.AuthorId,  // Asigură-te că AuthorId este corect setat
                CategoryId = bookModel.CategoryId  // Asigură-te că CategoryId este corect setat
            };

            // Verifică dacă există recenzii și mapează-le
            if (bookModel.Reviews != null && bookModel.Reviews.Any())
            {
                dbBook.Reviews = bookModel.Reviews.Select(r => new Review
                {
                    ReviewId = r.ReviewId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate,
                    UserId = r.UserId  // Asigură-te că UserId este corect setat
                }).ToList();
            }

            return dbBook;
        }

    }
}
