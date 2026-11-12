[⬅️ 03-04-00. Теория & Капани](03-04-00-Spring-Data-Repositories.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-05-00. Advanced Querying (JPQL & N+1)](03-05-00-Advanced-Querying.md)

---

# 03-04-01. Spring Data Intro & Repositories: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за дефиниране на `JpaRepository` интерфейси, всички ключови думи за Derived Query Methods (`Containing`, `StartingWith`, `Between`, `In`, `OrderBy`, `Top`), навигация по вложени свойства и използване в Service слой в Java от нулата.

## 🧭 Съдържание на темата
* [1. Създаване на `JpaRepository` и най-честите Derived Query методи](#1-създаване-на-jparepository-и-най-честите-derived-query-методи)
* [2. Навигация по вложени релации (Property Expressions)](#2-навигация-по-вложени-релации-property-expressions)
* [3. Използване на `Repository` в `@Service` слой (CRUD операции)](#3-използване-на-repository-в-service-слой-crud-операции)
* [4. Таблица с всички ключови думи за Derived Queries](#4-таблица-с-всички-ключови-думи-за-derived-queries)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА `JPAREPOSITORY` И НАЙ-ЧЕСТИТЕ DERIVED QUERY МЕТОДИ

```java
import org.springframework.data.jpa.repository.JpaRepository;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.Collection;
import java.util.List;
import java.util.Optional;

public interface ProductRepository extends JpaRepository<Product, Long> {

    // 1. Точно съвпадение и проверка за съществуване
    Optional<Product> findBySku(String sku);
    boolean existsBySku(String sku);
    long countByCategoryName(String categoryName);

    // 2. Търсене в текст (LIKE %keyword% без значение от главни/малки букви)
    List<Product> findByNameContainingIgnoreCase(String keyword);
    List<Product> findByNameStartingWith(String prefix);

    // 3. Числови и времеви диапазони
    List<Product> findByPriceBetween(BigDecimal minPrice, BigDecimal maxPrice);
    List<Product> findByPriceGreaterThanEqualOrderByPriceAsc(BigDecimal minPrice);
    List<Product> findByCreatedAtAfter(LocalDateTime date);

    // 4. Списък от стойности (SQL IN (...)) и Null проверки
    List<Product> findByIdIn(Collection<Long> ids);
    List<Product> findByDeletedAtIsNull();

    // 5. Ограничаване на броя резултати (LIMIT N)
    Optional<Product> findFirstByOrderByPriceDesc(); // Най-скъпият продукт
    List<Product> findTop5ByIsAvailableTrueOrderByRatingDesc(); // Топ 5 налични по рейтинг
}
```

---

## 2. НАВИГАЦИЯ ПО ВЛОЖЕНИ РЕЛАЦИИ (PROPERTY EXPRESSIONS)

Spring Data позволява да преминаваш през обектни релации директно в името на метода (например `Order` има поле `customer`, което има поле `address`, което има поле `city`):

```java
public interface OrderRepository extends JpaRepository<Order, Long> {

    // Търси поръчки по полето 'email' на свързания обект 'customer'
    List<Order> findByCustomerEmail(String email);

    // Преминава през 2 нива: Order -> customer -> address -> city
    // Долната черта (_) може да се използва за яснота при двусмислие: findByCustomer_Address_City
    List<Order> findByCustomerAddressCity(String city);
}
```

---

## 3. ИЗПОЛЗВАНЕ НА `REPOSITORY` В `@SERVICE` СЛОЙ (CRUD ОПЕРАЦИИ)

```java
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.math.BigDecimal;

@Service
public class ProductService {

    private final ProductRepository productRepository;

    // Constructor Injection (Spring инжектира динамичното Proxy на ProductRepository)
    public ProductService(ProductRepository productRepository) {
        this.productRepository = productRepository;
    }

    @Transactional
    public Product createProduct(String sku, String name, BigDecimal price) {
        if (productRepository.existsBySku(sku)) {
            throw new IllegalArgumentException("Продукт с код " + sku + " вече съществува!");
        }
        Product product = new Product(sku, name, price);
        return productRepository.save(product);
    }

    @Transactional
    public void updatePrice(Long id, BigDecimal newPrice) {
        Product product = productRepository.findById(id)
                .orElseThrow(() -> new IllegalArgumentException("Продуктът не е намерен!"));
        // Dirty Checking автоматично ще изпълни UPDATE при края на метода!
        product.setPrice(newPrice);
    }

    @Transactional
    public void deleteProduct(Long id) {
        if (!productRepository.existsById(id)) {
            throw new IllegalArgumentException("Няма такъв продукт за изтриване!");
        }
        productRepository.deleteById(id);
    }
}
```

---

## 4. ТАБЛИЦА С ВСИЧКИ КЛЮЧОВИ ДУМИ ЗА DERIVED QUERIES

| Ключова дума в метода | Генериран SQL еквивалент | Пример |
| :--- | :--- | :--- |
| **`And` / `Or`** | `WHERE a = ? AND b = ?` / `OR` | `findByEmailAndActiveTrue` |
| **`Between`** | `WHERE price BETWEEN ? AND ?` | `findByPriceBetween(min, max)` |
| **`LessThan` / `GreaterThan`** | `WHERE age < ?` / `WHERE age > ?` | `findByAgeGreaterThan(18)` |
| **`IsNull` / `IsNotNull`** | `WHERE deleted_at IS NULL` / `IS NOT NULL` | `findByDeletedAtIsNull()` |
| **`Like` / `Containing`** | `WHERE name LIKE ?` / `LIKE '%?%'` | `findByNameContaining("Pro")` |
| **`StartingWith` / `EndingWith`** | `LIKE '?%'` / `LIKE '%?'` | `findByCodeStartingWith("BG")` |
| **`IgnoreCase`** | `WHERE LOWER(name) = LOWER(?)` | `findByEmailIgnoreCase(email)` |
| **`OrderBy...Asc` / `Desc`** | `ORDER BY created_at DESC` | `findByActiveTrueOrderByNameAsc()` |
| **`True` / `False`** | `WHERE is_verified = true` | `findByIsVerifiedTrue()` |
| **`In` / `NotIn`** | `WHERE status IN (?, ?)` | `findByStatusIn(List.of(...))` |

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод на `JpaRepository<T, ID>` | Връщан тип | Какво прави |
| :--- | :--- | :--- |
| **`save(entity)`** | `T` | Прави `INSERT` (ако `id == null`) или `UPDATE` (`merge`) |
| **`saveAll(iterable)`** | `List<T>` | Записва колекция от обекти |
| **`findById(id)`** | `Optional<T>` | Търси по Primary Key и връща `Optional` |
| **`getReferenceById(id)`** | `T` | Връща мързелив Proxy обект без `SELECT` заявка |
| **`findAll()` / `findAll(Sort)`** | `List<T>` | Връща всички редове (сортирани по желание) |
| **`existsById(id)`** | `boolean` | Проверява дали съществува запис с това ID |
| **`count()`** | `long` | Връща общия брой записи в таблицата |
| **`deleteById(id)`** | `void` | Изтрива запис по ID (хвърля грешка, ако липсва) |
| **`deleteAllInBatch()`** | `void` | Изтрива всички редове с **една** `DELETE` заявка |

---
[⬅️ 03-04-00. Теория & Капани](03-04-00-Spring-Data-Repositories.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-05-00. Advanced Querying (JPQL & N+1)](03-05-00-Advanced-Querying.md)
