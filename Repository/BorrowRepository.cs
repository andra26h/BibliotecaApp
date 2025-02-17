using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Models.DBObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaApp.Repository
{
    public class BorrowRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserRepository userRepository;
        private readonly BookRepository bookRepository;

        public BorrowRepository()
        {
            dbContext = new ApplicationDbContext();
            bookRepository = new BookRepository();
        }

        public async Task<BorrowingModel> GetActiveBorrowingAsync(int userId, int bookId)
        {
            // Căutăm împrumutul activ pentru utilizatorul specificat și cartea respectivă
            var borrow = await dbContext.Borrowings
                .Where(b => b.UserId == userId && b.BookId == bookId && b.Status == "Active")
                .FirstOrDefaultAsync();

            if (borrow == null)
            {
                return null; // Dacă nu există împrumut activ, returnăm null
            }

            // Folosim metoda de mapare existentă pentru a crea un BorrowingModel din entitatea Borrow
            var borrowingModel = MapToModel(borrow);

            return borrowingModel;
        }

        /*public async Task<BorrowingModel> GetBorrowedBorrowingAsync(int userId, int bookId)
        {
            // Căutăm împrumuturile anteriore pentru utilizatorul specificat și cartea respectivă
            var borrow = await dbContext.Borrowings
                .Where(b => b.UserId == userId && b.BookId == bookId && b.Status == "Borrowed")
                .FirstOrDefaultAsync();

            if (borrow == null)
            {
                return null; // Dacă nu există împrumut activ, returnăm null
            }

            // Folosim metoda de mapare existentă pentru a crea un BorrowingModel din entitatea Borrow
            var borrowingModel = MapToModel(borrow);

            return borrowingModel;
        }
*/
        public async Task<bool> HasUserBorrowedBookAsync(int userId, int bookId)
        {
            // Căutăm un împrumut existent pentru utilizatorul și cartea specificate
            var borrow = await dbContext.Borrowings
                .Where(b => b.UserId == userId && b.BookId == bookId)
                .FirstOrDefaultAsync();

            // Dacă găsim un împrumut pentru utilizator și carte, returnăm true
            return borrow != null;
        }

        // Metodă pentru maparea unui obiect DB la BorrowingModel
        private BorrowingModel MapToModel(Borrowing dbBorrow)
        {
            if (dbBorrow == null)
                return null;

            return new BorrowingModel
            {
                BorrowingId = dbBorrow.BorrowingId,
                BorrowDate = dbBorrow.BorrowDate,
                ReturnDate = dbBorrow.ReturnDate,
                Status = dbBorrow.Status,
                User = userRepository.MapDbObjectToModel(dbBorrow.User),  // Inclusiv User-ul complet, din relatia cu User
                Book = bookRepository.MapDbObjectToModel(dbBorrow.Book)   // Inclusiv Cartea completă, din relația cu Book
            };
        }

        // Metodă pentru maparea unui BorrowingModel la obiectul DB
        private Borrowing MapToDbObject(BorrowingModel model)
        {
            if (model == null)
                return null;

            return new Borrowing
            {
                BorrowingId = model.BorrowingId,
                BorrowDate = model.BorrowDate,
                ReturnDate = (DateTime)model.ReturnDate,
                Status = model.Status,
                UserId = model.User.UserId,  // Asignăm UserId din modelul User
                BookId = model.Book.BookId   // Asignăm BookId din modelul Book
            };
        }

        // Adaugă un împrumut
        public async Task AddBorrowAsync(BorrowingModel borrowingModel)
        {
            var borrowingDbObject = MapToDbObject(borrowingModel);
            dbContext.Borrowings.Add(borrowingDbObject);
            await dbContext.SaveChangesAsync();
        }

        // Obține toate împrumuturile
        public async Task<List<BorrowingModel>> GetAllBorrowsAsync()
        {
            var borrows = await dbContext.Borrowings
                .Include(b => b.User)
                .Include(b => b.Book)
                .ToListAsync();

            return borrows.Select(b => MapToModel(b)).ToList();
        }

        // Obține un împrumut după ID
        public async Task<BorrowingModel> GetBorrowByIdAsync(int id)
        {
            var borrow = await dbContext.Borrowings
                .Include(b => b.User)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowingId == id);

            return MapToModel(borrow);
        }

        // Actualizează starea împrumutului
        public async Task UpdateBorrowAsync(BorrowingModel borrowingModel)
        {
            var borrowingDbObject = MapToDbObject(borrowingModel);
            dbContext.Borrowings.Update(borrowingDbObject);
            await dbContext.SaveChangesAsync();
        }
    }
}
