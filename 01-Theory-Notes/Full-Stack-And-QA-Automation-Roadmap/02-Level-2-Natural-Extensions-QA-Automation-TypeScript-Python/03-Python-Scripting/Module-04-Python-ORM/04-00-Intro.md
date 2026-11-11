[⬅️ Назад към Индекса](README.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01. Models and Fields](04-01-00-Models-And-Fields.md)

---

# 04-00. Intro: Django ORM срещу SQLAlchemy & EF Core (Архитектурен Roadmap)

**Модул 04:** Python ORM | **Цел:** Фундаментално сравнение между Active Record (Django ORM) и Data Mapper (SQLAlchemy) спрямо Entity Framework Core, инсталация и избор на правилен инструмент.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектурна разлика (Active Record vs Data Mapper) и Блок-схема](#2-архитектурна-разлика-active-record-vs-data-mapper-и-блок-схема)
* [3. Архитектурно сравнение: Django ORM срещу SQLAlchemy и C# EF Core](#3-архитектурно-сравнение-django-orm-срещу-sqlalchemy-и-c-ef-core)
* [4. Плюсове и минуси на Django ORM и SQLAlchemy](#4-плюсове-и-минуси-на-django-orm-и-sqlalchemy)
* [5. Кодов одит: Един и същ запис в базата по двата начина](#5-кодов-одит-един-и-същ-запис-в-базата-по-двата-начина)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Django ORM = Умен документ с химикалка (Active Record):** Всеки обект в кода знае как сам да се запише или изтрие от базата. Просто викаш `user.save()` или `user.delete()` директно върху самия обект.
*   **SQLAlchemy = Банков превод чрез посредник (Data Mapper):** Обектът `user` е просто празен лист хартия (чист Python клас) и не знае нищо за базата данни. Трябва да го занесеш на гишето (**`Session`**), да го подадеш (`session.add(user)`) и служителят да удари печата (`session.commit()`), точно както прави `DbContext` в C# Entity Framework Core.

---

## 2. АРХИТЕКТУРНА РАЗЛИКА (ACTIVE RECORD VS DATA MAPPER) И БЛОК-СХЕМА

```text
1. DJANGO ORM (Active Record Pattern):
   [ Обект: user ] ──(user.save())──► [ SQL Таблица: auth_user ]
   (Моделът и връзката с базата са слети в едно)

2. SQLALCHEMY (Data Mapper Pattern - като C# EF Core):
   [ Чист Обект: user ] ──► [ Session (Unit of Work) ] ──(session.commit())──► [ SQL Таблица ]
                            (Пази промените в RAM и ги праща наведнъж)
```

---

## 3. АРХИТЕКТУРНО СРАВНЕНИЕ: DJANGO ORM СРЕЩУ SQLALCHEMY И C# EF CORE

| Критерий | Django ORM | SQLAlchemy (v2.0+) | C# Entity Framework Core |
| :--- | :--- | :--- | :--- |
| **Архитектурен патерн** | Active Record | Data Mapper + Unit of Work | Data Mapper + Unit of Work |
| **Инсталация** | `pip install django` | `pip install sqlalchemy` | NuGet пакет |
| **Базов клас за модели** | `models.Model` | `DeclarativeBase` | Чист POCO клас |
| **Четене на данни** | `User.objects.filter(...)` | `session.execute(select(User))` | `context.Users.Where(...)` |
| **Запис в базата** | `user.save()` (веднага!) | `session.add(u); session.commit()` | `context.Add(u); context.SaveChanges()` |
| **Миграции** | Вградени (`makemigrations`) | Външен пакет (`alembic`) | Вградени (`dotnet ef migrations`) |
| **Асинхронна работа** | Частична (`aget`, `afilter`) | Пълна (`AsyncSession`) | Пълна (`ToListAsync()`, `SaveChangesAsync()`) |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА DJANGO ORM И SQLALCHEMY

### 🟢 Django ORM — Предимства и Недостатъци:
*   **Предимства:** Изключително бързо писане на код; вграден админ панел веднага върху моделите; автоматични миграции без допълнителна конфигурация.
*   **Недостатъци:** Силно обвързан с Django рамката; труден за тестване без реална база данни (заради Active Record); по-ограничен при сложни аналитични SQL заявки (`GROUP BY`, `HAVING`, сложни подзаявки).

### 🔵 SQLAlchemy — Предимства и Недостатъци:
*   **Предимства:** Работи навсякъде (Flask, FastAPI, конзолни скриптове); поддържа Unit of Work (групира промените и ги праща в една трансакция); дава пълен контрол върху генерирания SQL; поддържа композитни първични ключове нативно.
*   **Недостатъци:** По-стръмна крива на учене; изисква отделна настройка на `Engine`, `Session` и външния инструмент `Alembic` за миграции.

---

## 5. КОДОВ ОДИТ: ЕДИН И СЪЩ ЗАПИС В БАЗАТА ПО ДВАТА НАЧИНА

### 🟢 Вариант А: Django ORM (Кратък и директен)
```python
from django.db import models

class Course(models.Model):
    title = models.CharField(max_length=100)
    credits = models.IntegerField(default=3)

# Създаване и мигновен запис в базата на 1 ред:
course = Course.objects.create(title="Python ORM", credits=5)
```

### 🔵 Вариант Б: SQLAlchemy 2.0 (Изричен контрол върху сесията)
```python
from sqlalchemy import String, create_engine
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, Session

class Base(DeclarativeBase):
    pass

class Course(Base):
    __tablename__ = "courses"
    id: Mapped[int] = mapped_column(primary_key=True)
    title: Mapped[str] = mapped_column(String(100))
    credits: Mapped[int] = mapped_column(default=3)

engine = create_engine("sqlite:///academy.db")
Base.metadata.create_all(engine)

# Създаване на сесия и изрично потвърждаване (commit):
with Session(engine) as session:
    new_course = Course(title="Python ORM", credits=5)
    session.add(new_course)
    session.commit()  # Изпраща INSERT заявката към SQL чак тук!
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Каква е разликата между Lazy Evaluation при `QuerySet` в Django ORM и `Session` в SQLAlchemy, и кога реално се изпълнява SQL заявката към базата данни?“*
*   **Уловката:** Кандидатите често мислят, че редът `users = User.objects.filter(is_active=True)` веднага удря базата данни.
*   **Точен Senior отговор:** В Django ORM `QuerySet` е **мързелив (Lazy)** — можеш да навържеш 5 филтъра един след друг (`filter().exclude().order_by()`) и няма да се изпълни нито една SQL заявка, докато не се опиташ да прочетеш данните (например с `for u in users:`, `list(users)` или `len(users)`). В SQLAlchemy 2.0 обектът `stmt = select(User).where(...)` също е само чертеж на заявката и не удря базата, докато не го подадеш изрично на сесията чрез `session.execute(stmt)`.

---
[⬅️ Назад към Индекса](README.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-01. Models and Fields](04-01-00-Models-And-Fields.md)
