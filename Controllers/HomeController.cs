using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.Models;
using BibliotecaApp.Repository;

namespace BibliotecaApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserRepository userRepository;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        userRepository = new UserRepository();
    }

    public IActionResult Index()
    {
        // Verificăm dacă utilizatorul este autentificat în sesiune
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var loginToken = HttpContext.Session.GetString("LoginToken");
        var roleToken = HttpContext.Session.GetInt32("RoleId");

        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(loginToken) || roleToken == null)
        {
            // Dacă nu există utilizator autentificat, redirecționăm spre pagina de login
            return RedirectToAction("Index", "Login");
        }

        // Dacă utilizatorul este autentificat, sau e guest continuăm și încărcăm pagina principală
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
