using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Models.DBObjects;

namespace BibliotecaApp.Repository
{
    public class AuthorRepository
    {
        private ApplicationDbContext dbContext;
        private BookRepository bookRepository;

        public AuthorRepository()
        {
            this.dbContext = new ApplicationDbContext();
            bookRepository = new BookRepository();
        }

        public List<AuthorModel> getAllAuthors()
        {
            var dbAuthor = dbContext.Authors.ToList();
            return dbAuthor.Select(a => MapDbObjectToModel(a)).ToList();
        }

        public void AddAuthor(AuthorModel authorModel)
        {
            var author = MapModelToDbObject(authorModel);
            dbContext.Authors.Add(author);
            dbContext.SaveChanges();
        }

        public void DeleteAuthor(int id)
        {
            var author = dbContext.Authors.FirstOrDefault(a => a.AuthorId == id);

            if(author != null)
            {
                dbContext.Authors.Remove(author);
                dbContext.SaveChanges();
            }
        }
        private AuthorModel MapDbObjectToModel(Author dbAuthor)
        {
            AuthorModel authorModel = new AuthorModel();

            authorModel.AuthorId = dbAuthor.AuthorId;
            authorModel.FirstName = dbAuthor.FirstName;
            authorModel.LastName = dbAuthor.LastName;

            // Mapează cărțile scrise de autor
            authorModel.Books = dbAuthor.Books.Select(b => bookRepository.MapDbObjectToModel(b)).ToList();

            return authorModel;
        }

        private Author MapModelToDbObject(AuthorModel authorModel)
        {
            // Asigură-te că authorModel nu este null
            if (authorModel == null)
            {
                throw new ArgumentNullException(nameof(authorModel), "AuthorModel cannot be null");
            }

            // Crează obiectul Author din model
            Author dbAuthor = new Author
            {
                AuthorId = authorModel.AuthorId,
                FirstName = authorModel.FirstName,
                LastName = authorModel.LastName
            };

            // Mapează cărțile autorului (dacă există) în obiectele Book
            if (authorModel.Books != null && authorModel.Books.Any())
            {
                dbAuthor.Books = authorModel.Books.Select(book => bookRepository.MapModelToDbObject(book)).ToList();
            }
            else
            {
                dbAuthor.Books = new List<Book>();  // Asigură-te că ai o listă goală în caz că nu există cărți
            }

            return dbAuthor;
        }

        public AuthorModel GetAuthorById(int authorId)
        {
            var dbAuthor = dbContext.Authors.FirstOrDefault(b => b.AuthorId == authorId);

            // Verificăm dacă cartea există
            if (dbAuthor == null)
            {
                // Poți să arunci o excepție, să returnezi null sau să returnezi un mesaj de eroare, depinde de logica aplicației tale
                throw new KeyNotFoundException($"Author with ID {authorId} not found.");
            }

            // Mapează obiectul Book din DB într-un BookModel și returnează-l
            return MapDbObjectToModel(dbAuthor);
        }
    }
}
