[⬅️ 03-00. Intro to Spring Data](03-00-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-01-01. Syntax From Scratch](03-01-01-Syntax-From-Scratch.md)

---

# 03-01-00. DB Apps Introduction (JDBC, `PreparedStatement` & HikariCP — Теория & Капани)

**Модул 03:** Spring Data | **Лекционна тема 01:** Как Java комуникира директно с релационни бази данни чрез **JDBC (Java Database Connectivity)**, петте стъпки на JDBC заявката, разликата между `Statement` и `PreparedStatement` (защита от SQL Injection), Connection Pooling с **HikariCP** и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Мнемоника и основни интерфейси (`Connection`, `PreparedStatement`, `ResultSet`, `HikariCP`)](#1-мнемоника-и-основно-разпределение)
* [2. Сравнителна карта в 4 езика](#2-сравнителна-карта-в-4-езика)
* [3. Плюсове, минуси и код за защита от SQL Injection](#3-плюсове-минуси-и-код-за-защита-от-sql-injection)
* [4. Интервю капани и сравнение Java 21+ спрямо C# 12+](#4-интервю-капани-и-сравнение-java-21-спрямо-c-12)

---

## 1. МНЕМОНИКА И ОСНОВНО РАЗПРЕДЕЛЕНИЕ

*   **`Connection`:** Телефонната линия до базата данни.
*   **`PreparedStatement`:** Компилиран шаблон за SQL заявка с параметри `?`.
*   **`ResultSet`:** Табличен курсор с върнати редове (изисква `rs.next()`, тъй като курсорът сочи преди първия ред).
*   **HikariCP (Connection Pool):** Поддържа пул от предварително отворени TCP връзки, спестявайки време за инициализация.

---

## 2. СРАВНИТЕЛНА КАРТА В 4 ЕЗИКА

| Концепция | ☕ Java 21+ (JDBC) | 🟣 C# 12+ (ADO.NET / Npgsql) | 🐍 Python 3.x (`psycopg` / `sqlite3`) | 🟡 JavaScript (`pg` / `mysql2`) |
| :--- | :--- | :--- | :--- | :--- |
| **Обект за връзка** | `Connection` (`DriverManager`) | `SqlConnection` / `NpgsqlConnection` | `conn = psycopg.connect(...)` | `await client.connect()` |
| **Подготвена заявка** | `PreparedStatement` | `SqlCommand` | `cursor = conn.cursor()` | `pool.query(sql, params)` |
| **Синтаксис за параметри** | Позиционни `?` (`setString(1, x)`) | Именувани `@name` | `%s` или `%(name)s` | `$1, $2` или `?` |
| **Четене на редове** | `ResultSet` (`while (rs.next())`) | `SqlDataReader` (`while (reader.Read())`) | `cursor.fetchall()` / `fetchone()` | `const { rows } = await pool.query(...)` |

---

## 3. ПЛЮСОВЕ, МИНУСИ И КОД ЗА ЗАЩИТА ОТ SQL INJECTION

*   **Плюсове:** Максимална скорост и пълен контрол върху SQL, идеален за Batch Processing.
*   **Минуси:** Голям обем повтарящ се код (boilerplate) при мапване на `ResultSet`.
*   **Безопасен код с `PreparedStatement` и `try-with-resources`:**

```java
import java.sql.*;

public class SafeJdbcQuery {
    public static void findUserByEmail(String url, String user, String pass, String email) {
        String sql = "SELECT id, username, email FROM users WHERE email = ?";

        try (
            Connection conn = DriverManager.getConnection(url, user, pass);
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, email); // Индексите в JDBC започват от 1

            try (ResultSet rs = ps.executeQuery()) {
                while (rs.next()) {
                    long id = rs.getLong("id");
                    String username = rs.getString("username");
                    System.out.printf("Намерен: #%d - %s%n", id, username);
                }
            }
        } catch (SQLException e) {
            throw new RuntimeException("Грешка при заявка към базата данни", e);
        }
    }
}
```

---

## 4. ИНТЕРВЮ КАПАНИ И СРАВНЕНИЕ JAVA 21+ СПРЯМО C# 12+

*   **Интервю уловка:** `PreparedStatement` разделя SQL структурата от данните на ниво Execution Plan в базата данни, поради което входни данни с кавички или SQL команди се третират стриктно като низове.
*   **Java 21+ спрямо C# 12+:** В JDBC индексите на параметрите и колоните започват от **`1`**, докато в ADO.NET колоните започват от **`0`**. В Spring Framework 6.1+ съществува модерен Fluent API клиент **`JdbcClient`** (аналог на Dapper в .NET).

---
[⬅️ 03-00. Intro to Spring Data](03-00-Intro.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-01-01. Syntax From Scratch](03-01-01-Syntax-From-Scratch.md)
