using BibliotecaApp.Models;
using BibliotecaApp.Repository;
using BibliotecaApp.resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BibliotecaApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AuthorRepository authorRepository;

        public AuthorController ()
        {
            authorRepository = new AuthorRepository();
        }
        public IActionResult Index()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create (AuthorModel authorModel)
        {
            if(ModelState.IsValid)
            {
                authorRepository.AddAuthor(authorModel);

                return Json(new { success = true, message = Resource.AuthorSucces });
            }
            return Json(new { success = false, message = Resource.AuthorError });
        }
    }
}
