````text
EF Core Entity → Table/Column mapping, основно през DbContext и Fluent API. 
G
GitHub

За ASP.NET Core MVC бих го направил малко по-практично: Entity → EF Core Configuration → DbContext → Controller → ViewModel → View.

Например, ако имаме Product, структурата може да бъде:

MyMvcApp/
│
├── Controllers/
│   └── ProductsController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Models/
│   └── Product.cs
│
├── Configurations/
│   └── ProductConfiguration.cs
│
├── ViewModels/
│   └── ProductViewModel.cs
│
└── Views/
    └── Products/
        ├── Index.cshtml
        ├── Create.cshtml
        ├── Edit.cshtml
        └── Delete.cshtml

1. Entity
namespace MyMvcApp.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

Това е Entity, а не ViewModel.

2. EF Core Mapping
Вместо да слагаме [Table], [Column] и други атрибути в Entity-то, можем да използваме Fluent API, както е показано и в твоя файл. 
G
GitHub

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyMvcApp.Models;

namespace MyMvcApp.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("store_products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("product_id");

        builder.Property(x => x.Name)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("product_price")
            .HasPrecision(18, 2);
    }
}

Така получаваме:

Product.Id
    ↓
store_products.product_id

Product.Name
    ↓
store_products.product_name

Product.Price
    ↓
store_products.product_price

3. DbContext
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;

namespace MyMvcApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}

Това е удобен вариант, защото не е необходимо да пишеш:

modelBuilder.Entity<Product>()...

директно в DbContext.

Всички класове, които имплементират IEntityTypeConfiguration<T>, ще бъдат намерени автоматично.

4. Connection string
В appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MyMvcDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

5. Program.cs
При съвременен ASP.NET Core MVC:

using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

6. ViewModel
Тук вече идва специфичната част за MVC.

Не е задължително View-то да работи директно с Entity.

namespace MyMvcApp.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

Можеш да имаш и отделни модели за Create/Edit:

Product
    ↓
Database Entity

ProductViewModel
    ↓
MVC View

Това е по-чисто от това да подаваш EF Entity директно към формите.

7. Controller
Например:

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.ViewModels;

namespace MyMvcApp.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price
            })
            .ToListAsync();

        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var product = new Product
        {
            Name = model.Name,
            Price = model.Price
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}

Тук имаме много ясна граница:

              ASP.NET Core MVC
                     │
                     ↓
              ProductViewModel
                     │
                mapping
                     │
                     ↓
                  Product
                     │
                     ↓
                 EF Core
                     │
                     ↓
              store_products

8. Index.cshtml
@model IEnumerable<MyMvcApp.ViewModels.ProductViewModel>

<h1>Products</h1>

<p>
    <a asp-action="Create" class="btn btn-primary">
        Create Product
    </a>
</p>

<table class="table">
    <thead>
        <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Price</th>
        </tr>
    </thead>

    <tbody>
    @foreach (var product in Model)
    {
        <tr>
            <td>@product.Id</td>
            <td>@product.Name</td>
            <td>@product.Price</td>
        </tr>
    }
    </tbody>
</table>

9. Ако искаш AutoMapper
Можем да добавим и AutoMapper, за да не пишеш ръчно:

var product = new Product
{
    Name = model.Name,
    Price = model.Price
};

а да имаш:

var product = _mapper.Map<Product>(model);

и:

var model = _mapper.Map<ProductViewModel>(product);

AutoMapper официално поддържа този тип mapping чрез IMapper, а при EF Core има и ProjectTo, който позволява проектиране директно от IQueryable. 
A
Automapper Documentation
+1

Тогава архитектурата става:

                    MVC
                     │
          ┌──────────┴──────────┐
          ↓                     ↓
   ProductViewModel        Product Entity
          │                     │
          └──── AutoMapper ─────┘
                    │
                    ↓
                 EF Core
                    │
                    ↓
                Database

Тоест мога да ти преработя целия map-efcore.md конкретно за ASP.NET Core MVC, включително:

Entity

DTO / ViewModel

Fluent API

IEntityTypeConfiguration

DbContext

Relationships (1:1, 1:N, N:N)

Foreign Keys

Migrations

CRUD Controller

Razor Views

AutoMapper

ProjectTo

как да решаваш кога да използваш Entity, DTO или ViewModel
