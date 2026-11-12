[⬅️ 05-05-00. Теория & Капани](05-05-00-Class-Based-Views.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-06-00. Django Admin](05-06-00-Django-Admin.md)

---

# 05-05-01. Class-Based Views (CBV): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за всички вградени Generic Views (`ListView`, `DetailView`, `CreateView`, `UpdateView`, `DeleteView`), Mixins и регистрация в `urls.py` от нулата.

## 🧭 Съдържание на темата
* [1. Базов `View` и `TemplateView`](#1-базов-view-и-templateview)
* [2. Изгледи за четене (`ListView` и `DetailView`)](#2-изгледи-за-четене-listview-и-detailview)
* [3. Изгледи за редакция (`CreateView`, `UpdateView`, `DeleteView`)](#3-изгледи-за-редакция-createview-updateview-deleteview)
* [4. Защита на достъпа с Mixins (`LoginRequiredMixin`, `UserPassesTestMixin`)](#4-защита-на-достъпа-с-mixins-loginrequiredmixin-userpassestestmixin)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. БАЗОВ `VIEW` И `TEMPLATEVIEW`

```python
from django.http import HttpResponse
from django.views import View
from django.views.generic import TemplateView

# 1. Най-простият клас-изглед (разделя GET и POST в отделни методи)
class PingView(View):
    def get(self, request):
        return HttpResponse("GET заявка получена!")

    def post(self, request):
        return HttpResponse("POST заявка получена!")

# 2. Изглед само за показване на HTML шаблон с данни
class AboutPageView(TemplateView):
    template_name = "common/about.html"

    def get_context_data(self, **kwargs):
        context = super().get_context_data(**kwargs)
        context["company_name"] = "SoftUni Academy"
        return context
```

---

## 2. ИЗГЛЕДИ ЗА ЧЕТЕНЕ (`LISTVIEW` И `DETAILVIEW`)

```python
from django.views.generic import ListView, DetailView
from .models import Book

# 1. Списък с пагинация и търсене (ListView)
class BookListView(ListView):
    model = Book
    template_name = "books/list.html"
    context_object_name = "books"       # В шаблона се ползва {{ books }} вместо {{ object_list }}
    paginate_by = 10                    # Автоматично разделя на страници по 10 елемента!

    # Персонализиране на заявката към базата данни
    def get_queryset(self):
        return Book.objects.select_related("author").order_by("-created_at")

# 2. Детайли за 1 конкретен обект по <int:pk> (DetailView)
class BookDetailView(DetailView):
    model = Book
    template_name = "books/detail.html"
    context_object_name = "book"        # В шаблона се ползва {{ book }} вместо {{ object }}
```

---

## 3. ИЗГЛЕДИ ЗА РЕДАКЦИЯ (`CREATEVIEW`, `UPDATEVIEW`, `DELETEVIEW`)

```python
from django.urls import reverse_lazy
from django.views.generic import CreateView, UpdateView, DeleteView
from .models import Book
from .forms import BookForm

# 1. Създаване (CreateView)
class BookCreateView(CreateView):
    model = Book
    form_class = BookForm               # Ползва нашата форма с CSS класове и валидации
    template_name = "books/form.html"
    success_url = reverse_lazy("books:list")

# 2. Редакция (UpdateView - очаква <int:pk> в urls.py)
class BookUpdateView(UpdateView):
    model = Book
    form_class = BookForm
    template_name = "books/form.html"
    success_url = reverse_lazy("books:list")

# 3. Изтриване (DeleteView - при GET показва confirm шаблон, при POST трие!)
class BookDeleteView(DeleteView):
    model = Book
    template_name = "books/confirm_delete.html"
    success_url = reverse_lazy("books:list")
```

### Регистрация на всички класове в `books/urls.py`:
```python
from django.urls import path
from . import views

urlpatterns = [
    path("", views.BookListView.as_view(), name="list"),
    path("create/", views.BookCreateView.as_view(), name="create"),
    path("<int:pk>/", views.BookDetailView.as_view(), name="detail"),
    path("<int:pk>/edit/", views.BookUpdateView.as_view(), name="edit"),
    path("<int:pk>/delete/", views.BookDeleteView.as_view(), name="delete"),
]
```

---

## 4. ЗАЩИТА НА ДОСТЪПА С MIXINS (`LOGINREQUIREDMIXIN`, `USERPASSESTESTMIXIN`)

```python
from django.contrib.auth.mixins import LoginRequiredMixin, UserPassesTestMixin
from django.views.generic import UpdateView

# Проверява едновременно дали потребителят е логнат И дали е авторът на книгата!
class SecureBookEditView(LoginRequiredMixin, UserPassesTestMixin, UpdateView):
    model = Book
    fields = ["title", "price"]
    template_name = "books/form.html"

    # Връща True (пуска го) или False (хвърля 403 Forbidden)
    def test_func(self) -> bool:
        book = self.get_object()
        return self.request.user == book.author
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Клас / Метод | Какво прави | Ключови атрибути и методи |
| :--- | :--- | :--- |
| **`View`** | Базов клас за HTTP методи | `def get(self, req):`, `def post(self, req):` |
| **`TemplateView`** | Рендира HTML шаблон | `template_name`, `get_context_data()` |
| **`ListView`** | Показва списък от обекти | `model`, `context_object_name`, `paginate_by`, `get_queryset()` |
| **`DetailView`** | Показва 1 обект по `pk` или `slug` | `model`, `context_object_name`, `pk_url_kwarg` |
| **`CreateView`** | Създава нов запис чрез форма | `model`, `form_class` (или `fields`), `success_url`, `form_valid()` |
| **`UpdateView`** | Редактира съществуващ запис по `pk` | `model`, `form_class`, `success_url` |
| **`DeleteView`** | Изтрива запис по `pk` след потвърждение | `model`, `template_name`, `success_url` |
| **`LoginRequiredMixin`** | Пренасочва нелогнатите към Login | Слага се **най-отляво** в наследяването |

---
[⬅️ 05-05-00. Теория & Капани](05-05-00-Class-Based-Views.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-06-00. Django Admin](05-06-00-Django-Admin.md)
