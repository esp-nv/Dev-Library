[⬅️ 06-05. Celery and Redis](06-05-00-Celery-And-Redis.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-06-01. Syntax From Scratch](06-06-01-Syntax-From-Scratch.md)

---

# 06-06-00. Testing in Django Projects (Теория, `TestCase`, `APIClient`, Mocking & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 06:** Автоматизирано тестване на модели, форми, изгледи (Views) и REST API ендпойнти, изолация на тестовата база данни (`TestCase` срещу `TransactionTestCase`), симулиране на заявки с `Client` и `APIClient` и Mocking на външни услуги.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на Тестовата База Данни в Django и Блок-схема](#2-архитектура-на-тестовата-база-данни-в-django-и-блок-схема)
* [3. Архитектурно сравнение: Django Testing срещу C# ASP.NET Core и NestJS](#3-архитектурно-сравнение-django-testing-срещу-c-aspnet-core-и-nestjs)
* [4. Плюсове и минуси на вградения `django.test.TestCase`](#4-плюсове-и-минуси-на-вградения-djangotesttestcase)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Временна тестова лаборатория (Test Database):** Когато стартираш `python manage.py test`, Django **никога не пипа реалната ти база данни**! Той тайно създава нова празна база в паметта (или с префикс `test_mydb`), пуска всички миграции, върти тестовете и накрая унищожава тестовата база.
*   **Трите нива на тестване в Django:**
    1.  **Unit Tests (Тестове на модели и форми):** Проверяват валидации, методи на модели и чисти функции без HTTP заявки.
    2.  **Integration Tests (`self.client` / `APIClient`):** Симулират истински браузър или Postman — пращат `GET`/`POST` заявки към URL адресите, минават през Middlewares и проверяват статус кодовете (`200`, `302`, `403`, `404`) и шаблоните.
    3.  **Mocking (`@patch`):** Замразява външния свят (Stripe плащания, имейли, Celery задачи), за да са тестовете ти светкавични и да не пращат реални пари или спам.

---

## 2. АРХИТЕКТУРА НА ТЕСТОВАТА БАЗА ДАННИ В DJANGO И БЛОК-СХЕМА

Тайната на бързите тестове в **`django.test.TestCase`** е обвиването на всеки отделен тест в SQL трансакция с автоматичен `ROLLBACK`:

```text
Старт на `python manage.py test`
       │
       ▼
┌──────────────────────────────────────────────────────────┐
│ 1. Създава временна база `test_db` и прилага миграциите  │
└────────────────────────────┬─────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────┐
│ 2. setUpTestData() -> Създава общи начални данни (1 път) │
└────────────────────────────┬─────────────────────────────┘
                             │
         ┌───────────────────┴───────────────────┐
         ▼ (Тест #1)                             ▼ (Тест #2)
┌─────────────────────────┐             ┌─────────────────────────┐
│ BEGIN TRANSACTION       │             │ BEGIN TRANSACTION       │
│ • test_create_book()    │             │ • test_delete_book()    │
│ • ROLLBACK (изтрива     │             │ • ROLLBACK (базата е    │
│   всичко на мига!)      │             │   пак 100% чиста!)      │
└─────────────────────────┘             └─────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO TESTING СРЕЩУ C# ASP.NET CORE И NESTJS

| Концепция | Django (`TestCase` / `pytest`) | ASP.NET Core (`xUnit`) | NestJS (`Jest` + `Supertest`) |
| :--- | :--- | :--- | :--- |
| **Базов клас за тестове** | `from django.test import TestCase` | `IClassFixture<WebApplicationFactory>` | `Test.createTestingModule(...)` |
| **Симулация на HTTP заявки** | `self.client.get("/books/")` | `var client = _factory.CreateClient()` | `request(app.getHttpServer()).get(...)` |
| **Тестов клиент за REST API** | `from rest_framework.test import APIClient` | `HttpClient` с `JsonContent.Create()` | `supertest` с `.send({ ... })` |
| **Бърз логин без парола** | `self.client.force_login(user)` | Тестов `AuthenticationHandler` | Мокване на `JwtAuthGuard` |
| **Проверка на HTML шаблон** | `self.assertTemplateUsed(res, "a.html")` | Парсване на HTML с `AngleSharp` | Проверка на отговора |
| **Проверка на брой SQL заявки** | `with self.assertNumQueries(2):` | EF Core Interceptor / MiniProfiler | TypeORM query logger |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ВГРАДЕНИЯ `DJANGO.TEST.TESTCASE`

### 🟢 Предимства (Pros):
*   **Светкавична изолация чрез `ROLLBACK`:** Тъй като всеки тест се връща назад с трансакция вместо да трие и пресъздава таблиците, 500 теста се изпълняват за няколко секунди.
*   **Вграден детектор за N+1 заявки (`assertNumQueries`):** Можеш да напишеш `with self.assertNumQueries(2):` около заявка към API-то и тестът автоматично ще се провали, ако някой колега случайно въведе N+1 проблем в бъдеще!

### 🔴 Недостатъци (Cons):
*   **Бавен старт при много миграции:** Преди да тръгне първият тест, Django изпълнява всички миграции от нулата (решава се с флага `--keepdb`, който запазва тестовата база между пусканията).
*   **Не тества истински SQL `COMMIT` куки:** Тъй като `TestCase` прави `ROLLBACK` накрая, функциите регистрирани с `transaction.on_commit()` не се изпълняват в стандартен `TestCase` (за тях се ползва `self.captureOnCommitCallbacks(execute=True)`).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Грешен базов клас `unittest.TestCase` вместо `django.test.TestCase` и липсващ Mock):
```python
import unittest
from books.models import Book

# 🚨 ГРЕШКА 1: Наследяване на обикновения unittest.TestCase вместо django.test.TestCase!
# Обикновеният unittest.TestCase НЕ прави трансакционен ROLLBACK след теста!
# Така създадената книга остава в тестовата база и чупи следващите тестове!
class BadBookTest(unittest.TestCase):
    def test_create_book(self):
        Book.objects.create(title="Test Book", price=20)
        self.assertEqual(Book.objects.count(), 1)
```
*   **Как да се избегне:** За тестове, които докосват базата данни на Django, **винаги** наследявай **`from django.test import TestCase`** (или **`APITestCase`** за DRF).

### 🟢 ПРАВИЛЕН КОД (Чист Django `TestCase` с `setUpTestData`, `assertNumQueries` и `@patch`):
```python
from unittest.mock import patch
from django.test import TestCase
from django.urls import reverse
from django.contrib.auth import get_user_model
from books.models import Book

User = get_user_model()

class BookViewsTests(TestCase):
    @classmethod
    def setUpTestData(cls):
        # Изпълнява се САМО ВЕДНЪЖ за целия клас (много по-бързо от setUp!)
        cls.user = User.objects.create_user(username="tester", password="StrongPassword123!")
        cls.book = Book.objects.create(title="Clean Code", price=50.00, author=cls.user)

    def test_book_list_view_uses_correct_template_and_queries(self):
        # Гарантираме, че страницата прави точно 1 SQL заявка (няма N+1 проблем!)
        with self.assertNumQueries(1):
            response = self.client.get(reverse("books:list"))

        self.assertEqual(response.status_code, 200)
        self.assertTemplateUsed(response, "books/list.html")
        self.assertContains(response, "Clean Code")

    @patch("books.tasks.send_welcome_email_task.delay")
    def test_authenticated_user_can_create_book_and_queue_task(self, mock_task_delay):
        self.client.force_login(self.user)  # Бърз логин без POST форма
        
        response = self.client.post(reverse("books:create"), {
            "title": "Python Mastery",
            "price": 65.00
        })

        self.assertEqual(response.status_code, 302)  # Очакваме PRG пренасочване
        self.assertEqual(Book.objects.count(), 2)
        mock_task_delay.assert_called_once()         # Проверяваме, че Celery задачата е извикана!
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между методите `setUp(self)` и `@classmethod def setUpTestData(cls)` в `django.test.TestCase` и кога употребата на `setUpTestData` може да предизвика скрит бъг?“*
*   **Уловката:** Кандидатите често пишат всичко в `setUp()` и тестовете им стават бавни, или пък променят обекти от `setUpTestData()` в паметта!
*   **Точен Senior отговор:** 
    1.  **`setUp(self)`** се изпълнява **наново преди всеки отделен `test_` метод** (ако имаш 20 теста, създава потребителя в базата 20 пъти).
    2.  **`setUpTestData(cls)`** се изпълнява **само веднъж за целия тестов клас** преди всички тестове, което ускорява тестовете драстично.
    *   *Скритата уловка:* Тъй като обектът `cls.book` е създаден веднъж в паметта и се споделя между всички тестове, ако в `test_1` промениш атрибута му в Python (`self.book.title = "Changed"`), без да го презаредиш с `.refresh_from_db()`, `test_2` ще види променената стойност в RAM! Затова данните в `setUpTestData` трябва да се третират като Read-Only.

---
[⬅️ 06-05. Celery and Redis](06-05-00-Celery-And-Redis.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-06-01. Syntax From Scratch](06-06-01-Syntax-From-Scratch.md)
