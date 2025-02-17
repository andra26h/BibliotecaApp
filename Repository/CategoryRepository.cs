using BibliotecaApp.Data;
using BibliotecaApp.Models.DBObjects;
using BibliotecaApp.Models;
using BibliotecaApp.Repository;

public class CategoryRepository
{
    private readonly ApplicationDbContext dbContext;
    private readonly BookRepository bookRepository;

    public CategoryRepository()
    {
        dbContext = new ApplicationDbContext();
        bookRepository = new BookRepository();
    }

    public List<CategoryModel> getAllCategories()
    {
        var dbCategory = dbContext.Categories.ToList();
        return dbCategory.Select(c => MapDbObjectToModel(c)).ToList();
    }

    public void AddCategory(CategoryModel categoryModel)
    {
        var category = MapModelToDbObject(categoryModel);
        dbContext.Categories.Add(category);
        dbContext.SaveChanges();
    }

    public void DeleteCategory(int id)
    {
        var category = dbContext.Categories.FirstOrDefault(a => a.CategoryId == id);

        if (category != null)
        {
            dbContext.Categories.Remove(category);
            dbContext.SaveChanges();
        }
    }
    private CategoryModel MapDbObjectToModel(Category dbCategory)
    {
        CategoryModel categoryModel = new CategoryModel();

        categoryModel.CategoryId = dbCategory.CategoryId;
        categoryModel.CategoryName = dbCategory.CategoryName;

        // Mapează cărțile aferente categoriei
        categoryModel.Books = dbCategory.Books.Select(b => bookRepository.MapDbObjectToModel(b)).ToList();

        return categoryModel;
    }

    private Category MapModelToDbObject(CategoryModel categoryModel)
    {
        // Asigură-te că authorModel nu este null
        if (categoryModel == null)
        {
            throw new ArgumentNullException(nameof(categoryModel), "CategoryModel cannot be null");
        }

        // Crează obiectul Author din model
        Category dbCategory = new Category
        {
            CategoryId = categoryModel.CategoryId,
            CategoryName = categoryModel.CategoryName
        };

        // Mapează cărțile autorului (dacă există) în obiectele Book
        if (categoryModel.Books != null && categoryModel.Books.Any())
        {
            dbCategory.Books = categoryModel.Books.Select(book => bookRepository.MapModelToDbObject(book)).ToList();
        }
        else
        {
            dbCategory.Books = new List<Book>();  // Asigură-te că ai o listă goală în caz că nu există cărți
        }

        return dbCategory;
    }


    public List<CategoryModel> GetAllCategories()
    {
        var dbCategories = dbContext.Categories.ToList();
        return dbCategories.Select(c => MapDbObjectToModel(c)).ToList();
    }
}
