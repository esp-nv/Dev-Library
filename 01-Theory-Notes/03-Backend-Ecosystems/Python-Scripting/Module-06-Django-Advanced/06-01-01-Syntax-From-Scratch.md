[⬅️ 06-01-00. Теория & Капани](06-01-00-Auth-And-Authorization.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-02-00. User Model & Signals](06-02-00-User-Model-And-Signals.md)

---

# 06-01-01. Django Auth & Authorization: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за регистрация (`UserCreationForm`), логин, логаут, работа с групи и права, и защита на HTML елементи и изгледи в Django от нулата.

## 🧭 Съдържание на темата
* [1. Регистрация, Вход и Изход (Вградени класове и форми)](#1-регистрация-вход-и-изход-вградени-класове-и-форми)
* [2. Управление на Групи (`Group`) и Права (`Permission`) с код](#2-управление-на-групи-group-и-права-permission-с-код)
* [3. Защита на изгледи (FBV декоратори и CBV Mixins)](#3-защита-на-изгледи-fbv-декоратори-и-cbv-mixins)
* [4. Проверка на права вътре в HTML шаблони (DTL)](#4-проверка-на-права-вътре-в-html-шаблони-dtl)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. РЕГИСТРАЦИЯ, ВХОД И ИЗХОД (ВГРАДЕНИ КЛАСОВЕ И ФОРМИ)

Django предлага готови вградени изгледи за логин и логаут в `django.contrib.auth.views`:

```python
# Във файла accounts/urls.py:
from django.urls import path
from django.contrib.auth.views import LoginView, LogoutView
from . import views

urlpatterns = [
    path("register/", views.RegisterView.as_view(), name="register"),
    path("login/", LoginView.as_view(template_name="accounts/login.html"), name="login"),
    path("logout/", LogoutView.as_view(), name="logout"),  # Изисква POST заявка в Django 5+!
]
```

### Изглед за регистрация с вградената `UserCreationForm`:
```python
from django.contrib.auth.forms import UserCreationForm
from django.contrib.auth import login
from django.urls import reverse_lazy
from django.views.generic import CreateView

class RegisterView(CreateView):
    form_class = UserCreationForm
    template_name = "accounts/register.html"
    success_url = reverse_lazy("home")

    # Автоматично логва потребителя веднага след успешна регистрация
    def form_valid(self, form):
        response = super().form_valid(form)
        login(self.request, self.object)
        return response
```

---

## 2. УПРАВЛЕНИЕ НА ГРУПИ (`GROUP`) И ПРАВА (`PERMISSION`) С КОД

```python
from django.contrib.auth.models import Group, Permission
from django.contrib.auth import get_user_model

User = get_user_model()

# 1. Създаване или намиране на група (Роля)
editors_group, created = Group.objects.get_or_create(name="Editors")

# 2. Намиране на конкретно право (Формат: <app_label>.<action>_<model_name>)
can_edit_book = Permission.objects.get(codename="change_book")

# 3. Добавяне на правото към групата и добавяне на потребител в групата
editors_group.permissions.add(can_edit_book)

user = User.objects.get(username="elizabeth")
user.groups.add(editors_group)

# 4. Проверка дали потребителят има това право
print(user.has_perm("books.change_book"))  # Изход: True
```

---

## 3. ЗАЩИТА НА ИЗГЛЕДИ (FBV ДЕКОРАТОРИ И CBV MIXINS)

В **`config/settings.py`** задай къде да отиват нелогнатите потребители:
```python
LOGIN_URL = "login"
LOGIN_REDIRECT_URL = "home"
LOGOUT_REDIRECT_URL = "login"
```

### А. При Function-Based Views (FBV):
```python
from django.contrib.auth.decorators import login_required, permission_required

@login_required
def profile_view(request):
    pass

# raise_exception=True връща 403 Forbidden вместо да препраща към Login страницата
@permission_required("books.add_book", raise_exception=True)
def add_book_view(request):
    pass
```

### Б. При Class-Based Views (CBV):
```python
from django.contrib.auth.mixins import LoginRequiredMixin, PermissionRequiredMixin
from django.views.generic import CreateView

class BookCreateView(LoginRequiredMixin, PermissionRequiredMixin, CreateView):
    permission_required = "books.add_book"
    # ... останалите настройки ...
```

---

## 4. ПРОВЕРКА НА ПРАВА ВЪТРЕ В HTML ШАБЛОНИ (DTL)

Във всеки HTML шаблон променливата `perms` е достъпна автоматично:

```html
{% if user.is_authenticated %}
    <span>Здравей, {{ user.username }}!</span>
    
    <!-- В Django 5+ Logout изисква POST форма с CSRF токен -->
    <form method="post" action="{% url 'logout' %}" style="display:inline;">
        {% csrf_token %}
        <button type="submit">Изход</button>
    </form>
{% endif %}

<!-- Показва бутона за редакция САМО на потребители с право 'books.change_book' -->
{% if perms.books.change_book %}
    <a href="{% url 'books:edit' book.pk %}">Редактирай книгата</a>
{% endif %}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис в Django Auth |
| :--- | :--- |
| **Създаване на потребител (с хеш)** | `User.objects.create_user(username="...", password="...")` |
| **Създаване на суперадмин** | `User.objects.create_superuser(username="...", password="...")` |
| **Смяна на парола** | `user.set_password("new_pass"); user.save()` |
| **Проверка на парола и вход** | `u = authenticate(req, ...); if u: login(req, u)` |
| **Изход от профила** | `logout(request)` |
| **Защита на FBV (Логин / Право)** | `@login_required` / `@permission_required("app.perm")` |
| **Защита на CBV (Логин / Право)** | `LoginRequiredMixin` / `PermissionRequiredMixin` |
| **Проверка на право в Python / HTML** | `user.has_perm("app.add_model")` / `{% if perms.app.add_model %}` |

---
[⬅️ 06-01-00. Теория & Капани](06-01-00-Auth-And-Authorization.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-02-00. User Model & Signals](06-02-00-User-Model-And-Signals.md)
