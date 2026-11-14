[⬅️ Назад към Java Хъба](README.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-97. Category Cheat Sheet](04-97-Category-Cheat-Sheet.md)

---

# 04-96. Java 21+ & Spring Boot 3 Оптимизации: Кое е грешно, защо и как е по-добре (с Блок-схеми)

**Цел:** Инженерен анализ на най-честите тесни места в производителността на **Java 21+ (JVM)** и **Spring Boot 3 / Hibernate**, придружен с визуални блок-схеми за паметта (Heap / GC), SQL заявките и конкурентността (Virtual Threads).

## 🧭 Съдържание на темата
* [1. Оптимизация 1: Конкатенация на низове в цикъл (`+`) срещу `StringBuilder` (Heap & GC)](#1-оптимизация-1-конкатенация-на-низове-в-цикъл--срещу-stringbuilder-heap--gc)
* [2. Оптимизация 2: Hibernate N+1 заявки срещу `JOIN FETCH` и `@EntityGraph` (SQL I/O)](#2-оптимизация-2-hibernate-n1-заявки-срещу-join-fetch-и-entitygraph-sql-io)
* [3. Оптимизация 3: Автобоксинг в цикли (`List<Integer>` / `Stream<Integer>`) срещу `IntStream` и примитиви](#3-оптимизация-3-автобоксинг-в-цикли-listinteger--streaminteger-срещу-intstream-и-примитиви)
* [4. Оптимизация 4: Блокиращи нишки на ОС срещу Java 21 Virtual Threads (`Project Loom`)](#4-оптимизация-4-блокиращи-нишки-на-ос-срещу-java-21-virtual-threads-project-loom)

---

## 1. ОПТИМИЗАЦИЯ 1: КОНКАТЕНАЦИЯ НА НИЗОВЕ В ЦИКЪЛ (`+`) СРЕЩУ `STRINGBUILDER` (HEAP & GC)

### Блок-схема на JVM Heap паметта:
```text
❌ БАВЕН ПОДХОД (result += item в цикъл — Неизменяеми String обекти в Heap):
   Итерация 1: Алокира String("A") в Heap
   Итерация 2: Алокира нов StringBuilder + копира + алокира нов String("AB") ──► Старият става боклук!
   ...
   Итерация 50 000: Създава 100 000+ временни обекта ──► Чести Stop-The-World паузи на Garbage Collector!

✅ БЪРЗ ПОДХОД (Един обект StringBuilder с предварително зададен капацитет):
   new StringBuilder(estimatedSize) ──► Пише директно в един вътрешен byte[] масив ──► 1 String накрая!
```

### Кодово сравнение:
```java
// ❌ ГРЕШНО И БАВНО: В цикъл компилаторът не може да обедини конкатенациите
public String buildReportSlow(List<String> lines) {
    String report = "";
    for (String line : lines) {
        report += line + "\n";
    }
    return report;
}

// ✅ ПРАВИЛНО И ОПТИМИЗИРАНО: Използва 1 буфер в паметта
public String buildReportFast(List<String> lines) {
    StringBuilder sb = new StringBuilder(lines.size() * 32);
    for (String line : lines) {
        sb.append(line).append('\n');
    }
    return sb.toString();
}
```
*   **Защо е по-добре:** Типът `String` в Java е неизменяем (*immutable*). Вътре в цикъл операторът `+=` заделя нов `StringBuilder` и нов `String` при всяко завъртане, натоварвайки излишно Garbage Collector-а.

---

## 2. ОПТИМИЗАЦИЯ 2: HIBERNATE N+1 ЗАЯВКИ СРЕЩУ `JOIN FETCH` И `@ENTITYGRAPH` (SQL I/O)

### Блок-схема на мрежовите заявки към PostgreSQL / MySQL:
```text
❌ БАВЕН ПОДХОД (N+1 Problem при FetchType.LAZY без JOIN FETCH):
   1. SELECT * FROM authors;                  ──► Връща 100 автора (1 заявка)
   2. За всеки автор при author.getBooks():
      SELECT * FROM books WHERE author_id = 1; ──► +100 отделни мрежови заявки към БД! (101 общо!)

✅ БЪРЗ ПОДХОД (JPQL JOIN FETCH или @EntityGraph):
   1. SELECT a FROM Author a LEFT JOIN FETCH a.books; ──► Всичко се зарежда с 1 ЕДИНСТВЕНА SQL заявка!
```

### Кодово сравнение:
```java
public interface AuthorRepository extends JpaRepository<Author, Long> {

    // ❌ АКО ПОЛЗВАШ стандартния findAll() и после обхождаш author.getBooks() в цикъл -> 101 SQL заявки!

    // ✅ ПРАВИЛНО: Една-единствена SQL заявка зарежда авторите заедно с книгите им
    @Query("SELECT DISTINCT a FROM Author a LEFT JOIN FETCH a.books")
    List<Author> findAllWithBooks();
}
```
*   **Защо е по-добре:** Всяка отделна SQL заявка носи мрежова латентност (*Round-Trip Time* между приложението и базата). Обединяването в `JOIN FETCH` намалява времето за отговор от стотици милисекунди на единици милисекунди.

---

## 3. ОПТИМИЗАЦИЯ 3: АВТОБОКСИНГ В ЦИКЛИ (`LIST<INTEGER>` / `STREAM<INTEGER>`) СРЕЩУ `INTSTREAM` И ПРИМИТИВИ

### Блок-схема на паметта (Heap срещу Stack / CPU регистри):
```text
❌ БАВЕН ПОДХОД (Autoboxing с Stream<Integer> / List<Integer>):
   int (4 байта) ──► Опаковане в обект Integer (16 байта в Heap + указател) при ВСЯКО събиране!
   При 1 000 000 числа: 1 000 000 обекта Integer в Heap + разпръсната памет (CPU Cache Misses)!

✅ БЪРЗ ПОДХОД (Примитивни потоци IntStream / LongStream / DoubleStream):
   Работи директно с 32-битови/64-битови примитиви в CPU регистрите ──► 0 Heap алокации!
```

### Кодово сравнение:
```java
import java.util.List;
import java.util.stream.IntStream;

public class StreamOptimizationDemo {

    // ❌ ПО-БАВНО: Всяка операция разопакова (unbox) и опакова (box) Integer обекти в Heap
    public long sumSlow(List<Integer> numbers) {
        return numbers.stream()
                .reduce(0, Integer::sum);
    }

    // ✅ ОПТИМИЗИРАНО: Примитивен поток без създаване на Integer обекти
    public long sumFast(int[] numbers) {
        return IntStream.of(numbers)
                .asLongStream()
                .sum();
    }
}
```
*   **Защо е по-добре:** Примитивните потоци (`IntStream`, `LongStream`, `DoubleStream`) елиминират разходите за създаване на Wrapper обекти (`Integer`, `Long`) и позволяват на JIT компилатора на JVM да приложи SIMD векторизация на процесора.

---

## 4. ОПТИМИЗАЦИЯ 4: БЛОКИРАЩИ НИШКИ НА ОС СРЕЩУ JAVA 21 VIRTUAL THREADS (`PROJECT LOOM`)

### Блок-схема на нишките при I/O изчакване (HTTP / SQL заявки):
```text
❌ СТАР ПОДХОД (Platform Threads — 1 заявка = 1 тежка нишка на ОС ~1 MB RAM):
   200 едновременни заявки, чакащи база данни ──► Всички 200 нишки в Tomcat са БЛОКИРАНИ!
   Заявка №201 чака на опашка, въпреки че CPU натоварването е едва 5%!

✅ МОДЕРЕН ПОДХОД (Java 21 Virtual Threads — spring.threads.virtual.enabled=true):
   100 000 виртуални нишки (~1 KB RAM всяка) ──► При чакане на SQL/HTTP виртуалната нишка се
   откача мигновено от OS Carrier Thread, за да обслужи друга заявка!
```

### Кодово сравнение:
```properties
# ✅ В Spring Boot 3.2+ с Java 21+ активираш Virtual Threads за целия Tomcat и @Async с 1 ред:
spring.threads.virtual.enabled=true
```
*   **Защо е по-добре:** При микросървиси и REST API-та, които прекарват 90% от времето си в изчакване на PostgreSQL, Redis или външни HTTP услуги, Virtual Threads увеличават пропускателната способност (**Throughput**) многократно без нужда от сложен реактивен код (WebFlux).

---

## 5. БЪРЗ ОДИТ НА ОПТИМИЗАЦИИТЕ В JAVA 21+ & SPRING BOOT 3

| Сценарий | Бавен подход | Оптимизиран подход | Печалба |
| :--- | :--- | :--- | :--- |
| **Сглобяване на текст в цикъл** | `str += part` | `StringBuilder` с начален капацитет | От $O(n^2)$ на $O(n)$ и минимален GC |
| **Четене на релации в JPA** | `FetchType.LAZY` в цикъл ($N+1$) | `JOIN FETCH` или `@EntityGraph` | От $N+1$ заявки към БД на 1 заявка |
| **Само четене (Read-only) в Spring** | Стандартен `@Transactional` | `@Transactional(readOnly = true)` + DTO проекция | Изключва Hibernate Dirty Checking и пести RAM |
| **Математически потоци** | `Stream<Integer>` / `Stream<Double>` | `IntStream` / `DoubleStream` | 0 Heap алокации (без Autoboxing) |
| **Висока I/O конкурентност** | Стандартен пул от 200 Platform нишки | `spring.threads.virtual.enabled=true` (Java 21+) | Хиляди едновременни заявки с ниска RAM |
| **Масово записване (Batch Insert)** | `repository.save()` в цикъл | `repository.saveAll()` + `hibernate.jdbc.batch_size=50` | Групира SQL `INSERT` пакетите в мрежата |

---
[⬅️ Назад към Java Хъба](README.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-97. Category Cheat Sheet](04-97-Category-Cheat-Sheet.md)
