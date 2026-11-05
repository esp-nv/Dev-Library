### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го в SQL Server Management Studio (SSMS) или в твоите .NET миграции.
- Модифицирай: Имената на таблиците (`Users`), колоните и типовете данни.

### 🛠️ ГОТОВ КОД (SQL)
```sql
-- 1. СЪЗДАВАНЕ НА ТАБЛИЦА С КЛЮЧОВЕ И ВАЛИДАЦИЯ
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Автоматично увеличаващо се ID
    Username NVARCHAR(50) NOT NULL UNIQUE, -- Текст до 50 знака, задължителен, уникален
    Age INT NOT NULL CHECK (Age >= 18), -- Валидация на ниво база данни
    CreatedAt DATETIME DEFAULT GETDATE() -- Автоматична дата на създаване
);

-- 2. ДОБАВЯНЕ НА ДАННИ (Create)
INSERT INTO Users (Username, Age) 
VALUES ('DevUser', 25);

-- 3. ЧЕТЕНЕ И ФИЛТРИРАНЕ НА ДАННИ (Read)
SELECT Id, Username, Age 
FROM Users 
WHERE Age >= 20 
ORDER BY Username ASC;

-- 4. ОБНОВЯВАНЕ НА ДАННИ (Update) - ВИНАГИ с WHERE, за да не счупиш базата!
UPDATE Users 
SET Age = 26 
WHERE Username = 'DevUser';

-- 5. ИЗТРИВАНЕ НА ДАННИ (Delete) - ВИНАГИ с WHERE!
DELETE FROM Users 
WHERE Id = 1;
```

---


