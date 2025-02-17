using BibliotecaApp.Models;
using BibliotecaApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Cryptography;
using System.Text;

namespace BibliotecaApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserRepository userRepository;
        private readonly BookRepository bookRepository;

        public RegisterController()
        {
            userRepository = new UserRepository();
            bookRepository = new BookRepository();
        }

        public IActionResult Index()
        {
            /* var recommendedBooks = bookRepository.GetRecommendedBooks();
             var popularBooks = bookRepository.GetPopularBooks();

             var registerModel = new UserModel
             {
                 Username = "",
                 Email = "",
                 Password = "",
                 FirstName = "",
                 LastName = "",
                 RecommendedBooks = recommendedBooks,
                 PopularBooks = popularBooks
             };*/
            //return View(registerModel);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserModel userModel)
        {
            // Validarea modelului
            if (ModelState.IsValid)
            {
                // Verificăm dacă există deja un utilizator cu același email
                var existingUser = userRepository.GetUserByEmail(userModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", resources.Resource.UserExistsError);
                    return Json(new { success = false, message = resources.Resource.UserExistsError });
                }
                // Verificăm dacă parola și confirmarea parolei sunt identice
                if (userModel.Password != userModel.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", resources.Resource.ConfirmPassword);
                    return Json(new { success = false, message = "Parolele nu se potrivesc." });
                }

                // Hash-uim parola
                userModel.Password = BCrypt.Net.BCrypt.HashPassword(userModel.Password);

                try
                {
                    // Adăugăm utilizatorul în baza de date
                    userRepository.AddUser(userModel);

                    // Logăm automat utilizatorul după înregistrare
                    var token = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("UserEmail", userModel.Email);
                    HttpContext.Session.SetString("LoginToken", token);
                    HttpContext.Session.SetInt32("RoleId", userModel.RoleId);

                    return Json(new { success = true, message = "Registration successful", redirectUrl = "/Home/Index" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", resources.Resource.RegisterError + ex.Message);
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Please fill in all required fields." });
        }

    }
}
