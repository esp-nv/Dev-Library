[⬅️ 03-01-00. Теория & Капани](03-01-00-JDBC-Introduction.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-02-00. Introduction to Hibernate](03-02-00-Hibernate-Intro.md)

---

# 03-01-01. DB Apps Introduction (JDBC): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за свързване с PostgreSQL/MySQL чрез JDBC, изпълнение на `SELECT`, `INSERT` с връщане на генерираното `ID`, Batch вмъкване и ръчни транзакции в Java от нулата.

## 🧭 Съдържание на темата
* [1. Свързване с база данни и `INSERT` с генерирано ID](#1-свързване-с-база-данни-и-insert-с-генерирано-id)
* [2. Batch Processing и ръчни транзакции (`setAutoCommit(false)`)](#2-batch-processing-и-ръчни-транзакции-setautocommitfalse)
* [3. Обобщение / Бърз Cheat Sheet](#3-обобщение--бърз-cheat-sheet)

---

## 1. СВЪРЗВАНЕ С БАЗА ДАННИ И `INSERT` С ГЕНЕРИРАНО ID

```java
import java.sql.*;

public class DbOperationsDemo {
    public static Connection getConnection() throws SQLException {
        return DriverManager.getConnection("jdbc:postgresql://localhost:5432/shop_db", "postgres", "secret_pass");
    }

    public static long insertProduct(Connection conn, String name, double price) throws SQLException {
        String sql = "INSERT INTO products (name, price) VALUES (?, ?)";

        try (PreparedStatement ps = conn.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
            ps.setString(1, name);
            ps.setDouble(2, price);

            int affectedRows = ps.executeUpdate();
            if (affectedRows == 0) {
                throw new SQLException("Създаването на продукта се провали!");
            }

            try (ResultSet generatedKeys = ps.getGeneratedKeys()) {
                if (generatedKeys.next()) {
                    return generatedKeys.getLong(1);
                }
                throw new SQLException("Няма върнато ID.");
            }
        }
    }
}
```

---

## 2. BATCH PROCESSING И РЪЧНИ ТРАНЗАКЦИИ (`SETAUTOCOMMIT(FALSE)`)

```java
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.List;

public class BatchAndTransactionDemo {
    public static void insertUsersBatch(Connection conn, List<String> usernames) throws SQLException {
        String sql = "INSERT INTO users (username) VALUES (?)";

        try (PreparedStatement ps = conn.prepareStatement(sql)) {
            for (String user : usernames) {
                ps.setString(1, user);
                ps.addBatch();
            }
            ps.executeBatch();
        }
    }

    public static void transferMoney(Connection conn, long fromId, long toId, double amount) throws SQLException {
        try {
            conn.setAutoCommit(false);

            try (PreparedStatement withdraw = conn.prepareStatement("UPDATE accounts SET balance = balance - ? WHERE id = ?");
                 PreparedStatement deposit = conn.prepareStatement("UPDATE accounts SET balance = balance + ? WHERE id = ?")) {

                withdraw.setDouble(1, amount);
                withdraw.setLong(2, fromId);
                withdraw.executeUpdate();

                deposit.setDouble(1, amount);
                deposit.setLong(2, toId);
                deposit.executeUpdate();
            }

            conn.commit();
        } catch (SQLException e) {
            conn.rollback();
            throw e;
        } finally {
            conn.setAutoCommit(true);
        }
    }
}
```

---

## 3. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод / Клас в `java.sql` | Какво прави | Важна особеност |
| :--- | :--- | :--- |
| **`DriverManager.getConnection(url, u, p)`** | Отваря връзка към базата данни | Затваряй през `try-with-resources` |
| **`conn.prepareStatement(sql)`** | Създава защитена параметризирана заявка | Предпазва от SQL Injection |
| **`ps.setInt(1, val)` / `ps.setString(2, val)`** | Попълва параметрите `?` | Номерацията започва от **`1`** |
| **`ps.executeQuery()` / `ps.executeUpdate()`** | Изпълнява `SELECT` или `INSERT/UPDATE/DELETE` | Връща `ResultSet` или брой засегнати редове |
| **`rs.next()`** | Премества курсора на следващия ред | Връща `false`, когато свършат редовете |

---
[⬅️ 03-01-00. Теория & Капани](03-01-00-JDBC-Introduction.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-02-00. Introduction to Hibernate](03-02-00-Hibernate-Intro.md)
