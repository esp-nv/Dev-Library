[⬅️ 05-03. Templates, Static & Media](05-03-00-Templates-Static-Media.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-04-01. Syntax From Scratch](05-04-01-Syntax-From-Scratch.md)

---

# 05-04-00. Django Forms and ModelForms (Теория, CSRF, Валидация & Капани)

**Модул 05:** Django Fundamentals | **Лекционна тема 04:** Обработка на потребителски вход (`forms.Form` срещу `forms.ModelForm`), двустепенна валидация (`clean_<field>` и `clean()`), защита от CSRF атаки и патернът PRG (Post/Redirect/Get).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Жизнен цикъл на валидация на форма и Блок-схема (PRG Патерн)](#2-жизнен-цикъл-на-валидация-на-форма-и-блок-схема-prg-патерн)
* [3. Архитектурно сравнение: Django Forms срещу C# ASP.NET Core и React](#3-архитектурно-сравнение-django-forms-срещу-c-aspnet-core-и-react)
* [4. Плюсове и минуси на `Form` и `ModelForm`](#4-плюсове-и-минуси-на-form-и-modelform)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`forms.Form` срещу `forms.ModelForm`:**
    *   **`forms.Form`** е **самостоятелен въпросник** (напр. форма за контакт, търсачка или форма за логин), който не е свързан директно с конкретна SQL таблица.
    *   **`forms.ModelForm`** е **огледало на твоя SQL модел** — чете колоните от `models.py`, генерира автоматично HTML полетата, валидира ги и накрая има готов метод **`form.save()`**, който записва обекта директно в базата!
*   **Патернът PRG (Post / Redirect / Get):** Никога не връщай `render()` след успешен `POST` запис! Ако потребителят натисне бутона Refresh (`F5`), браузърът ще изпрати `POST` заявката втори път и ще дублира поръчката или плащането. Винаги прави **`redirect()`** след успешен `POST`.

---

## 2. ЖИЗНЕН ЦИКЪЛ НА ВАЛИДАЦИЯ НА ФОРМА И БЛОК-СХЕМА (PRG ПАТЕРН)

Когато извикаш **`form.is_valid()`**, Django преминава през строг 4-степенен конвейер за проверка и почистване на данните:

```text
Потребителят натиска Submit (POST заявка с request.POST и request.FILES)
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 1. Field.to_python() & Вградени Validators (MaxLength, Email, MinValue) │
└─────────────────────────────────────┬───────────────────────────────────┘
                                      ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 2. Потребителски метод clean_<име_на_поле>() (Проверява 1 конкретно поле)│
└─────────────────────────────────────┬───────────────────────────────────┘
                                      ▼
┌─────────────────────────────────────────────────────────────────────────┐
│ 3. Общ метод clean() (Проверява връзка между 2 полета, напр. пароли)    │
└─────────────────────────────────────┬───────────────────────────────────┘
                                      │
                        ┌─────────────┴─────────────┐
                        │ Валидна ли е формата?     │
                        └──┬─────────────────────┬──┘
                      ДА   │                     │   НЕ
                           ▼                     ▼
          ┌────────────────────────┐   ┌────────────────────────────┐
          │ ✅ form.save()         │   │ ❌ Връща същия HTML шаблон │
          │ ➔ redirect("success")  │   │ с попълнени form.errors    │
          └────────────────────────┘   └────────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO FORMS СРЕЩУ C# ASP.NET CORE И REACT

| Концепция | Django 5.x (Python) | ASP.NET Core MVC (C#) | React / TypeScript (Zod + RHF) |
| :--- | :--- | :--- | :--- |
| **Дефиниране на форма** | `class BookForm(forms.ModelForm):` | ViewModel + `[Required]`, `[MaxLength]` | `z.object({ title: z.string() })` |
| **Защита от CSRF атака** | `{% csrf_token %}` в HTML формата | `@Html.AntiForgeryToken()` (автоматично) | SameSite Cookies / Custom Headers |
| **Проверка за валидност** | `if form.is_valid():` | `if (ModelState.IsValid)` | `schema.safeParse(data)` |
| **Почистени данни** | `form.cleaned_data["email"]` | Свойствата на обвързания `Model` | `result.data.email` |
| **Качване на файлове** | `request.FILES` + `enctype="multipart/form-data"` | `IFormFile` + `enctype="multipart/form-data"` | `FormData` обект в `fetch()` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА `FORM` И `MODELFORM`

### 🟢 Предимства (Pros):
*   **Автоматично преобразуване на типовете (`cleaned_data`):** Текстовият вход `"2026-11-12"` от HTML формата автоматично се превръща в истински Python `datetime.date` обект, а `"45.50"` — в `Decimal("45.50")`.
*   **Двупосочно обвързване при редакция (Edit):** При подаване на `BookForm(instance=existing_book)`, формата автоматично попълва всички стари стойности от базата и при `.save()` прави `UPDATE` вместо `INSERT`.

### 🔴 Недостатъци (Cons):
*   **Грозна HTML визия по подразбиране:** Без допълнителни пакети (като `django-crispy-forms` или `django-widget-tweaks`), HTML елементите се генерират без Bootstrap или Tailwind CSS класове.
*   **Трудна работа с динамични форми:** Създаването на форми с променлив брой редове (например добавяне на 1 до 20 продукта в една фактура) изисква по-сложния механизъм `FormSets` и допълнителен JavaScript.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Четене от `request.POST` вместо `cleaned_data`, липсващ `request.FILES` и липсващ `{% csrf_token %}`):
```python
def bad_create_book(request):
    if request.method == "POST":
        # 🚨 ГРЕШКА 1: Забравен request.FILES -> качената снимка на корицата се губи тихо!
        form = BookForm(request.POST)
        if form.is_valid():
            # 🚨 ГРЕШКА 2: Четене директно от суровия request.POST вместо от form.cleaned_data!
            raw_title = request.POST["title"]
            # 🚨 ГРЕШКА 3: Връщане на render() вместо redirect() -> дублира записа при F5 Refresh!
            return render(request, "books/create.html", {"form": form})
```
*   **Как да се избегне:** Винаги подавай `(request.POST, request.FILES)` при форми с файлове; в HTML формата задължително слагай `{% csrf_token %}` и `enctype="multipart/form-data"`; след успешен `.save()` винаги прави `return redirect(...)`.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт с `ModelForm`, `commit=False` и PRG):
```python
from django import forms
from django.core.exceptions import ValidationError
from django.shortcuts import render, redirect
from .models import Book

# 1. Във файла books/forms.py
class BookForm(forms.ModelForm):
    class Meta:
        model = Book
        fields = ["title", "price", "cover"]
        widgets = {
            "title": forms.TextInput(attrs={"class": "form-control", "placeholder": "Заглавие"}),
            "price": forms.NumberInput(attrs={"class": "form-control"}),
        }

    # Персонализирана валидация за конкретно поле (clean_<поле>)
    def clean_price(self):
        price = self.cleaned_data["price"]
        if price <= 0:
            raise ValidationError("Цената на книгата трябва да бъде положително число!")
        return price

# 2. Във файла books/views.py
def create_book(request):
    if request.method == "POST":
        form = BookForm(request.POST, request.FILES)
        if form.is_valid():
            # commit=False създава обекта в RAM без да го праща в SQL, за да добавим автора!
            book = form.save(commit=False)
            book.author = request.user
            book.save()
            return redirect("books:list")  # PRG Патерн: Redirect след успешен POST!
    else:
        form = BookForm()

    return render(request, "books/create.html", {"form": form})
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Ако в `ModelForm` имате връзка `ManyToManyField` (напр. тагове на книга) и използвате `book = form.save(commit=False)`, защо връзките Many-to-Many **не се записват** в базата след `book.save()` и как се решава това?“*
*   **Уловката:** Кандидатите мислят, че `book.save()` записва всички полета от формата, включително `ManyToMany`.
*   **Точен Senior отговор:** Когато извикаш `form.save(commit=False)`, Django не може да запише данните в междинната `ManyToMany` таблица, защото главният обект `book` **все още няма генерирано `id` (Primary Key)** в базата данни! Затова Django закача скрит метод върху самата форма, наречен **`form.save_m2m()`**. След като ръчно запишеш главния обект с `book.save()`, **задължително** трябва да извикаш **`form.save_m2m()`**, за да се запишат и свързаните Many-to-Many релации!

---
[⬅️ 05-03. Templates, Static & Media](05-03-00-Templates-Static-Media.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-04-01. Syntax From Scratch](05-04-01-Syntax-From-Scratch.md)
