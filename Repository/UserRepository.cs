using BibliotecaApp.Data;
using BibliotecaApp.Models.DBObjects;
using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Repository
{
    public class UserRepository
    {
        private ApplicationDbContext dbContext;

        public UserRepository()
        {
            this.dbContext = new ApplicationDbContext();
        }
        //toti utilizatorii
        public List<UserModel> GetAllUsers()
        {
            var users = dbContext.Users.ToList();
            return users.Select(u => MapDbObjectToModel(u)).ToList();
        }

        //un utilizator dupa id
        public UserModel GetUserById(int userId)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            return MapDbObjectToModel(user);
        }

        //un utilizator dupa email
        public UserModel GetUserByEmail(string email)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Email == email);
            return MapDbObjectToModel(user);
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            return MapDbObjectToModel(user);
        }
        public UserModel GetUserByUsername(string username)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            return MapDbObjectToModel(user);
        }

        //verificare daca un utilizator exista
        public bool UserExists(int userId)
        {
            return dbContext.Users.Any(u => u.UserId == userId);
        }

        //adaugarea unui utilizator
        public void AddUser(UserModel userModel)
        {
            var user = MapModelToDbObject(userModel);
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        //actualizarea unui utilizator
        public void UpdateUser(UserModel userModel)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserId == userModel.UserId);

            if (user != null)
            {
                // Actualizăm valorile
                user.Username = userModel.Username;
                user.Email = userModel.Email;
                user.Password = userModel.Password; // Aici poate fi un hash al parolei
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.RoleId = userModel.RoleId;
            }
            dbContext.SaveChanges();
        }
        //stergere user
        public void DeleteUser(int userId)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }
        }
        //Autentificare user
        public UserModel AuthenticateUser(string username, string hashedPassword)
        {
            var user = GetAllUsers().FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);
            return user;
        }
        public UserModel MapDbObjectToModel(User dbUser)
        {
            UserModel userModel = new UserModel();

            if (dbUser != null)
            {
                userModel.UserId = dbUser.UserId;
                userModel.Username = dbUser.Username;
                userModel.Password = dbUser.Password;
                userModel.Email = dbUser.Email;
                userModel.FirstName = dbUser.FirstName;
                userModel.LastName = dbUser.LastName;
                userModel.RoleId = dbUser.RoleId;
            }
            return userModel;
        }
        public User MapModelToDbObject(UserModel userModel)
        {
            User dbUser = new User();

            if (userModel != null)
            {
                dbUser.UserId = userModel.UserId;
                dbUser.Username = userModel.Username;
                dbUser.Password = userModel.Password;
                dbUser.Email = userModel.Email;
                dbUser.FirstName = userModel.FirstName;
                dbUser.LastName = userModel.LastName;
                dbUser.RoleId = userModel.RoleId;
            }
            return dbUser;
        }
    }
}
