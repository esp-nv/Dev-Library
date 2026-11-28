[⬅️ 06-02-00. Теория & Капани](06-02-00-User-Model-And-Signals.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-03-00. Middlewares & Sessions](06-03-00-Middlewares-And-Sessions.md)

---

# 06-02-01. Extending User Model & Signals: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на `AbstractUser`, логин с имейл чрез `AbstractBaseUser`, регистрация в `admin.py` и писане на сигнали в Django от нулата.

## 🧭 Съдържание на темата
* [1. Разширяване чрез `AbstractUser` (Най-честият сценарий)](#1-разширяване-чрез-abstractuser-най-честият-сценарий)
* [2. Логин само с Email чрез `AbstractBaseUser` и `BaseUserManager`](#2-логин-само-с-email-чрез-abstractbaseuser-и-baseusermanager)
* [3. Регистрация на CustomUser в `admin.py`](#3-регистрация-на-customuser-в-adminpy)
* [4. Писане и активиране на Django Signals (`post_save`, `pre_delete`)](#4-писане-и-активиране-на-django-signals-post_save-pre_delete)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. РАЗШИРЯВАНЕ ЧРЕЗ `ABSTRACTUSER` (НАЙ-ЧЕСТИЯТ СЦЕНАРИЙ)

### Стъпка 1: Дефинирай класа в `accounts/models.py`
```python
from django.contrib.auth.models import AbstractUser
from django.db import models

class AppUser(AbstractUser):
    email = models.EmailField(unique=True)
    bio = models.TextField(blank=True)
```

### Стъпка 2: Посочи новия модел в `config/settings.py` (ПРЕДИ `makemigrations`!)
```python
AUTH_USER_MODEL = "accounts.AppUser"
```

### Стъпка 3: Как да реферираш новия модел в други файлове
```python
# 1. Във ForeignKey на други модели (използвай settings.AUTH_USER_MODEL):
from django.conf import settings

class Post(models.Model):
    author = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)

# 2. Във views.py, forms.py или tests.py (използвай функцията get_user_model()):
from django.contrib.auth import get_user_model

User = get_user_model()
```

---

## 2. ЛОГИН САМО С EMAIL ЧРЕЗ `ABSTRACTBASEUSER` И `BASEUSERMANAGER`

Когато искаш потребителите да се регистрират и логват **без `username`**, само с `email` и `password`:

```python
from django.contrib.auth.models import AbstractBaseUser, BaseUserManager, PermissionsMixin
from django.db import models

class EmailUserManager(BaseUserManager):
    def create_user(self, email, password=None, **extra_fields):
        if not email:
            raise ValueError("Имейл адресът е задължителен!")
        email = self.normalize_email(email)
        user = self.model(email=email, **extra_fields)
        user.set_password(password)
        user.save(using=self._db)
        return user

    def create_superuser(self, email, password=None, **extra_fields):
        extra_fields.setdefault("is_staff", True)
        extra_fields.setdefault("is_superuser", True)
        return self.create_user(email, password, **extra_fields)

class EmailUser(AbstractBaseUser, PermissionsMixin):
    email = models.EmailField(unique=True)
    is_active = models.BooleanField(default=True)
    is_staff = models.BooleanField(default=False)

    objects = EmailUserManager()

    USERNAME_FIELD = "email"        # Полето, което се използва за вход (Login)
    REQUIRED_FIELDS = []            # Допълнителни задължителни полета при createsuperuser
```

---

## 3. РЕГИСТРАЦИЯ НА CUSTOMUSER В `ADMIN.PY`

Ако регистрираш `CustomUser` с обикновен `admin.site.register(AppUser)`, паролите в админ панела ще се виждат като обикновено текстово поле! Затова винаги наследявай **`UserAdmin`**:

```python
from django.contrib import admin
from django.contrib.auth.admin import UserAdmin
from .models import AppUser

@admin.register(AppUser)
class AppUserAdmin(UserAdmin):
    # Добавяме новите полета към стандартните секции на UserAdmin
    fieldsets = UserAdmin.fieldsets + (
        ("Допълнителна информация", {"fields": ("bio",)}),
    )
```

---

## 4. ПИСАНЕ И АКТИВИРАНЕ НА DJANGO SIGNALS (`POST_SAVE`, `PRE_DELETE`)

### Във файла `accounts/signals.py`:
```python
from django.db.models.signals import post_save, pre_delete
from django.dispatch import receiver
from django.contrib.auth import get_user_model

User = get_user_model()

@receiver(post_save, sender=User)
def log_user_creation(sender, instance, created, **kwargs):
    if created:
        print(f"[SIGNAL] Нов потребител: {instance.email}")

@receiver(pre_delete, sender=User)
def archive_user_before_delete(sender, instance, **kwargs):
    print(f"[SIGNAL] Изтриване на потребител: {instance.email}")
```

*Не забравяй да импортираш `accounts.signals` вътре в метода `ready(self)` на `accounts/apps.py`!*

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис | Какво прави |
| :--- | :--- | :--- |
| **Надграждане на User с полета** | `class AppUser(AbstractUser):` | Запазва всички стандартни полета + твоите |
| **Логин с Email (без username)** | `class EmailUser(AbstractBaseUser, PermissionsMixin):` | Пълен контрол върху структурата на User |
| **Настройка в `settings.py`** | `AUTH_USER_MODEL = "accounts.AppUser"` | Задава се преди първата миграция |
| **Взимане на User в код** | `User = get_user_model()` | Безопасен начин за достъп до активния модел |
| **Референция във `ForeignKey`** | `settings.AUTH_USER_MODEL` | Избягва проблеми при зареждане на приложенията |
| **Слушател за сигнал** | `@receiver(post_save, sender=User)` | Изпълнява функция след `save()` |
| **Активиране на сигнали** | `def ready(self): import app.signals` | Във файла `apps.py` на приложението |

---
[⬅️ 06-02-00. Теория & Капани](06-02-00-User-Model-And-Signals.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-03-00. Middlewares & Sessions](06-03-00-Middlewares-And-Sessions.md)
