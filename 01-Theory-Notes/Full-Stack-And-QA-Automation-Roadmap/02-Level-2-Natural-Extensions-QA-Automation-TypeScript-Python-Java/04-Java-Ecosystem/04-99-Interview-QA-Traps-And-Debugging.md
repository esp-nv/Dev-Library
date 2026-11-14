[⬅️ 04-98. Java & Spring Common Errors & Fixes](04-98-Common-Errors-And-Fixes.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 01-QA-Automation](../01-QA-Automation/README.md)

---

# 04-99. Java 21+ & Spring Boot 3 Interview Q&A, Скрити Капани & Debugging Techniques

**Цел:** Подготовка за технически интервюта по Java & Spring Boot (от Junior до Senior / Backend / SDET ниво), критични подводни камъни в JVM паметта, Hibernate и Spring прокситата, както и професионални техники за дебъгване и диагностика в IntelliJ IDEA и продукционни JVM контейнери.

## 🧭 Съдържание на темата
* [1. Интервю въпросник (Top 6 въпроса с точни Senior отговори)](#1-интервю-въпросник-top-6-въпроса-с-точни-senior-отговори)
* [2. Скрити капани в Java & Spring (`Integer` Cache, `equals`/`hashCode`, `@Transactional` Self-Invocation)](#2-скрити-капани-в-java--spring-integer-cache-equalshashcode-transactional-self-invocation)
* [3. Debugging & Profiling Techniques в Java & Spring Boot (IntelliJ, SQL Logs, `jstack`, Heap Dump)](#3-debugging--profiling-techniques-в-java--spring-boot-intellij-sql-logs-jstack-heap-dump)

---

## 1. ИНТЕРВЮ ВЪПРОСНИК (TOP 6 ВЪПРОСА С ТОЧНИ SENIOR ОТГОВОРИ)

1. **Въпрос:** *„Каква е разликата между **`HashMap`**, **`LinkedHashMap`** и **`ConcurrentHashMap`**?“*
   * **Отговор:**
     * **`HashMap`:** Не пази реда на вмъкване, допуска един `null` ключ, $O(1)$ средна сложност (при колизии над 8 елемента в един bucket превръща списъка в червено-черно дърво с $O(\log n)$ от Java 8+), но **не е потокобезопасен**.
     * **`LinkedHashMap`:** Пази точния ред на вмъкване (или ред на достъп при LRU кеш) чрез двойно свързан списък.
     * **`ConcurrentHashMap`:** Потокобезопасен, заключва само конкретния bucket (CAS + `synchronized` на ниво възел) вместо цялата карта, не допуска `null` ключове или стойности.

2. **Въпрос:** *„Как работят **JVM Heap (Young/Old Generation)** и Garbage Collector-ите (**G1GC** / **ZGC**)?“*
   * **Отговор:** Новите обекти се алокират в **Eden** пространството на Young Generation. Оцелелите след Minor GC преминават през Survivor пространствата (`S0`/`S1`) и при достигане на определен праг се преместват в **Old Generation (Tenured)**. G1GC разделя Heap-а на малки региони, а **Generational ZGC** (в Java 21+) извършва почистването конкурентно с паузи под **1 милисекунда** дори при стотици гигабайти памет.

3. **Въпрос:** *„Каква е разликата между **`@Component`**, **`@Service`** и **`@Repository`** в Spring?“*
   * **Отговор:** И трите регистрират класа като Spring Bean чрез Component Scanning. Разликата е семантична и функционална за `@Repository` — Spring автоматично обвива `@Repository` класовете в **PersistenceExceptionTranslationPostProcessor**, който превежда нисконивовите JDBC/Hibernate `SQLException` грешки в единната Spring йерархия **`DataAccessException`**.

4. **Въпрос:** *„Какво е **Persistence Context (First-Level Cache)** в Hibernate и кога се изпълнява реалният SQL `UPDATE`?“*
   * **Отговор:** Persistence Context е кеш на ниво транзакция (`EntityManager`), който следи всички заредени `@Entity` обекти (**Dirty Checking**). Когато промениш поле на управляван обект (`user.setEmail(...)`) вътре в `@Transactional` метод, не е нужно да викаш `repository.save()` — при приключване на транзакцията (*Flush*) Hibernate сравнява текущото състояние със снимката в паметта и генерира SQL `UPDATE` автоматично.

5. **Въпрос:** *„Каква е разликата между **Spring Security Authentication** и **Authorization**, и каква е разликата между `401` и `403`?“*
   * **Отговор:** **Authentication (401 Unauthorized)** проверява *„Кой си ти?“* (валиден JWT или парола). **Authorization (403 Forbidden)** проверява *„Имаш ли права за този ресурс?“* (напр. потребителят е логнат с роля `USER`, но се опитва да достъпи `/admin` с `@PreAuthorize("hasRole('ADMIN')")`).

6. **Въпрос:** *„Защо **Constructor Injection** с `final` полета е по-добър от `@Autowired` върху поле?“*
   * **Отговор:** Позволява полетата да са `final` (неизменяеми и потокобезопасни), гарантира, че обектът никога не може да бъде създаден в невалидно състояние без зависимостите си, и позволява лесно писане на чисти Unit тестове с `new OrderService(mockRepo)` без стартиране на целия Spring контейнер.

---

## 2. СКРИТИ КАПАНИ В JAVA & SPRING (`INTEGER` CACHE, `EQUALS`/`HASHCODE`, `@TRANSACTIONAL` SELF-INVOCATION)

1. **Капанът на `Integer` Cache (`-128` до `127`) при `==`:**
   * JVM кешира обектите `Integer` в диапазона от `-128` до `127`. Затова `Integer.valueOf(100) == Integer.valueOf(100)` връща `true`, но `Integer.valueOf(200) == Integer.valueOf(200)` връща `false` (сравнява референции в Heap!).
   * **Решение:** Винаги сравнявай Wrapper типове (`Long`, `Integer`, `BigDecimal`) с **`.equals()`** или **`Objects.equals(a, b)`**!
2. **Капанът на `equals()` и `hashCode()` при JPA `@Entity` в `HashSet`:**
   * Ако използваш генериран `@GeneratedValue` ID за `equals()`/`hashCode()`, преди `save()` ID-то е `null`, а след `save()` става `42`. Хеш кодът на обекта се променя, докато е вътре в `HashSet`, и `set.contains(entity)` започва да връща `false`!
   * **Решение:** В JPA `@Entity` класовете базирай `equals()` и `hashCode()` на естествен уникален ключ (напр. `isbn` или `uuid`) или връщай константа `getClass().hashCode()` и проверявай `id != null && id.equals(other.id)`.
3. **Капанът на `@Transactional` Self-Invocation (Вътрешно извикване в същия клас):**
   * Ако метод `public void registerUser()` (без `@Transactional`) извика вътрешен метод `this.saveWithAudit()` (с `@Transactional`), транзакцията **НЕ се стартира**, защото извикването минава директно през `this`, заобикаляйки Spring AOP Proxy обвивката!
   * **Решение:** Изнасяй `@Transactional` методите в отделен `@Service` клас или анотирай външния входен метод.

---

## 3. DEBUGGING & PROFILING TECHNIQUES В JAVA & SPRING BOOT (INTELLIJ, SQL LOGS, `JSTACK`, HEAP DUMP)

*   **IntelliJ IDEA Advanced Debugging:**
    *   **Evaluate Expression (`Alt + F8`):** Спри на Breakpoint и изпълнявай произволен Java код или Stream заявки на живо върху текущите променливи в паметта.
    *   **Drop Frame:** Връща изпълнението една стъпка назад в Call Stack-а (преизпълнява метода отначало, ако си пропуснал важен ред).
    *   **Field Watchpoints (Символ „Око“ върху поле):** Спира програмата автоматично в точния момент, когато някой код промени стойността на конкретно поле на обект.
*   **Диагностика на бавни SQL заявки и N+1 в Spring Boot:**
    ```properties
    # Показва реалните стойности на ? параметрите и статистиката на Hibernate сесията (само за dev!):
    spring.jpa.properties.hibernate.generate_statistics=true
    logging.level.org.hibernate.SQL=DEBUG
    logging.level.org.hibernate.orm.jdbc.bind=TRACE
    ```
*   **Продукционна диагностика на работещ JVM процес (без рестарт):**
    *   **Thread Dump (при зависване / Deadlock):** Изпълни `jstack <PID> > threads.txt` (или `jcmd <PID> Thread.print`), за да видиш коя нишка чака на заключване (`BLOCKED`).
    *   **Heap Dump (при `OutOfMemoryError`):** Стартирай приложението с `-XX:+HeapDumpOnOutOfMemoryError -XX:HeapDumpPath=/logs/heap.hprof` и анализирай `.hprof` файла с **Eclipse MAT** или IntelliJ Profiler, за да видиш кой обект заема гигабайти RAM.

---
[⬅️ 04-98. Java & Spring Common Errors & Fixes](04-98-Common-Errors-And-Fixes.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 01-QA-Automation](../01-QA-Automation/README.md)
