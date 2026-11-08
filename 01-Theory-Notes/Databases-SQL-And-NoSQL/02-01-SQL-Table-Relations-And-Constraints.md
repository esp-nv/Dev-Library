[⬅️ 02-00. Моделиране Overview](02-00-DATA-MODELING-OVERVIEW.md) | [📂 Глава 02-00. Проектиране и Моделиране на Бази Данни (SQL и NoSQL)](02-00-DATA-MODELING-OVERVIEW.md) | 🏠 [Библиотека БД](README.md)  | [➡️ 02-02. Нормализация](02-02-SQL-Database-Normalization.md)


---

# 02-01. Връзки между таблици, Ключове и Ограничения (Constraints)

**Цел:** Как се свързват таблиците (`1:1`, `1:Many`, `Many:Many`), как се пишат Primary и Foreign Keys в различните SQL бази (MS SQL, PostgreSQL, MySQL, SQLite) и как да избегнеш най-честите грешки при триене и проектиране.

## Съдържание на темата
* [1. Разлики при дефиниране на ключове по бази данни](#1-разлики-при-дефиниране-на-ключове-по-бази-данни)
* [2. Видове връзки с реални SQL примери (1:1, 1:Many, Many:Many)](#2-видове-връзки-с-реални-sql-примери-11-1many-manymany)
* [3. Ограничения при изтриване (ON DELETE CASCADE / RESTRICT / SET NULL)](#3-ограничения-при-изтриване-on-delete-cascade--restrict--set-null)
* [4. Чести грешки, Капани и Как да ги избегнем](#4-чести-грешки-капани-и-как-да-ги-избегнем)

---

## 1. РАЗЛИКИ ПРИ ДЕФИНИРАНЕ НА КЛЮЧОВЕ ПО БАЗИ ДАННИ

Всяка релационна база има различна ключова дума за автоматично нарастващ Primary Key и различен синтаксис за Foreign Key:

| Елемент | MS SQL Server (T-SQL) | PostgreSQL | MySQL / MariaDB | SQLite |
| :--- | :--- | :--- | :--- | :--- |
| **Auto-Increment думичка** | `IDENTITY(1,1)` | `GENERATED ALWAYS AS IDENTITY` *(или `SERIAL`)* | `AUTO_INCREMENT` | `AUTOINCREMENT` |
| **Дефиниция на PK** | `Id INT IDENTITY(1,1) PRIMARY KEY` | `id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY` | `id INT AUTO_INCREMENT PRIMARY KEY` | `id INTEGER PRIMARY KEY AUTOINCREMENT` *(само `INTEGER`!)* |
| **Inline Foreign Key** | `UserId INT FOREIGN KEY REFERENCES Users(Id)` | `user_id INT REFERENCES users(id)` | *(Не работи inline! Пише се на отделен ред)* | `user_id INTEGER REFERENCES users(id)` |
| **Именуван Foreign Key** | `CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)` | `CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(id)` | `CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(id)` | `CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(id)` |

```sql
-- ============================================================
-- СРАВНЕНИЕ НА СИНТАКСИСА: MS SQL SERVER СРЕЩУ MYSQL
-- ============================================================

-- 1. В MS SQL SERVER:
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 2. В MYSQL (Внимание: AUTO_INCREMENT и задължителен отделен ред за FOREIGN KEY!):
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(id)
);
```

---

## 2. ВИДОВЕ ВРЪЗКИ С РЕАЛНИ SQL ПРИМЕРИ (1:1, 1:MANY, MANY:MANY)

### А. Едно към Много (`1:Many`) — Най-честата връзка
**Правило:** Външният ключ (`Foreign Key`) винаги стои в таблицата от страната **„Много“** (един отдел има много служители ➔ `DepartmentId` стои в `Employees`).

```sql
CREATE TABLE Departments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    DepartmentId INT NOT NULL, -- Страната "Много" държи ключа!
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);
```

### Б. Едно към Едно (`1:1`) — Разделяне на редки или чувствителни данни
**Правило:** Външният ключ в дъщерната таблица трябва да е **`UNIQUE`** (или да е едновременно и нейният `PRIMARY KEY`), за да не може един потребител да има два профила.

```sql
CREATE TABLE UserProfiles (
    UserId INT PRIMARY KEY, -- Едновременно PK и FK -> гарантира 1:1 връзка!
    Bio NVARCHAR(500) NULL,
    AvatarUrl VARCHAR(255) NULL,
    CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### В. Много към Много (`Many:Many`) — Mapping (Свързваща) таблица
**Правило:** Релационните бази нямат директна връзка `Many:Many`. Създаваме трета таблица (напр. `StudentsCourses`), която има два `Foreign Key`-а и **съставен първичен ключ (`Composite Primary Key`)**, за да не може един студент да се запише два пъти в един и същи курс.

```sql
CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Courses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL
);

-- Mapping (Junction) таблица:
CREATE TABLE StudentsCourses (
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrolledOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY (StudentId, CourseId), -- Забранява дублирането на една и съща двойка!
    CONSTRAINT FK_SC_Students FOREIGN KEY (StudentId) REFERENCES Students(Id),
    CONSTRAINT FK_SC_Courses FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);
```

---

## 3. ОГРАНИЧЕНИЯ ПРИ ИЗТРИВАНЕ (ON DELETE CASCADE / RESTRICT / SET NULL)

Какво да направи базата, ако изтриеш родителски запис (напр. изтриеш потребител, който вече има 5 поръчки)?

| Правило (`ON DELETE ...`) | Какво се случва при `DELETE FROM Users WHERE Id = 1`? | Кога се използва? |
| :--- | :--- | :--- |
| **`NO ACTION` / `RESTRICT`** | **Блокира изтриването** и хвърля грешка (*Foreign Key constraint conflict*). | **По подразбиране.** За поръчки, фактури и финансови данни (не триеш клиент с плащания!). |
| **`CASCADE`** | Изтрива потребителя **И автоматично трие всичките му поръчки**. | За зависими данни без смисъл сами по себе си (напр. триеш публикация ➔ трият се и коментарите ѝ). |
| **`SET NULL`** | Изтрива потребителя, а в таблица `Orders` слага `UserId = NULL`. | Когато искаш да запазиш историята на продажбите за статистика (изисква `UserId INT NULL`). |
| **`SET DEFAULT`** | Слага предварително зададена стойност по подразбиране (напр. `UserId = 0` за „Изтрит потребител“). | Рядко — при архивни системи. |

```sql
-- Пример с ON DELETE CASCADE (ако изтрием публикация, трият се и коментарите):
CREATE TABLE Comments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    Content NVARCHAR(500) NOT NULL,
    CONSTRAINT FK_Comments_Posts FOREIGN KEY (PostId) 
        REFERENCES Posts(Id) ON DELETE CASCADE
);
```

---

## 4. ЧЕСТИ ГРЕШКИ, КАПАНИ И КАК ДА ГИ ИЗБЕГНЕМ

### Грешка 1: Inline `REFERENCES` в MySQL (Тихо игнориране)
* **Какво се случва:** В MySQL ако напишеш `user_id INT REFERENCES users(id)` вътре в реда на колоната, MySQL **не хвърля грешка**, но **изобщо не създава връзката**! Така в базата ти влизат несъществуващи `user_id = 99999` (Orphan records).
* **Как да избегнеш:** В MySQL винаги пиши `FOREIGN KEY (user_id) REFERENCES users(id)` на отделен ред най-отдолу в `CREATE TABLE`.

### Грешка 2: Разминаване на типовете между PK и FK
* **Какво се случва:** Правиш `Users.Id` да е `BIGINT` (или `INT UNSIGNED` в MySQL), а в таблица `Orders` правиш `UserId` да е обикновен `INT`. Базата хвърля грешка *Column 'Users.Id' is not the same data type as referencing column*.
* **Как да избегнеш:** Типът на `Foreign Key` колоната трябва да е **100% идентичен** с типа на `Primary Key` колоната (включително `UNSIGNED` в MySQL).

### Грешка 3: Липсващ индекс върху `Foreign Key` колоната
* **Какво се случва:** Базата автоматично прави индекс за `PRIMARY KEY`, но **не прави автоматично индекс за `FOREIGN KEY`**! Когато таблицата `Orders` стане 1 милион реда, заявката `SELECT * FROM Orders WHERE UserId = 5` или `DELETE FROM Users WHERE Id = 5` става много бавна, защото сканира цялата таблица.
* **Как да избегнеш:** Създавай индекс върху всяка `Foreign Key` колона (`CREATE INDEX IX_Orders_UserId ON Orders(UserId);`). *(EF Core го прави автоматично при миграции, но на чист SQL трябва да го напишеш ти!)*

### Грешка 4: Множествени `CASCADE` пътища в MS SQL Server
* **Какво се случва:** Ако имаш два пътя на триене към една и съща таблица (напр. `Users ➔ Projects ➔ Tasks` и `Users ➔ Tasks`), MS SQL Server хвърля грешка *May cause cycles or multiple cascade paths*.
* **Как да избегнеш:** Сложи `ON DELETE CASCADE` само на единия път, а на другия остави `ON DELETE NO ACTION` (или ползвай Soft Delete `IsDeleted = 1`).

---
[⬅️ 02-00. Моделиране Overview](02-00-DATA-MODELING-OVERVIEW.md) | [📂 Глава 02-00. Проектиране и Моделиране на Бази Данни (SQL и NoSQL)](02-00-DATA-MODELING-OVERVIEW.md) | 🏠 [Библиотека БД](README.md)  | [➡️ 02-02. Нормализация](02-02-SQL-Database-Normalization.md)

© 2026 Dev-Library / Бази данни: Връзки между таблици и Ограничения
