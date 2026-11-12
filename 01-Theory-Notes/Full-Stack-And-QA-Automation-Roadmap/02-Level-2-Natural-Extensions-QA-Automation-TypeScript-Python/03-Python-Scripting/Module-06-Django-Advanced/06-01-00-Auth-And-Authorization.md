[⬅️ 06-00. Intro & Roadmap](06-00-Intro.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-01-01. Syntax From Scratch](06-01-01-Syntax-From-Scratch.md)

---

# 06-01-00. Django Authentication and Authorization (Теория, Сесии, Пароли & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 01:** Разлика между Authentication и Authorization, криптографско хеширане на пароли (`PBKDF2` / `Argon2`), потребителски групи и права (`Groups` и `Permissions`) и защита на изгледи.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Етапи на Логин и Проверка на Права (Блок-схема)](#2-етапи-на-логин-и-проверка-на-права-блок-схема)
* [3. Архитектурно сравнение: Django Auth срещу C# Identity и JS/Node.js](#3-архитектурно-сравнение-django-auth-срещу-c-identity-и-jsnodejs)
* [4. Плюсове и минуси на вградената Auth система на Django](#4-плюсове-и-минуси-на-вградената-auth-система-на-django)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Authentication срещу Authorization (Паспорт срещу Билет за VIP зона):**
    *   **Authentication (Автентикация — „Кой си ти?“):** Показваш си паспорта на входа на летището (`username` + `password`). Системата потвърждава самоличността ти (`authenticate()` + `login()`).
    *   **Authorization (Авторизация — „Какво ти е позволено?“):** Вече си вътре на летището, но за да влезеш в бизнес салона, проверяват дали билетът ти има права за VIP зона (`user.has_perm()` или принадлежност към група `"Moderators"`).
*   **Паролите никога не се пазят в чист вид (Plain Text):** Django автоматично разбива паролата с еднопосочен криптографски алгоритъм и случайни символи (**Salt**), записвайки в базата низ от вида: `pbkdf2_sha256$720000$salt$hash...`.

---

## 2. ЕТАПИ НА ЛОГИН И ПРОВЕРКА НА ПРАВА (БЛОК-СХЕМА)

```text
Потребителят праща POST с username + password
                     │
                     ▼
┌──────────────────────────────────────────────────────────┐
│ 1. user = authenticate(request, username=..., password=..)│
│    (Проверява хеша на паролата в базата данни)           │
└────────────────────┬─────────────────────────────────────┘
                     │
           ┌─────────┴─────────┐
           │ Намерен ли е?     │
           └──┬─────────────┬──┘
       ДА     │             │     НЕ (None)
              ▼             ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ 2. login(request, u) │  │ ❌ Връща грешка:               │
│ Създава Session ID в │  │ "Невалидно име или парола"     │
│ базата и праща Cookie│  └────────────────────────────────┘
└──────────┬───────────┘
           │ 3. Следваща заявка към защитена страница
           ▼
┌──────────────────────────────────────────────────────────┐
│ 4. @login_required / @permission_required("app.add_book")│
│    Проверява request.user.is_authenticated и правата му  │
└──────────────────────────────────────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO AUTH СРЕЩУ C# IDENTITY И JS/NODE.JS

| Концепция | Django Auth (Python) | ASP.NET Core Identity (C#) | Node.js / Passport.js |
| :--- | :--- | :--- | :--- |
| **Проверка на парола** | `authenticate(request, ...)` | `signInManager.PasswordSignInAsync()` | `bcrypt.compare(pass, hash)` |
| **Създаване на сесия** | `login(request, user)` | Записва Auth Cookie автоматично | `req.login(user)` (express-session) |
| **Задаване на нова парола** | `user.set_password("1234")` | `userManager.CreateAsync(user, "1234")` | `await bcrypt.hash("1234", 10)` |
| **Роли / Групи** | `Group` (`user.groups.add(g)`) | `IdentityRole` (`AddToRoleAsync`) | Поле `role` в базата данни |
| **Защита на функция (FBV)** | `@login_required` | `[Authorize]` | `isAuthenticated` middleware |
| **Защита на клас (CBV)** | `LoginRequiredMixin` | `[Authorize]` над Controller | `@UseGuards(AuthGuard)` (NestJS) |
| **Проверка за конкретно право** | `@permission_required("app.change_book")` | `[Authorize(Policy = "CanEdit")]` | Custom Role/Permission Guard |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ВГРАДЕНАТА AUTH СИСТЕМА НА DJANGO

### 🟢 Предимства (Pros):
*   **Автоматични 4 права за всеки модел:** Щом направиш миграция за нов модел `Book`, Django автоматично създава 4 права в базата: `add_book`, `change_book`, `delete_book` и `view_book`.
*   **Защита от Timing Attacks:** Функцията `authenticate()` извършва фиктивно хеширане дори когато потребителското име не съществува, за да не може хакер да познае по времето за отговор кои акаунти съществуват в базата.

### 🔴 Недостатъци (Cons):
*   **Липса на Object-Level Permissions по подразбиране:** Вградените права важат за целия модел (напр. „Може да редактира книги“), но не знаят дали потребителят е собственик на конкретната книга с `id=5` (за това се ползва `UserPassesTestMixin` или външният пакет `django-guardian`).
*   **Сесиите изискват състояние на сървъра (Stateful):** При стандартния `login()` сървърът пази сесията в базата или Redis, докато за мобилни приложения и SPA фронтенди се предпочитат Stateless JWT токени (в DRF).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Записване на парола в чист текст и липсващ `login()` след `authenticate()`):
```python
from django.contrib.auth import authenticate, get_user_model

User = get_user_model()

# 🚨 ГРЕШКА 1: Записване на паролата директно като текст!
# Така паролата влиза некриптирана в SQL базата и authenticate() НИКОГА няма да я разпознае!
bad_user = User.objects.create(username="ivan", password="MySecretPassword123")

def bad_login_view(request):
    user = authenticate(request, username="ivan", password="MySecretPassword123")
    if user is not None:
        # 🚨 ГРЕШКА 2: Забравено извикване на login(request, user)!
        # authenticate() само проверява паролата, но НЕ записва потребителя в сесията!
        return redirect("home")
```
*   **Как да се избегне:** За създаване на потребител **винаги** използвай `User.objects.create_user(...)` или `user.set_password(...)`. При логин винаги викай `login(request, user)` веднага след успешна проверка с `authenticate()`.

### 🟢 ПРАВИЛЕН КОД (Сигурно създаване на потребител, логин и защита на страница):
```python
from django.contrib.auth import authenticate, login, logout, get_user_model
from django.contrib.auth.decorators import login_required, permission_required
from django.shortcuts import render, redirect

User = get_user_model()

# 1. Правилно създаване на потребител с хеширана парола
def register_safe(username: str, email: str, raw_password: str):
    user = User.objects.create_user(username=username, email=email, password=raw_password)
    return user

# 2. Пълен процес на вход (Authenticate + Login)
def login_view(request):
    if request.method == "POST":
        username = request.POST.get("username")
        password = request.POST.get("password")
        
        user = authenticate(request, username=username, password=password)
        if user is not None:
            login(request, user)  # Създава сесия и логва потребителя
            return redirect("dashboard")
            
    return render(request, "accounts/login.html")

# 3. Защита на изглед с изискване за логин И конкретно право
@login_required
@permission_required("books.delete_book", raise_exception=True)
def delete_catalog_item(request, pk: int):
    # Изпълнява се само за логнати потребители с право 'books.delete_book'
    pass
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между флаговете `is_staff`, `is_superuser` и `is_active` в модела `User` на Django, и как се държи `user.has_perm()` при `is_superuser=True`?“*
*   **Уловката:** Кандидатите често бъркат `is_staff` със `superuser` или мислят, че `is_active=False` трие акаунта.
*   **Точен Senior отговор:** 
    1.  **`is_active`:** Определя дали акаунтът може изобщо да се логва в системата. В реални проекти никога не трием потребители с `.delete()` (за да не счупим историята на поръчките им), а правим меко изтриване (Soft Delete) чрез `user.is_active = False`.
    2.  **`is_staff`:** Дава единствено правото да отвориш входната врата на **`/admin/` панела**, но **не ти дава никакви права** да виждаш или променяш таблици вътре, освен ако не ти бъдат дадени изрично через групи или права.
    3.  **`is_superuser`:** Има **абсолютно всички права** в системата автоматично — извикването на `user.has_perm("каквото.и_да_е")` винаги връща `True` без изобщо да проверява таблиците с права в базата данни.

---
[⬅️ 06-00. Intro & Roadmap](06-00-Intro.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-01-01. Syntax From Scratch](06-01-01-Syntax-From-Scratch.md)
