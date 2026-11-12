[⬅️ 06-04-00. Теория & Капани](06-04-00-Django-REST-Framework.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-05-00. Celery and Redis](06-05-00-Celery-And-Redis.md)

---

# 06-04-01. Django REST Framework: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за инсталиране на DRF и SimpleJWT, писане на `APIView`, `ModelSerializer`, `ModelViewSet` и регистрация в `DefaultRouter` от нулата.

## 🧭 Съдържание на темата
* [1. Инсталация и настройка в `settings.py` (DRF + JWT)](#1-инсталация-и-настройка-в-settingspy-drf--jwt)
* [2. Писане на `ModelSerializer` (Валидация и вложени данни)](#2-писане-на-modelserializer-валидация-и-вложени-данни)
* [3. Ниско ниво: `APIView` (Ръчни `GET` и `POST` методи)](#3-ниско-ниво-apiview-ръчни-get-и-post-методи)
* [4. Високо ниво: `ModelViewSet` и `DefaultRouter` (Пълен CRUD)](#4-високо-ниво-modelviewset-и-defaultrouter-пълен-crud)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИНСТАЛАЦИЯ И НАСТРОЙКА В `SETTINGS.PY` (DRF + JWT)

```bash
pip install djangorestframework djangorestframework-simplejwt
```

Във файла **`config/settings.py`**:
```python
INSTALLED_APPS = [
    # ...
    "rest_framework",
    "rest_framework_simplejwt",
]

REST_FRAMEWORK = {
    "DEFAULT_AUTHENTICATION_CLASSES": (
        "rest_framework_simplejwt.authentication.JWTAuthentication",
    ),
    "DEFAULT_PERMISSION_CLASSES": (
        "rest_framework.permissions.IsAuthenticated",
    ),
    "DEFAULT_PAGINATION_CLASS": "rest_framework.pagination.PageNumberPagination",
    "PAGE_SIZE": 10,
}
```

### Добавяне на готовите JWT ендпойнти за Login и Refresh в `config/urls.py`:
```python
from django.urls import path
from rest_framework_simplejwt.views import TokenObtainPairView, TokenRefreshView

urlpatterns = [
    path("api/token/", TokenObtainPairView.as_view(), name="token_obtain_pair"),
    path("api/token/refresh/", TokenRefreshView.as_view(), name="token_refresh"),
]
```

---

## 2. ПИСАНЕ НА `MODELSERIALIZER` (ВАЛИДАЦИЯ И ВЛОЖЕНИ ДАННИ)

Създай файл **`serializers.py`** в папката на приложението:

```python
from rest_framework import serializers
from .models import Book

class BookSerializer(serializers.ModelSerializer):
    # Изчислено поле, което не съществува в базата данни
    price_with_vat = serializers.SerializerMethodField()

    class Meta:
        model = Book
        fields = ["id", "title", "price", "price_with_vat", "created_at"]
        read_only_fields = ["id", "created_at"]

    def get_price_with_vat(self, obj) -> float:
        return round(float(obj.price) * 1.20, 2)

    # Валидация на ниво поле
    def validate_title(self, value: str) -> str:
        if len(value.strip()) < 3:
            raise serializers.ValidationError("Заглавието трябва да е поне 3 символа!")
        return value.strip()
```

---

## 3. НИСКО НИВО: `APIVIEW` (РЪЧНИ `GET` И `POST` МЕТОДИ)

```python
from rest_framework.views import APIView
from rest_framework.response import Response
from rest_framework import status
from .models import Book
from .serializers import BookSerializer

class BookListApiView(APIView):
    def get(self, request):
        books = Book.objects.all()
        serializer = BookSerializer(books, many=True)  # many=True за списък!
        return Response(serializer.data, status=status.HTTP_200_OK)

    def post(self, request):
        serializer = BookSerializer(data=request.data)  # request.data парсва JSON автоматично!
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
```

---

## 4. ВИСОКО НИВО: `MODELVIEWSET` И `DEFAULTROUTER` (ПЪЛЕН CRUD)

### А. Във файла `books/views.py`:
```python
from rest_framework import viewsets
from .models import Book
from .serializers import BookSerializer

class BookViewSet(viewsets.ModelViewSet):
    queryset = Book.objects.all()
    serializer_class = BookSerializer
```

### Б. Във файла `books/urls.py`:
```python
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import BookViewSet

router = DefaultRouter()
router.register(r"books", BookViewSet, basename="book")

# Генерира автоматично: /books/ (GET, POST) и /books/{id}/ (GET, PUT, PATCH, DELETE)
urlpatterns = [
    path("", include(router.urls)),
]
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в DRF | Какво прави |
| :--- | :--- | :--- |
| **JSON тяло на заявката** | `request.data` | Автоматично парсва JSON, Form-Data и XML |
| **Сериализиране на списък** | `BookSerializer(qs, many=True).data` | Превръща `QuerySet` в списък от речници |
| **Десериализиране и валидация** | `s = BookSerializer(data=request.data); s.is_valid(raise_exception=True)` | Валидира и връща `400 Bad Request` при грешка |
| **Изчислено поле** | `serializers.SerializerMethodField()` | Изисква метод `def get_<field_name>(self, obj):` |
| **API Отговор** | `return Response(data, status=status.HTTP_201_CREATED)` | Връща JSON с правилен HTTP статус код |
| **Пълен CRUD контролер** | `class XViewSet(viewsets.ModelViewSet):` | Включва `list`, `create`, `retrieve`, `update`, `destroy` |
| **Автоматични API URLs** | `router = DefaultRouter(); router.register(...)` | Генерира всички REST маршрути |

---
[⬅️ 06-04-00. Теория & Капани](06-04-00-Django-REST-Framework.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-05-00. Celery and Redis](06-05-00-Celery-And-Redis.md)
