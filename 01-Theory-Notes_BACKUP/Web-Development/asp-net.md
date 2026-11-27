````text
може стъпка по стъпка ASP.NET Core MVC проект

ChatGPT каза:
Разбира се. Можем да направим ASP.NET Core MVC проект стъпка по стъпка, като започнем от напълно празен проект и постепенно добавяме:

Създаване на проекта

MVC структурата — Model / View / Controller

Routing

Controller и Actions

Views с Razor

Layout и Bootstrap

Models

Entity Framework Core

SQL база данни и migrations

CRUD операции

Validation

Dependency Injection

Authentication/Authorization

Подготовка и публикуване на проекта

Стъпка 1 — Създаваме проекта
Ако използваш Visual Studio, избери:

Create a new project → ASP.NET Core Web App (Model-View-Controller)

След това например:

Project name: MyMvcApp

Framework: актуалната налична версия на .NET

Authentication: None

Configure for HTTPS: включено

Накрая натисни Create.

Структурата ще изглежда приблизително така:

MyMvcApp
│
├── Controllers
│   └── HomeController.cs
│
├── Models
│   └── ErrorViewModel.cs
│
├── Views
│   ├── Home
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   │
│   └── Shared
│       ├── _Layout.cshtml
│       └── Error.cshtml
│
├── wwwroot
│   ├── css
│   ├── js
│   └── lib
│
├── Program.cs
└── appsettings.json

MVC означава:

Model — данните и бизнес логиката

View — HTML/Razor страниците

Controller — приема заявката и определя какво да се изпълни

Ако искаш да го правим наистина стъпка по стъпка, можем да започнем от Program.cs, HomeController и Index.cshtml и да проследим как една HTTP заявка стига до страницата.



Вие казахте:

https://github.com/esp-nv/text_chatGPT/blob/main/map/ef.md може ли да направиш това и за asp.net core mvc

ChatGPT каза:
Да. Разгледах ef.md — той всъщност е карта за мислене при EF Core: Entity → Mapping → DbContext → DbSet → LINQ → SQL → Database, като разглежда Convention, Attributes и Fluent API. 
G
GitHub

Мога да направя същия тип документ, но специално за ASP.NET Core MVC, така че да учиш не само „какъв код да напиша“, а какво трябва да мисля на всяка стъпка.

Бих го подредил примерно така:

ASP.NET Core MVC — карта стъпка по стъпка
Browser
   ↓
HTTP Request
   ↓
Middleware
   ↓
Routing
   ↓
Controller
   ↓
Action
   ↓
Model / Service
   ↓
Entity Framework Core
   ↓
Database
   ↓
Controller
   ↓
ViewModel
   ↓
Razor View
   ↓
HTML
   ↓
Browser

ЕТАП 1 — Създавам проекта
ASP.NET Core MVC
        ↓
     Project
        ↓
Program.cs
        ↓
Middleware Pipeline
        ↓
Routing

Мисля:

Какъв тип приложение правя?

ЕТАП 2 — Request → Controller
Например:

GET /Products

минава през routing-а и стига до:

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

Мисля:

URL
 ↓
Route
 ↓
Controller
 ↓
Action

ЕТАП 3 — Controller → View
public IActionResult Index()
{
    return View();
}

означава:

ProductsController
       ↓
    Index()
       ↓
Views/Products/Index.cshtml

ЕТАП 4 — Model
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

Тук вече ще разделим:

Domain Model
ViewModel
Entity
DTO

и ще обясним защо не е добра идея автоматично да използваме един и същ клас за всичко.

ЕТАП 5 — Controller → Service
Вместо Controller-ът да съдържа цялата логика:

Controller
    ↓
Service
    ↓
DbContext
    ↓
Database

Например:

public async Task<IActionResult> Index()
{
    var products = await _productService.GetAllAsync();

    return View(products);
}

Тук ще разгледаме и Dependency Injection.

ЕТАП 6 — Service → EF Core
ProductService
      ↓
AppDbContext
      ↓
DbSet<Product>
      ↓
LINQ
      ↓
SQL
      ↓
Database

Например:

var products = await _context.Products
    .Where(p => p.Price > 100)
    .ToListAsync();

И ще проследим какво реално се случва.

ЕТАП 7 — ViewModel → View
Database
   ↓
Entity
   ↓
Service
   ↓
Controller
   ↓
ViewModel
   ↓
Razor View
   ↓
HTML

Например:

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
}

и:

@model ProductViewModel

<h1>@Model.Name</h1>
<p>@Model.Price</p>

ЕТАП 8 — Form → Controller
Тук ще направим целия обратен път:

HTML Form
   ↓
POST
   ↓
Routing
   ↓
Controller Action
   ↓
Model Binding
   ↓
Validation
   ↓
ViewModel
   ↓
Service
   ↓
EF Core
   ↓
Database

Например:

<form asp-action="Create" method="post">

→

[HttpPost]
public async Task<IActionResult> Create(ProductViewModel model)
{
    ...
}

ЕТАП 9 — CRUD
Ще направим пълната карта:

CREATE
Browser
 ↓
Form
 ↓
POST
 ↓
Controller
 ↓
Service
 ↓
EF Core
 ↓
INSERT
 ↓
Database

READ
Browser
 ↓
Controller
 ↓
Service
 ↓
EF Core
 ↓
SELECT
 ↓
ViewModel
 ↓
View

и същото за:

UPDATE
DELETE

ЕТАП 10 — Routing
Ще разгледаме:

/products
/products/details/5
/products/edit/5
/products/delete/5

и как се превръщат в:

ProductsController
    ↓
Index()
Details(5)
Edit(5)
Delete(5)

ЕТАП 11 — Validation
Ще проследим:

User Input
    ↓
Model Binding
    ↓
Data Annotations
    ↓
ModelState
    ↓
Controller
    ↓
View

Например:

[Required]
[StringLength(100)]
public string Name { get; set; }

ЕТАП 12 — Authentication / Authorization
После:

User
 ↓
Login
 ↓
Cookie
 ↓
Authentication Middleware
 ↓
[Authorize]
 ↓
Controller

И най-важното
Ще направим документа в стила на твоя ef.md, тоест не просто списък с API-та, а с въпросите:

Какво се случва?

Къде се случва?

Защо се случва?

Какъв е следващият слой?

Например:

GET /Products/Details/5
        ↓
    Middleware
        ↓
      Routing
        ↓
ProductsController
        ↓
    Details(5)
        ↓
   ProductService
        ↓
    DbContext
        ↓
  DbSet<Product>
        ↓
      LINQ
        ↓
       SQL
        ↓
    Database
        ↓
     Product
        ↓
    ViewModel
        ↓
 Details.cshtml
        ↓
       HTML
        ↓
     Browser

Това ще бъде ASP.NET Core MVC картата, а ef.md ще остане като отделната EF Core карта. Така двете ще се свързват точно там, където трябва — Service/Repository → DbContext → EF Core → Database.

Microsoft също структурира официалната MVC + EF Core поредица около CRUD, migrations, сложен модел, related data, concurrency и inheritance, така че можем постепенно да стигнем и до тези теми. 
M
Microsoft Learn
+1

Ако искаш, мога направо да ти направя пълен aspnet-mvc.md на български, в същия стил и със същата логика като твоя ef.md, започвайки от ЕТАП 1 — Request → Middleware → Routing → Controller.




G
M
Източници
Вие казахте:

da

ChatGPT каза:
Да. Нека го направим като реален aspnet-mvc.md, в същия „map“ стил като ef.md, но подреден така, че да можеш да го използваш като карта, когато пишеш проект.

Ще се придържаме към актуалния ASP.NET Core MVC модел за .NET 10: MVC, Routing, Model Binding, Validation, DI, Controllers, Views и Razor. Това са и основните части, които Microsoft посочва в документацията за MVC. 
M
Microsoft Learn
+1

ASP.NET Core MVC — MAP
0. Голямата картина
                    BROWSER
                       │
                       │ HTTP Request
                       ▼
              ┌─────────────────┐
              │ ASP.NET Core    │
              │ Middleware      │
              └────────┬────────┘
                       │
                       ▼
                 ┌───────────┐
                 │  ROUTING  │
                 └─────┬─────┘
                       │
                       ▼
                ┌─────────────┐
                │ CONTROLLER  │
                └──────┬──────┘
                       │
                       ▼
                  ┌─────────┐
                  │ ACTION  │
                  └────┬────┘
                       │
              ┌────────┴─────────┐
              │                  │
              ▼                  ▼
          ViewModel           Service
              │                  │
              │                  ▼
              │              EF Core
              │                  │
              │                  ▼
              │              DATABASE
              │                  │
              │                  ▼
              │              Entity
              │                  │
              └────────┬─────────┘
                       │
                       ▼
                    VIEW
                       │
                       │ Razor
                       ▼
                     HTML
                       │
                       ▼
                    BROWSER

1. Проект
Създавам
ASP.NET Core Web App (Model-View-Controller)

или CLI:

dotnet new mvc -n MyApp

Проектът съдържа приблизително:

MyApp
│
├── Controllers
│
├── Models
│
├── Views
│
├── wwwroot
│
├── Program.cs
│
├── appsettings.json
│
└── MyApp.csproj

Мисля
Какъв тип приложение правя?

        ↓

ASP.NET Core MVC

        ↓

HTTP
    +
MVC
    +
Razor
    +
Services
    +
EF Core

2. Program.cs
Program.cs е мястото, където приложението се конфигурира и стартира.

Мисловният модел е:

Program.cs
    │
    ├── Services
    │
    ├── Dependency Injection
    │
    ├── Middleware
    │
    └── Routing

Пример:

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

Мисля
builder
   ↓
Конфигурирам приложението

builder.Services
   ↓
Регистрирам зависимости

app
   ↓
Стартирам приложението

app.Use...
   ↓
Middleware pipeline

app.Map...
   ↓
Endpoints / Routing

3. Middleware
Request-ът преминава през middleware pipeline.

HTTP Request
     │
     ▼
Middleware 1
     │
     ▼
Middleware 2
     │
     ▼
Middleware 3
     │
     ▼
Routing
     │
     ▼
Controller

Например:

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

Мисля
Middleware може:

Request
   ↓
[Middleware]
   ↓
променя / проверява / обработва
   ↓
следващ Middleware

или:

Request
   ↓
[Middleware]
   ↓
Response

4. Routing
Routing решава:

Кой Controller и коя Action трябва да обработят този URL?

Например:

GET /Products

може да стане:

ProductsController
        ↓
Index()

При convention-based routing:

pattern: "{controller=Home}/{action=Index}/{id?}"

получаваме:

/Products

↓

ProductsController
Index()

А:

/Products/Details/5

↓

ProductsController
Details(5)

Routing в ASP.NET Core съпоставя URL пътищата с actions на controllers. 
M
Microsoft Learn

5. Controller
Controller е UI-level компонент.

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

Мисля:

Request
   ↓
Controller
   ↓
Action
   ↓
Result

Controller-ът не трябва да съдържа цялата бизнес логика или директно целия data-access код. Обикновено той делегира към services. 
M
Microsoft Learn

6. Action
Action е public метод на Controller.

public IActionResult Index()
{
    return View();
}

Пример:

public IActionResult Details(int id)
{
    ...
}

URL:

/Products/Details/5

може да даде:

id = 5

7. ActionResult
Action може да върне различни резултати.

Action
   │
   ├── View()
   ├── RedirectToAction()
   ├── NotFound()
   ├── BadRequest()
   ├── Ok()
   └── Json()

Пример:

return View();

или:

return RedirectToAction(nameof(Index));

или:

return NotFound();

8. Controller → View
public IActionResult Index()
{
    return View();
}

ASP.NET Core MVC търси:

Views
  │
  └── Products
        │
        └── Index.cshtml

Тоест:

ProductsController
        │
        └── Index()
                │
                ▼
       Views/Products/Index.cshtml

9. View
View е Razor файл:

.cshtml

Пример:

<h1>Products</h1>

<p>Hello MVC</p>

Razor позволява HTML и C# да се използват заедно. 
M
Microsoft Learn

10. ViewModel
ViewModel е обектът, който View получава.

public class ProductViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}

View:

@model ProductViewModel

<h1>@Model.Name</h1>

<p>@Model.Price</p>

Мисловният поток е:

Database
    ↓
Entity
    ↓
Service
    ↓
Controller
    ↓
ViewModel
    ↓
View

Strongly typed ViewModel дава type checking и IntelliSense в View-то. 
M
Microsoft Learn

11. Model
Тук трябва да внимаваме с думата "Model".

В реален проект може да имаме:

Domain Model
Entity
ViewModel
DTO
Input Model

Например Entity:

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}

EF Core може да използва такива POCO класове като модели за данните. 
M
Microsoft Learn

12. Entity
Entity представлява обект от domain/database.

Product

може да бъде:

Products table

с:

Id
Name
Price

Мисля:

Entity
   ↓
EF Core
   ↓
Database

13. Service
Не слагаме всичко в Controller.

Лош модел:

Controller
   │
   ├── validation
   ├── business logic
   ├── database
   ├── calculations
   └── mapping

По-добре:

Controller
     │
     ▼
  Service
     │
     ├── Business Logic
     │
     ▼
  DbContext
     │
     ▼
  Database

Пример:

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .ToListAsync();
    }
}

14. Dependency Injection
ASP.NET Core има вграден Dependency Injection контейнер.

Регистрирам:

builder.Services.AddScoped<ProductService>();

После:

public class ProductsController : Controller
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }
}

Мисловният модел:

Program.cs
    ↓
Register Service
    ↓
DI Container
    ↓
Controller constructor
    ↓
Service

ASP.NET Core MVC позволява Controller-ите да получават необходимите services чрез constructor injection. 
M
Microsoft Learn

15. Controller → Service
public async Task<IActionResult> Index()
{
    var products = await _service.GetAllAsync();

    return View(products);
}

Потокът:

GET /Products
       ↓
ProductsController
       ↓
Index()
       ↓
ProductService
       ↓
GetAllAsync()
       ↓
DbContext
       ↓
Database
       ↓
products
       ↓
View

16. EF Core
Тук MVC картата се свързва с EF картата.

MVC
 │
 └── Service
       │
       ▼
   EF Core
       │
       ▼
   DbContext
       │
       ▼
   DbSet<Product>
       │
       ▼
   LINQ
       │
       ▼
   SQL
       │
       ▼
 Database

Пример:

var products = await _context.Products
    .Where(p => p.Price > 100)
    .ToListAsync();

17. DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}

Мисля:

DbContext
   │
   ├── DbSet<Product>
   ├── DbSet<Category>
   ├── DbSet<Order>
   └── ...

18. CRUD
CREATE
Browser
   ↓
GET /Products/Create
   ↓
Create View
   ↓
HTML Form
   ↓
POST /Products/Create
   ↓
Controller
   ↓
Service
   ↓
EF Core
   ↓
INSERT
   ↓
Database

READ
Browser
   ↓
GET /Products
   ↓
Controller
   ↓
Service
   ↓
EF Core
   ↓
SELECT
   ↓
Database
   ↓
Entity
   ↓
ViewModel
   ↓
View

UPDATE
GET /Products/Edit/5
        ↓
Controller
        ↓
Database
        ↓
Product
        ↓
Edit View
        ↓
POST
        ↓
Controller
        ↓
Validation
        ↓
Service
        ↓
EF Core
        ↓
UPDATE

DELETE
GET /Products/Delete/5
        ↓
Confirmation
        ↓
POST
        ↓
Controller
        ↓
Service
        ↓
EF Core
        ↓
DELETE

19. Model Binding
Model Binding превръща данните от HTTP request в .NET обекти/параметри.

Източници могат да бъдат:

Route
Query String
Form
Headers

Например:

/products/details/5

може да даде:

public IActionResult Details(int id)

където:

id = 5

При form:

<input name="Name" />
<input name="Price" />

може да се bind-не към:

public IActionResult Create(ProductViewModel model)

ASP.NET Core автоматизира извличането и преобразуването на request data към параметрите на action-а. 
M
Microsoft Learn

20. Validation
public class ProductViewModel
{
    [Required]
    public string Name { get; set; }

    [Range(0.01, 100000)]
    public decimal Price { get; set; }
}

Request:

Browser
   ↓
POST
   ↓
Model Binding
   ↓
Validation
   ↓
ModelState

После:

if (!ModelState.IsValid)
{
    return View(model);
}

Мисля:

Input
 ↓
Bind
 ↓
Validate
 ↓
Valid?
 ├── YES → Service
 └── NO  → View

MVC поддържа validation чрез validation attributes и ModelState. 
M
Microsoft Learn

21. Form
View:

<form asp-action="Create" method="post">

    <input asp-for="Name" />

    <input asp-for="Price" />

    <button type="submit">
        Save
    </button>

</form>

Поток:

Razor
 ↓
HTML
 ↓
Browser
 ↓
POST
 ↓
Controller

22. Tag Helpers
Например:

<a asp-controller="Products"
   asp-action="Details"
   asp-route-id="@product.Id">
    Details
</a>

Tag Helper-ът помага за генериране на правилния HTML/URL.

Мисля:

Razor
 ↓
Tag Helper
 ↓
HTML
 ↓
Browser

23. Layout
Общата структура на сайта обикновено е в:

Views/Shared/_Layout.cshtml

Мислим:

                 _Layout
                    │
        ┌───────────┼───────────┐
        │           │           │
      Header      @RenderBody   Footer
                    │
                    ▼
                  View

Например:

<body>

    <header>
        ...
    </header>

    <main>
        @RenderBody()
    </main>

    <footer>
        ...
    </footer>

</body>

24. Partial View
Когато имаме повтарящ се UI:

Views/Shared/_Something.cshtml

например:

_Navigation.cshtml
_ProductCard.cshtml

Мисля:

View
 │
 ├── Partial
 ├── Partial
 └── Partial

25. ViewData / ViewBag / ViewModel
ViewModel
return View(model);

най-ясният вариант за strongly typed View.

ViewData
ViewData["Title"] = "Products";

ViewBag
ViewBag.Title = "Products";

Обикновено:

Основни данни
    ↓
ViewModel

Малки допълнителни данни
    ↓
ViewData / ViewBag

26. GET и POST
Една от основните MVC идеи:

GET
 ↓
Покажи / прочети

POST
 ↓
Промени / създай

Например:

[HttpGet]
public IActionResult Create()
{
    return View();
}

и:

[HttpPost]
public async Task<IActionResult> Create(
    ProductViewModel model)
{
    ...
}

27. POST → Redirect
След успешен POST често:

return RedirectToAction(nameof(Index));

Поток:

POST
 ↓
Save
 ↓
Redirect
 ↓
GET
 ↓
Index

Това е Post/Redirect/Get моделът.

28. Authentication
По-късно:

Browser
   ↓
Login
   ↓
Authentication
   ↓
Cookie
   ↓
Request
   ↓
User

29. Authorization
[Authorize]
public IActionResult Admin()
{
    ...
}

Мисля:

Request
   ↓
Authentication
   ↓
Кой е user-ът?
   ↓
Authorization
   ↓
Има ли право?
   ├── YES → Action
   └── NO  → Access Denied

30. Filters
Filters са допълнителна логика около MVC execution-а.

Request
   ↓
Authorization Filter
   ↓
Action Filter
   ↓
Action
   ↓
Result Filter
   ↓
Response

Могат да се използват за cross-cutting concerns като authorization. 
M
Microsoft Learn

31. Areas
При голям проект:

Areas
│
├── Admin
│   ├── Controllers
│   ├── Views
│   └── Models
│
└── Customer
    ├── Controllers
    ├── Views
    └── Models

Мисля:

Application
    │
    ├── Admin
    │
    ├── Customer
    │
    └── ...

Areas позволяват голям MVC проект да бъде разделен на функционални части. 
M
Microsoft Learn

32. Целият Request
Това е най-важната карта:

                         BROWSER
                            │
                            │
                       HTTP REQUEST
                            │
                            ▼
                    ┌──────────────┐
                    │  MIDDLEWARE  │
                    └──────┬───────┘
                           │
                           ▼
                    ┌──────────────┐
                    │    ROUTING   │
                    └──────┬───────┘
                           │
                           ▼
                 ┌──────────────────┐
                 │    CONTROLLER    │
                 └────────┬─────────┘
                          │
                          ▼
                     ┌─────────┐
                     │ ACTION  │
                     └────┬────┘
                          │
                 ┌────────┴────────┐
                 │                 │
                 ▼                 ▼
           MODEL BINDING       SERVICE
                 │                 │
                 ▼                 ▼
            VALIDATION          EF CORE
                                   │
                                   ▼
                               DATABASE
                                   │
                                   ▼
                                ENTITY
                                   │
                 ┌─────────────────┘
                 │
                 ▼
             VIEWMODEL
                 │
                 ▼
              RAZOR
                 │
                 ▼
               HTML
                 │
                 ▼
              BROWSER

33. POST Request
                   FORM
                     │
                     ▼
                  POST
                     │
                     ▼
                 ROUTING
                     │
                     ▼
                CONTROLLER
                     │
                     ▼
              MODEL BINDING
                     │
                     ▼
                VALIDATION
                     │
               ┌─────┴─────┐
               │           │
             INVALID      VALID
               │           │
               ▼           ▼
             VIEW        SERVICE
                           │
                           ▼
                        EF CORE
                           │
                           ▼
                       DATABASE
                           │
                           ▼
                     REDIRECT
                           │
                           ▼
                           GET

34. MVC + EF Core
Тук вече двете карти се срещат:

                 ASP.NET CORE MVC
                        │
                        ▼
                   Controller
                        │
                        ▼
                     Service
                        │
                        ▼
                  ┌───────────┐
                  │ EF CORE   │
                  └─────┬─────┘
                        │
             ┌──────────┴──────────┐
             │                     │
             ▼                     ▼
          DbContext             LINQ
             │                     │
             └──────────┬──────────┘
                        ▼
                     SQL
                        │
                        ▼
                    DATABASE

35. Основното разделение
┌─────────────────────────────────────────┐
│                MVC                      │
├─────────────────────────────────────────┤
│ Controller                              │
│ View                                    │
│ ViewModel                               │
│ Routing                                 │
│ Model Binding                           │
│ Validation                              │
│ Razor                                   │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│              APPLICATION                │
├─────────────────────────────────────────┤
│ Services                                │
│ Business Logic                          │
│ Dependency Injection                    │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│               DATA                      │
├─────────────────────────────────────────┤
│ EF Core                                 │
│ DbContext                               │
│ Entity                                  │
│ LINQ                                    │
│ Migrations                              │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│              DATABASE                   │
└─────────────────────────────────────────┘

36. Как да мисля при нова функционалност
Когато получа задача:

Добави Products.

Не започвам веднага да пиша Controller.

Мисля:

1. Какви данни имам?
        ↓
2. Entity
        ↓
3. Каква database таблица?
        ↓
4. DbContext / EF Core
        ↓
5. Каква бизнес логика?
        ↓
6. Service
        ↓
7. Какви HTTP операции?
        ↓
8. Controller Actions
        ↓
9. Какви ViewModels?
        ↓
10. Какви Views?
        ↓
11. Каква Validation?
        ↓
12. Какъв Routing?

37. Пример — Product
Entity
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}

↓

DbContext
public DbSet<Product> Products { get; set; }

↓

Service
public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products
        .ToListAsync();
}

↓

Controller
public async Task<IActionResult> Index()
{
    var products = await _service.GetAllAsync();

    return View(products);
}

↓

View
@model List<Product>

@foreach (var product in Model)
{
    <p>
        @product.Name - @product.Price
    </p>
}

↓

Browser
Product 1 - 10.00
Product 2 - 25.00
Product 3 - 50.00

38. Финалната карта
                    USER
                     │
                     ▼
                  BROWSER
                     │
                     ▼
                HTTP REQUEST
                     │
                     ▼
                MIDDLEWARE
                     │
                     ▼
                  ROUTING
                     │
                     ▼
                CONTROLLER
                     │
                     ▼
                  ACTION
                     │
              ┌──────┴──────┐
              │             │
              ▼             ▼
       MODEL BINDING      SERVICE
              │             │
              ▼             ▼
          VALIDATION      EF CORE
                            │
                            ▼
                         DbContext
                            │
                            ▼
                         DbSet
                            │
                            ▼
                           LINQ
                            │
                            ▼
                           SQL
                            │
                            ▼
                        DATABASE
                            │
                            ▼
                          ENTITY
                            │
                            ▼
                        VIEWMODEL
                            │
                            ▼
                           VIEW
                            │
                            ▼
                          RAZOR
                            │
                            ▼
                           HTML
                            │
                            ▼
                         BROWSER
                            │
                            ▼
                           USER

Основното правило
HTTP
 ↓
MVC
 ↓
Application
 ↓
Data
 ↓
Database

и обратно:

Database
 ↓
Data
 ↓
Application
 ↓
MVC
 ↓
HTML
 ↓
Browser

Това е основната карта, върху която могат да се добавят Authentication,
 Authorization, AJAX, APIs, Areas, Caching, Logging, Testing и Deployment.

Това е първата версия на картата. Следващата естествена стъпка е да направим
 отделна карта aspnet-mvc-project.md, в която всяка кутийка от тази схема се
превръща в реални файлове и код — например
Program.cs → Controller → Service → DbContext → Entity → ViewModel → View,
и да изградим един цял CRUD проект от нулата. Официалната MVC документация
също разглежда тези механизми като свързана система, а EF Core tutorial-ът
ги проследява до database слоя. 
