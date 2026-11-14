[⬅️ 04-02-01. Django ORM Syntax](04-02-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-00. CRUD and Queries](04-03-00-CRUD-And-Queries.md)

---

# 04-02-02. SQLAlchemy (Alembic) Migrations: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни стъпки за инсталация, конфигуриране на `alembic.ini` и `env.py`, генериране и връщане на миграции в SQLAlchemy от нулата.

## 🧭 Съдържание на темата
* [1. Инсталация и начална настройка на Alembic (Стъпка по стъпка)](#1-инсталация-и-начална-настройка-на-alembic-стъпка-по-стъпка)
* [2. Свързване на твоите модели в `alembic/env.py`](#2-свързване-на-твоите-модели-в-alembicenvpy)
* [3. Генериране и прилагане на миграции (`revision` и `upgrade`)](#3-генериране-и-прилагане-на-миграции-revision-и-upgrade)
* [4. Връщане назад (`downgrade`) и структура на миграционен файл](#4-връщане-назад-downgrade-и-структура-на-миграционен-файл)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИНСТАЛАЦИЯ И НАЧАЛНА НАСТРОЙКА НА ALEMBIC (СТЪПКА ПО СТЪПКА)

```bash
# Стъпка 1: Инсталиране на Alembic
pip install alembic

# Стъпка 2: Инициализиране на Alembic в папката на проекта
# Това създава файл 'alembic.ini' и папка 'alembic/' с файл 'env.py' вътре
alembic init alembic
```

След това отвори **`alembic.ini`** и задай адреса на твоята база данни на ред `sqlalchemy.url`:
```ini
sqlalchemy.url = sqlite:///company.db
# Или за PostgreSQL: postgresql+psycopg2://user:password@localhost/mydb
```

---

## 2. СВЪРЗВАНЕ НА ТВОИТЕ МОДЕЛИ В `ALEMBIC/ENV.PY`

За да може Alembic да засича автоматично промените по твоите модели, отвори **`alembic/env.py`**, намери реда `target_metadata = None` и го замени с твоя `Base.metadata`:

```python
# Във файла alembic/env.py:
from models import Base  # Импортираш базовия клас от твоя файл с модели!

target_metadata = Base.metadata
```

---

## 3. ГЕНЕРИРАНЕ И ПРИЛАГАНЕ НА МИГРАЦИИ (`REVISION` И `UPGRADE`)

```bash
# Стъпка 1: Автоматично генериране на миграционен файл в alembic/versions/
alembic revision --autogenerate -m "Create students and courses tables"

# Стъпка 2: Прилагане на всички миграции до последната версия (head)
alembic upgrade head
```

---

## 4. ВРЪЩАНЕ НАЗАД (`DOWNGRADE`) И СТРУКТУРА НА МИГРАЦИОНЕН ФАЙЛ

Всеки файл на Alembic има две функции: **`upgrade()`** (какво се прави напред) и **`downgrade()`** (как се отменя назад):

```python
# Примерен генериран файл в alembic/versions/8a9b12c_add_age.py:
from alembic import op
import sqlalchemy as sa

def upgrade() -> None:
    op.add_column("students", sa.Column("age", sa.Integer(), nullable=True))

def downgrade() -> None:
    op.drop_column("students", "age")
```

Команди за управление на версиите в терминала:
```bash
alembic current         # Показва коя версия е приложена в базата в момента
alembic history         # Показва цялата история на миграциите
alembic downgrade -1    # Връща базата с 1 миграция назад (изпълнява downgrade())
alembic downgrade base  # Отменя абсолютно всички миграции до началното празно състояние
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Команда в терминала | Какво прави |
| :--- | :--- |
| `alembic init alembic` | Създава структурата на Alembic в проекта |
| `alembic revision --autogenerate -m "msg"` | Създава нов миграционен файл от промените в моделите |
| `alembic upgrade head` | Прилага всички миграции до най-новата |
| `alembic upgrade +1` | Прилага само следващата 1 миграция |
| `alembic downgrade -1` | Връща базата с 1 миграция назад |
| `alembic downgrade base` | Изтрива всички промени (връща до нула) |
| `alembic current` | Показва текущата активна версия в базата |
| `alembic upgrade head --sql` | Принтира суровия SQL без да го изпълнява |

---
[⬅️ 04-02-01. Django ORM Syntax](04-02-01-Django-ORM-Syntax.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-03-00. CRUD and Queries](04-03-00-CRUD-And-Queries.md)
