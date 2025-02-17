using BibliotecaApp.Data;
using BibliotecaApp.resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 1️ Configurare conexiune la baza de date
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Name=BibliotecaDBConnection"));*/
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2️ Configurare sesiuni pentru autentificare custom
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sesiunea expiră după 30 de minute de inactivitate
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3️ Configurare localizare
builder.Services.AddLocalization(options => options.ResourcesPath = "resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization() // Suport pentru fișiere view localizate (ex: Index.ro.cshtml)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(Resource));
    });

// 4️ Adăugare Razor Pages
builder.Services.AddRazorPages(); // Adăugăm Razor Pages

// 5️ Configurare Request Localization
var supportedCultures = new[] { "en", "ro" };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    }
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
    options.SupportedCultures = localizationOptions.SupportedCultures;
    options.SupportedUICultures = localizationOptions.SupportedUICultures;
    options.RequestCultureProviders = localizationOptions.RequestCultureProviders;
});

// 6 Adăugăm serviciul pentru IHttpContextAccessor
builder.Services.AddHttpContextAccessor();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 7 Middleware pentru localizare
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

// 8 Configurare sesiuni
app.UseSession();  // Folosim sesiuni

// 9 Middleware standard
app.UseRouting();
app.MapRazorPages();  // Rutarea pentru Razor Pages

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
