using BibliotecaApp.Models;
using BibliotecaApp.resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryRepository categoryRepository;

        public CategoryController()
        {
            categoryRepository = new CategoryRepository();
        }
        public IActionResult Index()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.AddCategory(categoryModel);

                return Json(new { success = true, message = Resource.CategorySucces });
            }
            return Json(new { success = false, Data = Resource.CategoryError });
        }
    }
}
