[⬅️ 06-01. Authentication & Authorization](06-01-00-Auth-And-Authorization.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-02-01. Syntax From Scratch](06-02-01-Syntax-From-Scratch.md)

---

# 06-02-00. Extending User Model and Django Signals (Теория, `AbstractUser`, Сигнали & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 02:** Четирите начина за разширяване на потребителския модел (`Proxy`, `OneToOne Profile`, `AbstractUser`, `AbstractBaseUser`), `AUTH_USER_MODEL`, `get_user_model()` и събитийно-ориентирано програмиране чрез Django Signals (`post_save`, `pre_delete`).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурен избор на User модел и Поток на Сигнал (Блок-схема)](#2-архитектурен-избор-на-user-модел-и-поток-на-сигнал-блок-схема)
* [3. Архитектурно сравнение: Django User & Signals срещу C# Identity и Node.js](#3-архитектурно-сравнение-django-user--signals-срещу-c-identity-и-nodejs)
* [4. Плюсове и минуси на разширяването и сигналите](#4-плюсове-и-минуси-на-разширяването-и-сигналите)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **`AbstractUser` срещу `AbstractBaseUser`:**
    *   **`AbstractUser` (Надграждане на готова кола):** Взимаш стандартния User на Django (с готови `username`, `email`, `first_name`, `last_name`, `is_staff`) и просто му добавяш нови екстри (напр. `phone_number` и `age`).
    *   **`AbstractBaseUser` (Строеж на кола от нулата):** Взимаш само двигателя (хеширането на паролата и сесиите), а всички полета и начина на логин (например логин **само с имейл**, без `username`) ги пишеш сама от нулата.
*   **Django Signals = Радиостанция (Observer Pattern):** Моделът `User` излъчва по радиото: *„Току-що се записах в базата! (`post_save`)“*. Отделният файл `signals.py` слуша тази честота и автоматично създава празен `UserProfile` за новия потребител, без моделът `User` изобщо да знае за съществуването на `UserProfile`.

---

## 2. АРХИТЕКТУРЕН ИЗБОР НА USER МОДЕЛ И ПОТОК НА СИГНАЛ (БЛОК-СХЕМА)

```text
1. ИЗБОР НА USER МОДЕЛ:
   • Искаш стандартните полета + 2 нови? ──────► Наследи AbstractUser
   • Искаш логин с Email вместо Username? ─────► Наследи AbstractBaseUser + BaseUserManager
   • Искаш допълнителни данни в 2-ра таблица? ─► Създай модел Profile с OneToOneField(User)

2. ПОТОК НА DJANGO SIGNAL (post_save):
┌──────────────────────────┐     1. Излъчва сигнал      ┌─────────────────────────┐
│ user = User.objects.     │ ─────────────────────────► │ Django Signal Dispatcher│
│        create_user(...)  │   (sender=User,            │ Намира всички закачени  │
└──────────────────────────┘    created=True)           │ @receiver функции       │
                                                        └────────────┬────────────┘
                                                                     │ 2. Изпълнява
                                                                     ▼
                                                        ┌─────────────────────────┐
                                                        │ create_user_profile()   │
                                                        │ Profile.objects.create()│
                                                        └─────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO USER & SIGNALS СРЕЩУ C# IDENTITY И NODE.JS

| Концепция | Django Advanced (Python) | ASP.NET Core Identity (C#) | Node.js / NestJS |
| :--- | :--- | :--- | :--- |
| **Разширяване на User** | `class AppUser(AbstractUser):` | `class AppUser : IdentityUser` | Разширяване на Entity класа |
| **Регистрация на User клас** | `AUTH_USER_MODEL = "accounts.AppUser"` | `AddIdentity()` | Конфигурация в Auth модула |
| **Взимане на активния User** | `get_user_model()` или `settings.AUTH_USER_MODEL` | `UserManager<AppUser>` (чрез DI) | Инжектиране на `UserRepository` |
| **Мениджър за създаване** | `class AppUserManager(BaseUserManager):` | `UserManager<TUser>` | `UserService` |
| **Събития при запис (Hooks)** | `@receiver(post_save, sender=User)` | EF Core `SaveChangesInterceptor` | TypeORM `@AfterInsert()` / EventEmitter |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА РАЗШИРЯВАНЕТО И СИГНАЛИТЕ

### 🟢 Предимства (Pros):
*   **Чиста архитектура с `AbstractUser`:** Всички основни данни за потребителя живеят в една таблица без нужда от излишни `JOIN` заявки при всяко зареждане на `request.user`.
*   **Слабо обвързване (Decoupling) чрез Signals:** Позволява на външни приложения (напр. `analytics` или `notifications`) да реагират на създаване или изтриване на потребител, без да пипаш кода на приложението `accounts`.

### 🔴 Недостатъци (Cons):
*   **Трудно проследяване на сигналите (Скрит поток):** Когато имаш 15 сигнала в проекта, извикването на `user.save()` може скрито да задейства 10 верижни операции, което затруднява дебъгването.
*   **Сигналите са СИНХРОННИ:** Въпреки името си, Django Signals **не се изпълняват на заден план** — те блокират HTTP заявката, докато не приключат (за бавни задачи се ползва Celery!).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Безкрайна рекурсия в `post_save` сигнал и хардкоднат `User` import):
```python
from django.db.models.signals import post_save
from django.dispatch import receiver
# 🚨 ГРЕШКА 1: Директен import на вградения User!
# Ако проектът ползва CustomUser, този код ще гръмне с ValueError!
from django.contrib.auth.models import User

@receiver(post_save, sender=User)
def update_user_signal(sender, instance, created, **kwargs):
    # 🚨 ГРЕШКА 2: Липсва проверка 'if created:' и се вика instance.save() вътре в post_save!
    # instance.save() отново излъчва post_save -> вика същата функция -> RecursionError!
    instance.first_name = "Updated"
    instance.save()
```
*   **Как да се избегне:** Винаги взимай потребителския модел чрез **`get_user_model()`** (в код) или **`settings.AUTH_USER_MODEL`** (във `ForeignKey`). В `post_save` винаги проверявай **`if created:`** и никога не викай `.save()` върху същия `instance` без защита.

### 🟢 ПРАВИЛЕН КОД (Чист Pythonic стандарт с `AbstractUser` и правилно регистриран Signal):
```python
# 1. Във файла accounts/models.py
from django.contrib.auth.models import AbstractUser
from django.db import models

class CustomUser(AbstractUser):
    phone_number = models.CharField(max_length=15, blank=True)
    birth_date = models.DateField(null=True, blank=True)

# 2. Във файла accounts/signals.py
from django.conf import settings
from django.db.models.signals import post_save
from django.dispatch import receiver
from .models import UserProfile

@receiver(post_save, sender=settings.AUTH_USER_MODEL)
def create_profile_on_register(sender, instance, created: bool, **kwargs):
    if created:  # Изпълнява се САМО при първоначално създаване на потребителя!
        UserProfile.objects.create(user=instance)

# 3. Във файла accounts/apps.py (ЗАДЪЛЖИТЕЛНО за да се заредят сигналите при старт!)
from django.apps import AppConfig

class AccountsConfig(AppConfig):
    default_auto_field = "django.db.models.BigAutoField"
    name = "accounts"

    def ready(self):
        import accounts.signals  # Регистрира декораторите @receiver при старт на сървъра!
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Кои са трите ситуации в Django ORM, при които сигналите `pre_save` и `post_save` **изобщо НЕ се задействат**?“*
*   **Уловката:** Кандидатите често разчитат на `post_save` за критичен одит или изчисляване на статистики и остават изненадани, когато данните им се разминават!
*   **Точен Senior отговор:** Сигналите в Django се излъчват **единствено вътре в Python метода `.save()` на единичен обект**. Те **няма да се задействат** при следните 3 масови операции (защото те прескачат Python слоя и пращат директен SQL):
    1.  **`Model.objects.filter(...).update(...)`** (изпраща директен SQL `UPDATE`).
    2.  **`Model.objects.bulk_create([...])`** (изпраща директен SQL `INSERT` за хиляди редове наведнъж).
    3.  **`Model.objects.bulk_update([...], fields=[...])`** (масов SQL `UPDATE`).
    *   *Решение:* Ако ти трябва гарантирана реакция на ниво база данни при всяка промяна, използвай SQL тригери или сервизен слой (Service Layer), през който минават всички записи.

---
[⬅️ 06-01. Authentication & Authorization](06-01-00-Auth-And-Authorization.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-02-01. Syntax From Scratch](06-02-01-Syntax-From-Scratch.md)
