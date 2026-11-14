[⬅️ 03-05-00. Теория & Капани](03-05-00-Advanced-Querying.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-06-00. Auto Mapping & DTOs](03-06-00-Auto-Mapping-DTOs.md)

---

# 03-05-01. Spring Data Advanced Querying: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за JPQL заявки с `@Query`, нативен SQL (`nativeQuery = true`), масови операции с `@Modifying`, DTO/Interface Projections, странициране (`Pageable` / `Page<T>`) и динамични спецификации в Java от нулата.

## 🧭 Съдържание на темата
* [1. JPQL и Native SQL заявки с именувани параметри (`@Query` & `@Param`)](#1-jpql-и-native-sql-заявки-с-именувани-параметри-query--param)
* [2. Масови `UPDATE` и `DELETE` операции (`@Modifying`)](#2-масови-update-и-delete-операции-modifying)
* [3. Проекции (Interface Projection и `record` Constructor Expression)](#3-проекции-interface-projection-и-record-constructor-expression)
* [4. Странициране и сортиране (`Pageable`, `PageRequest`, `Page<T>`)](#4-странициране-и-сортиране-pageable-pagerequest-paget)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. JPQL И NATIVE SQL ЗАЯВКИ С ИМЕНУВАНИ ПАРАМЕТРИ (`@QUERY` & `@PARAM`)

```java
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import java.math.BigDecimal;
import java.util.List;

public interface BookRepository extends JpaRepository<Book, Long> {

    // 1. JPQL заявка (използва името на Java класа 'Book' и полетата му!)
    @Query("SELECT b FROM Book b WHERE b.price >= :minPrice AND LOWER(b.title) LIKE LOWER(CONCAT('%', :keyword, '%'))")
    List<Book> searchExpensiveBooks(@Param("minPrice") BigDecimal minPrice,
                                    @Param("keyword") String keyword);

    // 2. JPQL с JOIN FETCH (зарежда автора и книгите в 1 SQL заявка -> без N+1!)
    @Query("SELECT b FROM Book b JOIN FETCH b.author WHERE b.id = :id")
    Book findByIdWithAuthor(@Param("id") Long id);

    // 3. Нативен SQL (използва реалните имена на таблици и колони в PostgreSQL/MySQL!)
    @Query(value = "SELECT * FROM books WHERE published_year = :year ORDER BY rating DESC LIMIT 5",
           nativeQuery = true)
    List<Book> findTop5ByYearNative(@Param("year") int year);
}
```

---

## 2. МАСОВИ `UPDATE` И `DELETE` ОПЕРАЦИИ (`@MODIFYING`)

Всяка `@Modifying` заявка изисква активна транзакция (`@Transactional` в Service или Repository слоя):

```java
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.transaction.annotation.Transactional;

public interface UserAccountRepository extends JpaRepository<UserAccount, Long> {

    @Transactional
    @Modifying(clearAutomatically = true, flushAutomatically = true)
    @Query("UPDATE UserAccount u SET u.isActive = false WHERE u.lastLoginDays > :days")
    int deactivateInactiveUsers(@Param("days") int days); // Връща броя обновени редове!

    @Transactional
    @Modifying(clearAutomatically = true)
    @Query("DELETE FROM UserAccount u WHERE u.isActive = false AND u.verified = false")
    int purgeUnverifiedAccounts();
}
```

---

## 3. ПРОЕКЦИИ (INTERFACE PROJECTION И `RECORD` CONSTRUCTOR EXPRESSION)

```java
// Начин А: Closed Interface Projection (Spring генерира имплементацията автоматично)
public interface BookTitleAndAuthorView {
    String getTitle();
    String getAuthorName(); // Мапва автоматично book.getAuthor().getName()!
}

// Начин Б: DTO Record Projection
public record BookSummaryDto(Long id, String title, BigDecimal price) {}

public interface BookProjectionRepository extends JpaRepository<Book, Long> {
    // Използва Начин А
    List<BookTitleAndAuthorView> findAllByPriceLessThan(BigDecimal maxPrice);

    // Използва Начин Б (пълен път до пакета на record-а)
    @Query("SELECT new bg.softuni.library.dto.BookSummaryDto(b.id, b.title, b.price) FROM Book b")
    List<BookSummaryDto> findAllBookSummaries();
}
```

---

## 4. СТРАНИЦИРАНЕ И СОРТИРАНЕ (`PAGEABLE`, `PAGEREQUEST`, `PAGE<T>`)

```java
import org.springframework.data.domain.*;
import org.springframework.stereotype.Service;

// В Repository интерфейса:
// Page<Book> findByGenre(String genre, Pageable pageable);

@Service
public class BookCatalogService {
    private final BookRepository bookRepository;

    public BookCatalogService(BookRepository bookRepository) {
        this.bookRepository = bookRepository;
    }

    public Page<Book> getBooksPage(int pageNumber, int pageSize) {
        // Внимание: в Spring Data номерацията на страниците започва от 0!
        Sort sort = Sort.by(Sort.Direction.DESC, "price")
                        .and(Sort.by(Sort.Direction.ASC, "title"));

        Pageable pageable = PageRequest.of(pageNumber, pageSize, sort);
        Page<Book> page = bookRepository.findAll(pageable);

        // Полезни метаданни от обекта Page<T>:
        // page.getContent()       -> List<Book> за текущата страница
        // page.getTotalElements() -> Общ брой записи в базата (long)
        // page.getTotalPages()    -> Общ брой страници (int)
        // page.hasNext()          -> Има ли следваща страница (boolean)
        return page;
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Клас | Предназначение | Бележка |
| :--- | :--- | :--- |
| **`@Query("SELECT e FROM Entity e ...")`** | Дефинира JPQL заявка | Използва имената на Java класовете и полетата |
| **`@Query(value = "...", nativeQuery = true)`** | Дефинира чист SQL за конкретната БД | Внимание: `Pageable` изисква и `countQuery` при нативен SQL! |
| **`@Param("name")`** | Свързва параметър на метода с `:name` в заявката | Прави кода устойчив при пренареждане на аргументи |
| **`@Modifying(clearAutomatically = true)`** | Задължително за `UPDATE` и `DELETE` в `@Query` | Изчиства остарелите обекти от L1 кеша |
| **`@EntityGraph(attributePaths = {"rel"})`** | Зарежда `LAZY` релации с `LEFT JOIN` в 1 заявка | По-чиста алтернатива на `JOIN FETCH` |
| **`PageRequest.of(page, size, sort)`** | Създава обект `Pageable` за странициране | Първата страница винаги е индекс **`0`** |

---
[⬅️ 03-05-00. Теория & Капани](03-05-00-Advanced-Querying.md) | 📂 Папка: Module-03-Spring-Data | [🏠 Начало](../../README.md) | [➡️ 03-06-00. Auto Mapping & DTOs](03-06-00-Auto-Mapping-DTOs.md)