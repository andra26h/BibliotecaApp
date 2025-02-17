using BibliotecaApp.Data;
using BibliotecaApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BibliotecaApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepository userRepository;

        public LoginController ()
        {
            userRepository = new UserRepository();
        }
        public IActionResult Index()
        {
            // verific daca exista in cookie cheie pt autentificare
            if (HttpContext.Request.Cookies.ContainsKey("LoginToken"))
            {
                return RedirectToAction("Index", "Books");
            }

            return View();
        }

        public IActionResult Login(string email, string password, bool rememberMe)
        {
            // Căutăm utilizatorul în funcție de email
            var user = userRepository.GetUserByEmail(email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) // Verificăm parola
            {
                // Creăm un token pentru autentificare
                var token = Guid.NewGuid().ToString();

                // Salvăm informațiile utilizatorului în sesiune
                HttpContext.Session.SetString("UserEmail", user.Email); // Stocăm email-ul utilizatorului
                HttpContext.Session.SetString("LoginToken", token); // Stocăm token-ul
                HttpContext.Session.SetInt32("RoleId", user.RoleId);

                // Dacă RememberMe este bifat, putem salva și pentru o sesiune mai lungă, dar nu mai folosim cookies
                if (rememberMe)
                {
                    HttpContext.Session.SetString("RememberMe", user.Email);
                }

                // Returnează răspunsul JSON cu succesul
                return Json(new { success = true, message = "Login successful", redirectUrl = "/Books/Index" });
            }
            else
            {
                // Dacă login-ul nu este valid, returnează eroare
                return Json(new { success = false, message = resources.Resource.LoginError });
            }
        }

        //logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Șterge toate datele din sesiune

            // După ce cookie-urile au fost șterse, redirecționează utilizatorul la pagina de login
            return RedirectToAction("Index", "Login");
        }

    }
}
