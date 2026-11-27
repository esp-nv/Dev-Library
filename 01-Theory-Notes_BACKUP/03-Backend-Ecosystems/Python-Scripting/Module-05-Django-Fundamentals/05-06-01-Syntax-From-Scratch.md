[⬅️ 05-06-00. Теория & Капани](05-06-00-Django-Admin.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ Модул 06: Django Advanced](../Module-06-Django-Advanced/README.md)

---

# 05-06-01. Django Admin: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за регистрация на модели, персонализиране на таблици, секции (`fieldsets`), вградени деца (`TabularInline`) и потребителски действия в `admin.py` от нулата.

## 🧭 Съдържание на темата
* [1. Базова регистрация и конфигурация на таблицата (`list_display`)](#1-базова-регистрация-и-конфигурация-на-таблицата-list_display)
* [2. Групиране на полета във формата (`fieldsets`)](#2-групиране-на-полета-във-формата-fieldsets)
* [3. Вградено редактиране на свързани модели (`TabularInline`)](#3-вградено-редактиране-на-свързани-модели-tabularinline)
* [4. Изчислени колони и масови действия (`@admin.display` и `@admin.action`)](#4-изчислени-колони-и-масови-действия-admindisplay-и-adminaction)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. БАЗОВА РЕГИСТРАЦИЯ И КОНФИГУРАЦИЯ НА ТАБЛИЦАТА (`LIST_DISPLAY`)

Всички настройки се пишат във файла **`admin.py`** на съответното приложение:

```python
from django.contrib import admin
from .models import Book

@admin.register(Book)
class BookAdmin(admin.ModelAdmin):
    # 1. Колони в списъка
    list_display = ["title", "author", "price", "is_published", "created_at"]
    
    # 2. Колони, които са кликаеми линкове към страницата за редакция
    list_display_links = ["title"]
    
    # 3. Директна редакция в самата таблица (без влизане в обекта)
    list_editable = ["price", "is_published"]
    
    # 4. Филтри вдясно и търсачка отгоре
    list_filter = ["is_published", "created_at"]
    search_fields = ["title", "author__name"]  # Търси и по име на свързания автор
    
    # 5. Брой редове на страница и сортиране
    list_per_page = 25
    ordering = ["-created_at"]
```

---

## 2. ГРУПИРАНЕ НА ПОЛЕТА ВЪВ ФОРМАТА (`FIELDSETS`)

Когато един модел има 15 полета, можеш да ги разделиш на красиви секции:

```python
@admin.register(Book)
class BookDetailedAdmin(admin.ModelAdmin):
    readonly_fields = ["created_at"]

    fieldsets = (
        ("Основна информация", {
            "fields": ("title", "author", "price")
        }),
        ("Допълнителни настройки", {
            "classes": ("collapse",),  # Скрива секцията в падащ панел (Accordion)
            "fields": ("description", "is_published", "created_at")
        }),
    )
```

---

## 3. ВГРАДЕНО РЕДАКТИРАНЕ НА СВЪРЗАНИ МОДЕЛИ (`TABULARINLINE`)

Позволява ти да добавяш и редактираш книги директно вътре в страницата на техния автор (`Author`):

```python
from django.contrib import admin
from .models import Author, Book

# 1. Дефинираме Inline класа за детето (TabularInline = табличен вид, StackedInline = вертикален вид)
class BookInline(admin.TabularInline):
    model = Book
    extra = 1  # Колко празни реда за нови книги да показва отдолу

# 2. Закачаме го към родителя
@admin.register(Author)
class AuthorAdmin(admin.ModelAdmin):
    list_display = ["name", "country"]
    inlines = [BookInline]
```

---

## 4. ИЗЧИСЛЕНИ КОЛОНИ И МАСОВИ ДЕЙСТВИЯ (`@ADMIN.DISPLAY` И `@ADMIN.ACTION`)

```python
@admin.register(Book)
class BookCustomAdmin(admin.ModelAdmin):
    list_display = ["title", "price", "price_with_vat"]
    actions = ["make_published"]

    # Изчислена колона (не съществува в SQL таблицата)
    @admin.display(description="Цена с ДДС", ordering="price")
    def price_with_vat(self, obj):
        return f"{obj.price * 1.20:.2f} лв."

    # Масово действие от падащото меню над таблицата
    @admin.action(description="Публикувай избраните книги")
    def make_published(self, request, queryset):
        queryset.update(is_published=True)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Атрибут / Декоратор | Какво прави в Django Admin |
| :--- | :--- |
| **`@admin.register(Model)`** | Регистрира модела в админ панела |
| **`list_display`** | Определя видимите колони в табличния списък |
| **`list_filter`** | Добавя страничен панел с филтри вдясно |
| **`search_fields`** | Добавя лента за текстово търсене отгоре |
| **`list_editable`** | Позволява бърза редакция директно в списъка |
| **`list_select_related`** | Предотвратява N+1 заявки за `ForeignKey` колони |
| **`autocomplete_fields`** | Превръща тежък `<select>` в AJAX поле за търсене |
| **`readonly_fields`** | Прави посочените полета само за четене |
| **`inlines = [ChildInline]`** | Вгражда форма за децата вътре в страницата на родителя |
| **`@admin.action(...)`** | Добавя групова операция за избраните редове |

---
[⬅️ 05-06-00. Теория & Капани](05-06-00-Django-Admin.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ Модул 06: Django Advanced](../Module-06-Django-Advanced/README.md)
