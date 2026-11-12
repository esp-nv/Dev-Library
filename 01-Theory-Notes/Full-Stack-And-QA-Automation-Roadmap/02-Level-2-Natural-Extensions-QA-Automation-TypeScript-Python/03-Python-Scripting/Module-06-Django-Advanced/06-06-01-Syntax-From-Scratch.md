[⬅️ 06-06-00. Теория & Капани](06-06-00-Testing-In-Django.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-07-00. Deployment Setup](06-07-00-Deployment-Setup.md)

---

# 06-06-01. Testing in Django: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за тестване на модели, форми, HTML изгледи и DRF REST API ендпойнти с JWT в Django от нулата.

## 🧭 Съдържание на темата
* [1. Организация на тестовете и конзолни команди](#1-организация-на-тестовете-и-конзолни-команди)
* [2. Тестване на Модели и Форми (Unit Tests)](#2-тестване-на-модели-и-форми-unit-tests)
* [3. Тестване на Views със симулиран клиент (`self.client`)](#3-тестване-на-views-със-симулиран-клиент-selfclient)
* [4. Тестване на DRF REST API с `APITestCase` и `APIClient`](#4-тестване-на-drf-rest-api-с-apitestcase-и-apiclient)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОРГАНИЗАЦИЯ НА ТЕСТОВЕТЕ И КОНЗОЛНИ КОМАНДИ

За големи приложения изтрий единичния файл `tests.py` и създай папка **`tests/`** с празен `__init__.py` и отделни файлове, започващи с **`test_`**:
```text
books/
└── tests/
    ├── __init__.py
    ├── test_models.py
    ├── test_forms.py
    └── test_views.py
```

### Команди за пускане в терминала:
```bash
# 1. Пуска всички тестове в целия проект
python manage.py test

# 2. Пуска тестовете само на конкретно приложение с подробен изход (-v 2)
python manage.py test books -v 2

# 3. Запазва тестовата база между пусканията (ускорява старта 5 пъти!)
python manage.py test --keepdb

# 4. Измерване на покритието на кода с тестове (pip install coverage)
coverage run manage.py test
coverage report -m
```

---

## 2. ТЕСТВАНЕ НА МОДЕЛИ И ФОРМИ (UNIT TESTS)

```python
from django.test import TestCase
from django.core.exceptions import ValidationError
from books.models import Book
from books.forms import BookForm

class BookModelAndFormTests(TestCase):
    def test_book_str_representation(self):
        book = Book.objects.create(title="Dune", price=35.00)
        self.assertEqual(str(book), "Dune (35.00 лв.)")

    def test_book_form_rejects_negative_price(self):
        form = BookForm(data={"title": "Dune", "price": -10})
        self.assertFalse(form.is_valid())
        self.assertIn("price", form.errors)
```

---

## 3. ТЕСТВАНЕ НА VIEWS СЪС СИМУЛИРАН КЛИЕНТ (`SELF.CLIENT`)

```python
from django.test import TestCase
from django.urls import reverse
from django.contrib.auth import get_user_model

User = get_user_model()

class ProtectedViewTests(TestCase):
    def setUp(self):
        self.user = User.objects.create_user(username="eli", password="Password123!")

    def test_anonymous_user_is_redirected_to_login(self):
        response = self.client.get(reverse("books:create"))
        self.assertEqual(response.status_code, 302)
        self.assertIn("/login/", response.url)

    def test_logged_in_user_can_access_create_page(self):
        self.client.force_login(self.user)
        response = self.client.get(reverse("books:create"))
        self.assertEqual(response.status_code, 200)
```

---

## 4. ТЕСТВАНЕ НА DRF REST API С `APITESTCASE` И `APICLIENT`

```python
from django.urls import reverse
from rest_framework import status
from rest_framework.test import APITestCase
from django.contrib.auth import get_user_model
from books.models import Book

User = get_user_model()

class BookApiTests(APITestCase):
    def setUp(self):
        self.user = User.objects.create_user(username="api_user", password="Password123!")
        # Директна JWT / Token автентикация на тестовия API клиент без POST заявка!
        self.client.force_authenticate(user=self.user)

    def test_create_book_via_json_api(self):
        url = reverse("book-list")
        payload = {"title": "API Testing Guide", "price": "49.90"}
        
        response = self.client.post(url, payload, format="json")
        
        self.assertEqual(response.status_code, status.HTTP_201_CREATED)
        self.assertEqual(response.data["title"], "API Testing Guide")
        self.assertEqual(Book.objects.count(), 1)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Инструмент | Синтаксис | Какво проверява |
| :--- | :--- | :--- |
| **Бърза подготовка на данни** | `@classmethod def setUpTestData(cls):` | Създава общи данни веднъж за целия клас |
| **Бърз логин в Django View** | `self.client.force_login(user)` | Логва потребителя без да минава през форма |
| **Бърз логин в DRF API** | `self.client.force_authenticate(user=u)` | Заобикаля JWT/Session проверката в тестове |
| **Проверка на HTTP статус** | `self.assertEqual(res.status_code, 200)` | Проверява статус кода на отговора |
| **Проверка на редирект** | `self.assertRedirects(res, "/books/")` | Проверява статус `302` и крайния URL |
| **Проверка на HTML шаблон** | `self.assertTemplateUsed(res, "a.html")` | Гарантира, че е използван правилният шаблон |
| **Проверка на текст в HTML** | `self.assertContains(res, "Здравей")` | Проверява статус `200` + наличие на текста |
| **Лимит на SQL заявки** | `with self.assertNumQueries(2):` | Засича и предотвратява N+1 заявки |

---
[⬅️ 06-06-00. Теория & Капани](06-06-00-Testing-In-Django.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-07-00. Deployment Setup](06-07-00-Deployment-Setup.md)
