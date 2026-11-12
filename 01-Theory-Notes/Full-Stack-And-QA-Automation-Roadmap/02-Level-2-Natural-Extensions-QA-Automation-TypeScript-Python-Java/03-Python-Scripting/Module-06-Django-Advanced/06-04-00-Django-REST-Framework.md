[⬅️ 06-03. Middlewares and Sessions](06-03-00-Middlewares-And-Sessions.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-04-01. Syntax From Scratch](06-04-01-Syntax-From-Scratch.md)

---

# 06-04-00. Django REST Framework (Теория, Serializers, ViewSets, JWT & Капани)

**Модул 06:** Django Advanced | **Лекционна тема 04:** Изграждане на модерни RESTful API-та с DRF, двупосочна сериализация (`Serializer` и `ModelSerializer`), изгледи (`APIView`, `GenericAPIView`, `ModelViewSet`), маршрутизиране с `DefaultRouter`, JWT автентикация и права (`Permissions`).

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на DRF Заявка и Блок-схема](#2-архитектура-на-drf-заявка-и-блок-схема)
* [3. Архитектурно сравнение: Django DRF срещу C# ASP.NET Core Web API и NestJS](#3-архитектурно-сравнение-django-drf-срещу-c-aspnet-core-web-api-и-nestjs)
* [4. Плюсове и минуси на Django REST Framework](#4-плюсове-и-минуси-на-django-rest-framework)
* [5. Кодов одит: Правилен срещу Грешен код](#5-кодов-одит-правилен-срещу-грешен-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Serializer = Двупосочен митничар между Python и JSON:**
    *   **На излизане (Serialization):** Взима сложен Python ORM обект от базата данни и го превежда в чист JSON текст, който React, Angular или мобилно приложение може да прочете (`instance` ➔ `data`).
    *   **На влизане (Deserialization):** Приема суров JSON текст от клиента, проверява го за грешки (`is_valid()`), превръща го в Python типове (`validated_data`) и го записва в базата (`save()`).
*   **Стълбицата на DRF Views (От ръчно към автоматично):**
    1.  **`APIView`:** Най-ниското ниво — сам пишеш методите `def get(self, request):` и `def post(self, request):`.
    2.  **`GenericAPIView` + Mixins (`ListCreateAPIView`):** Готови класове за стандартни комбинации (напр. списък + създаване).
    3.  **`ModelViewSet` + `DefaultRouter`:** Пълен автомат — с 4 реда код генерира всички 6 REST маршрута (`GET` списък, `POST` създаване, `GET` детайл, `PUT` пълна редакция, `PATCH` частична редакция и `DELETE` изтриване).

---

## 2. АРХИТЕКТУРА НА DRF ЗАЯВКА И БЛОК-СХЕМА

```text
Клиент (React / Postman) праща POST /api/books/ с JWT Token и JSON тяло
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ 1. Authentication (JWTAuthentication) ➔ Разчита токена и вади req.user   │
└───────────────────────────────────┬──────────────────────────────────────┘
                                    ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ 2. Permissions (IsAuthenticated / IsOwner) ➔ Проверява има ли права (403)│
└───────────────────────────────────┬──────────────────────────────────────┘
                                    ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ 3. Throttling (Rate Limiting) ➔ Проверява дали не прави 1000 заяв./мин.  │
└───────────────────────────────────┬──────────────────────────────────────┘
                                    ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ 4. Serializer (is_valid() -> save()) ➔ Валидира JSON и връща 201 Created│
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO DRF СРЕЩУ C# ASP.NET CORE WEB API И NESTJS

| Концепция | Django REST Framework (DRF) | ASP.NET Core Web API (C#) | NestJS (TypeScript) |
| :--- | :--- | :--- | :--- |
| **DTO & Валидация** | `ModelSerializer` (`fields`, `validate_x`) | C# `record` / DTO + DataAnnotations | DTO клас + `class-validator` |
| **API Контролер** | `APIView` или `ModelViewSet` | `[ApiController]` + `ControllerBase` | `@Controller('books')` |
| **Автоматични маршрути** | `router = DefaultRouter()` | `[Route("api/[controller]")]` | `@Get()`, `@Post()`, `@Patch()` |
| **JWT Автентикация** | `djangorestframework-simplejwt` | `JwtBearerDefaults.AuthenticationScheme` | `@nestjs/jwt` + `JwtStrategy` |
| **Права (Authorization)** | `permission_classes = [IsAuthenticated]` | `[Authorize(Roles = "Admin")]` | `@UseGuards(JwtAuthGuard, RolesGuard)` |
| **Swagger / OpenAPI** | `drf-spectacular` (автоматичен UI) | `Swashbuckle` / `Scalar` | `@nestjs/swagger` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА DJANGO REST FRAMEWORK

### 🟢 Предимства (Pros):
*   **Browsable API:** DRF генерира интерактивна уеб страница за всеки твой API ендпойнт, от която можеш да тестваш `GET`, `POST` и `DELETE` директно в браузъра без Postman.
*   **Екосистема от готови пакети:** Вградена пагинация (`PageNumberPagination`), търсене (`SearchFilter`), ограничения на заявките (`Throttling`) и лесна JWT интеграция.

### 🔴 Недостатъци (Cons):
*   **По-ниска скорост спрямо FastAPI / ASP.NET Core:** Сериализацията на хиляди сложни вложени обекти в DRF е чисто синхронна и по-бавна от Pydantic в FastAPI.
*   **Сложност при прекалено вложени `ViewSets`:** Когато имаш 3 нива вложени ресурси (`/shops/1/departments/2/employees/`), `DefaultRouter` изисква допълнителни пакети като `drf-nested-routers`.

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Липсващ `many=True` при списък и N+1 срив във вложен Serializer):
```python
from rest_framework import serializers
from rest_framework.response import Response
from rest_framework.views import APIView

class BookSerializer(serializers.ModelSerializer):
    # 🚨 ГРЕШКА 1: Четене на поле от свързан модел (author.name) без select_related в View-то!
    # При 100 книги сериализаторът ще пусне 101 SQL заявки (N+1 проблем)!
    author_name = serializers.CharField(source="author.name", read_only=True)

    class Meta:
        model = Book
        fields = ["id", "title", "author_name"]

class BadBookListView(APIView):
    def get(self, request):
        books = Book.objects.all()  # Липсва .select_related("author")!
        # 🚨 ГРЕШКА 2: Забравен параметър many=True при подаване на списък (QuerySet)!
        # Гърми с AttributeError: Got AttributeError when attempting to get a value for field 'title' on serializer!
        serializer = BookSerializer(books)
        return Response(serializer.data)
```
*   **Как да се избегне:** Когато подаваш списък или `QuerySet` към сериализатор, **задължително** добави **`many=True`**. За всяко поле със `source="relation.field"` в сериализатора, добави `.select_related("relation")` в `queryset`-а на изгледа!

### 🟢 ПРАВИЛЕН КОД (Чист DRF стандарт с `ModelSerializer`, `ModelViewSet` и Custom Permission):
```python
from rest_framework import serializers, viewsets, permissions
from .models import Book

# 1. Custom Permission: Всеки може да чете, но само авторът може да редактира/трие!
class IsAuthorOrReadOnly(permissions.BasePermission):
    def has_object_permission(self, request, view, obj):
        if request.method in permissions.SAFE_METHODS:  # GET, HEAD, OPTIONS
            return True
        return obj.author == request.user

# 2. Serializer с валидация и read-only поле за автора
class BookSerializer(serializers.ModelSerializer):
    author_username = serializers.CharField(source="author.username", read_only=True)

    class Meta:
        model = Book
        fields = ["id", "title", "price", "author_username"]

    def validate_price(self, value):
        if value <= 0:
            raise serializers.ValidationError("Цената трябва да е по-голяма от 0.")
        return value

# 3. Пълен CRUD ViewSet с оптимизирана заявка и права
class BookViewSet(viewsets.ModelViewSet):
    queryset = Book.objects.select_related("author").all()
    serializer_class = BookSerializer
    permission_classes = [permissions.IsAuthenticatedOrReadOnly, IsAuthorOrReadOnly]

    def perform_create(self, serializer):
        serializer.save(author=self.request.user)  # Автоматично закача текущия потребител
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между методите `has_permission(self, request, view)` и `has_object_permission(self, request, view, obj)` в DRF Permissions, и защо `has_object_permission` **не се извиква** при `POST` заявки?“*
*   **Уловката:** Кандидатите често слагат проверка за собственост само в `has_object_permission` и се чудят защо не сработва при списъци или при създаване!
*   **Точен Senior отговор:** 
    1.  **`has_permission()`** се изпълнява **на входа на всеки изглед** (за всички HTTP методи: `GET` списък, `POST` създаване, `DELETE` и др.), преди да се зареди какъвто и да е обект от базата.
    2.  **`has_object_permission()`** се изпълнява **само след като конкретен обект е извлечен от базата** чрез `self.get_object()` (при `GET /books/5/`, `PUT`, `PATCH`, `DELETE`). При **`POST` (създаване)** обектът все още не съществува в базата, а при **`GET` списък** няма единичен обект — затова в тези два случая `has_object_permission` **никога не се извиква**!

---
[⬅️ 06-03. Middlewares and Sessions](06-03-00-Middlewares-And-Sessions.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [➡️ 06-04-01. Syntax From Scratch](06-04-01-Syntax-From-Scratch.md)
