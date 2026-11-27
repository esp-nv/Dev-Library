[⬅️ 04-97. Java & Spring Category Cheat Sheet](04-97-Category-Cheat-Sheet.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-99. Interview QA, Traps & Debugging](04-99-Interview-QA-Traps-And-Debugging.md)

---

# 04-98. Java 21+ & Spring Boot 3 Common Errors, Edge Cases & Fixes (Каталог на грешките)

**Цел:** Практически наръчник за бързо диагностициране и фиксиране на най-честите грешки в **Java 21+** и **Spring Boot 3 / Hibernate** (`NullPointerException`, `LazyInitializationException`, `ConcurrentModificationException`, `UnsatisfiedDependencyException`), гранични случаи (**Edge Cases** при колекции, цели числа, `BigDecimal` и `Optional`) и йерархията на `Throwable` (`Error` срещу `Checked` и `Unchecked` Exceptions).

## 🧭 Съдържание на темата
* [1. Фундаментални Java изключения (`NullPointerException` / `ConcurrentModificationException` / `IndexOutOfBoundsException`)](#1-фундаментални-java-изключения-nullpointerexception--concurrentmodificationexception--indexoutofboundsexception)
* [2. Spring Boot & Hibernate грешки (`LazyInitializationException` / `UnsatisfiedDependencyException`)](#2-spring-boot--hibernate-грешки-lazyinitializationexception--unsatisfieddependencyexception)
* [3. Гранични случаи (Edge Cases): Integer Overflow, `BigDecimal`, празни колекции и `List.of()`](#3-гранични-случаи-edge-cases-integer-overflow-bigdecimal-празни-колекции-и-listof)
* [4. Йерархия на `Throwable` (`Error` vs `Checked` vs `Unchecked`) & Exception Chaining](#4-йерархия-на-throwable-error-vs-checked-vs-unchecked--exception-chaining)
* [5. Подробна справка: Кога възниква всяка грешка и в коя фаза?](#5-подробна-справка-кога-възниква-всяка-грешка-и-в-коя-фаза)

---

## 1. ФУНДАМЕНТАЛНИ JAVA ИЗКЛЮЧЕНИЯ (`NULLPOINTEREXCEPTION` / `CONCURRENTMODIFICATIONEXCEPTION`)

### 1.1 `NullPointerException: Cannot invoke "String.toLowerCase()" because "email" is null`
*   **Причина:** Опит за извикване на метод или достъп до поле върху референция със стойност `null` (от Java 14+ JVM показва точно коя променлива е `null`!).
*   **Грешно срещу Правилно:**
```java
// ❌ ГРЕШНО: Директен достъп или викане на optional.get() без проверка
public String normalizeEmailBad(Optional<String> emailOpt) {
    return emailOpt.get().trim().toLowerCase(); // Хвърля NoSuchElementException или NPE!
}

// ✅ ПРАВИЛНО: Функционална верига с Optional или Objects.requireNonNullElse
public String normalizeEmailGood(Optional<String> emailOpt) {
    return emailOpt
            .map(String::trim)
            .map(String::toLowerCase)
            .orElse("guest@example.com");
}
```

### 1.2 `ConcurrentModificationException`
*   **Причина:** Изтриване или добавяне на елементи в `ArrayList` или `HashMap` директно вътре в `for-each` цикъл.
*   **Грешно срещу Правилно:**
```java
List<String> users = new ArrayList<>(List.of("admin", "guest", "banned_user"));

// ❌ ГРЕШНО: Хвърля ConcurrentModificationException по време на итерацията
// for (String u : users) {
//     if (u.startsWith("banned")) users.remove(u);
// }

// ✅ ПРАВИЛНО: Използвайте removeIf() (Java 8+) или Iterator.remove()
users.removeIf(u -> u.startsWith("banned"));
```

---

## 2. SPRING BOOT & HIBERNATE ГРЕШКИ (`LAZYINITIALIZATIONEXCEPTION` / `UNSATISFIEDDEPENDENCYEXCEPTION`)

### 2.1 `org.hibernate.LazyInitializationException: could not initialize proxy - no Session`
*   **Причина:** Опит за достъп до `FetchType.LAZY` колекция (напр. `author.getBooks()`) в Controller-а или Thymeleaf шаблона, след като `@Transactional` методът в Service слоя вече е приключил и Hibernate сесията е затворена.
*   **Решение:**
    1. Напиши JPQL заявка с **`JOIN FETCH`** в Repository слоя, за да заредиш релацията още в транзакцията.
    2. Преобразувай Entity-то в **`record` DTO** вътре в `@Transactional(readOnly = true)` метода на Service слоя и никога не връщай `@Entity` директно от REST контролера.

### 2.2 `UnsatisfiedDependencyException: Error creating bean with name ...`
*   **Причина:** Липсваща анотация (`@Service`, `@Repository`, `@Component`) върху имплементиращия клас, два бийна от един и същ интерфейс без `@Primary`/`@Qualifier`, или кръгова зависимост (`ServiceA` инжектира `ServiceB`, а `ServiceB` инжектира `ServiceA`).
*   **Решение:** Провери анотациите на класовете и прекъсни кръговата зависимост чрез изнасяне на общата логика в трети сървис или Spring Events (`ApplicationEventPublisher`).

---

## 3. ГРАНИЧНИ СЛУЧАИ (EDGE CASES): INTEGER OVERFLOW, `BIGDECIMAL`, ПРАЗНИ КОЛЕКЦИИ И `LIST.OF()`

| Граничен случай (Edge Case) | Какво се чупи в Runtime | Как се фиксира правилно |
| :--- | :--- | :--- |
| **Препълване на `int` (`2_000_000_000 + 2_000_000_000`)** | В Java `int` превърта тихо (*Silent Overflow*) и връща отрицателното число **`-294967296`**! | Използвайте `long` за суми и броячи или `Math.addExact(a, b)`, което хвърля `ArithmeticException` при препълване. |
| **Финансови изчисления с `double` (`0.1 + 0.2`)** | Връща `0.30000000000000004` и чупи счетоводните проверки. | Винаги ползвайте **`BigDecimal`** със String конструктор (`new BigDecimal("0.1")`) и `.compareTo()` вместо `.equals()`. |
| **Модифициране на `List.of(...)` или `Arrays.asList(...)`** | `List.of(1, 2).add(3)` хвърля `UnsupportedOperationException`! | Когато ти трябва променяем списък, обвивай в `new ArrayList<>(List.of(1, 2))`. |
| **Празен масив или колекция (`list.get(0)`, `arr`)** | Хвърля `IndexOutOfBoundsException` или `ArrayIndexOutOfBoundsException`. | Проверявайте `if (list == null \|\| list.isEmpty())` преди достъп по индекс. |
| **Сравняване на низове с `==` вместо `.equals()`** | `new String("abc") == new String("abc")` връща `false`, защото сравнява адреси в Heap! | Винаги сравнявайте низове и обекти с `Objects.equals(a, b)` или `"CONST".equals(var)`. |

---

## 4. ЙЕРАРХИЯ НА `THROWABLE` (`ERROR` VS `CHECKED` VS `UNCHECKED`) & EXCEPTION CHAINING

В Java всички грешки и изключения наследяват базовия клас **`java.lang.Throwable`**, който се дели на три строго разграничени клона:

### 1. Трите клона на `Throwable`
*   **`Error` (Критични системни сривове на JVM):** `OutOfMemoryError`, `StackOverflowError`, `NoClassDefFoundError`. Приложението **не трябва** да ги хваща с `try-catch`, защото JVM е в невъзстановимо състояние.
*   **`Checked Exceptions` (Наследяват `Exception`, но НЕ `RuntimeException`):** `IOException`, `SQLException`, `ParseException`. Компилаторът на Java **задължава** разработчика или да ги хване в `try-catch`, или да ги декларира в сигнатурата с `throws IOException`.
*   **`Unchecked Exceptions` (Наследяват `RuntimeException`):** `NullPointerException`, `IllegalArgumentException`, `IndexOutOfBoundsException`, `LazyInitializationException`. Не се проверяват при компилация и сочат към програмни бъгове или нарушени бизнес правила (в Spring Boot 3 всички домейн изключения наследяват `RuntimeException`, защото `@Transactional` автоматично прави `rollback` само при `RuntimeException` и `Error`!).

### 2. Custom Business Exception & Запазване на веригата (`cause` = `InnerException`)
```java
public class PaymentGatewayException extends RuntimeException {
    public PaymentGatewayException(String message, Throwable cause) {
        super(message, cause); // Запазва оригиналния Stack Trace в getCause()!
    }
}

public void chargeCard(String orderId) {
    try {
        // Симулация на външен I/O проблем
        throw new java.io.IOException("Gateway timeout after 5000ms");
    } catch (java.io.IOException ex) {
        // ✅ ПРАВИЛНО: Преопаковаме Checked IOException в Unchecked бизнес изключение с cause!
        throw new PaymentGatewayException("Неуспешно плащане за поръчка " + orderId, ex);
    }
}
```
*   **Правило за производителност:** Конструкторът на `Throwable` вика нативния метод `fillInStackTrace()`, който обхожда целия стек от нишки на JVM — това е скъпа операция. Никога не използвайте изключения за нормална логика на управление (напр. при търсене на потребител връщайте `Optional<User>`).

---

## 5. ПОДРОБНА СПРАВКА: КОГА ВЪЗНИКВА ВСЯКА ГРЕШКА И В КОЯ ФАЗА?

| Клас / Тип грешка | Фаза на възникване | При какво възниква (Точна причина) | Пример и как се предотвратява |
| :--- | :--- | :--- | :--- |
| **`Syntax / Type / Checked Exception` грешка (`javac`)** | **Компилационна** (`Compile-time` преди `.class`) | Несъвместими типове, липсваща `;` или необработено `Checked Exception` (`IOException`). | `int x = "hello";` ➔ Хваща се веднага от IDE-то и `javac` преди старт. |
| **`NoClassDefFoundError` / `ClassNotFoundException`** | **При стартиране / Зареждане** (`Classloading-time`) | Липсваща `.jar` библиотека в Classpath-а или конфликт на версии в Maven/Gradle. | Проверете `pom.xml` зависимостите с `mvn dependency:tree`. |
| **`UnsatisfiedDependencyException` / `NoSuchBeanDefinitionException`** | **При стартиране на Spring** (`Context Startup-time`) | Липсващ `@Service`/`@Repository`, дублирани бийнове без `@Primary` или кръгова зависимост. | Добавете липсващата анотация или изнесете общата логика в отделен сървис. |
| **`NullPointerException` (`NPE`)** | **При изпълнение** (`Runtime` в JVM) | Извикване на метод или четене на поле върху `null` референция. | Използвайте `Optional<T>`, `Objects.requireNonNull()` и `@NotNull` валидации. |
| **`IndexOutOfBoundsException`** | **При изпълнение** (`Runtime` в JVM) | Достъп до невалиден индекс в `List`, масив или `String` (`idx < 0` или `idx >= size`). | Проверявайте границите `0 <= idx && idx < list.size()` преди достъп. |
| **`ConcurrentModificationException`** | **При изпълнение** (`Runtime` в JVM) | Промяна на колекция (`add`/`remove`) по време на `for-each` итерация. | Използвайте `list.removeIf(...)` или `Iterator.remove()`. |
| **`LazyInitializationException`** | **При изпълнение** (`Runtime` в Hibernate) | Достъп до `LAZY` релация извън активен `@Transactional` метод. | Заредете данните с `JOIN FETCH` и мапнете към `record` DTO в Service слоя. |
| **`DataIntegrityViolationException`** | **При изпълнение** (`Runtime` в Spring Data) | Нарушен SQL Constraint (`UNIQUE`, `NOT NULL`, `FOREIGN KEY`) при `save()` в базата. | Валидирайте уникалността предварително и хванете грешката в `@RestControllerAdvice` (`409 Conflict`). |
| **`OutOfMemoryError` / `StackOverflowError`** | **При изпълнение** (`Runtime` — критичен JVM срив) | Изчерпване на JVM Heap паметта (утечка / огромни заявки) или безкрайна рекурсия. | Ползвайте `Pageable` странициране, `-XX:MaxRAMPercentage=75.0` и базово дъно в рекурсията. |

---
[⬅️ 04-97. Java & Spring Category Cheat Sheet](04-97-Category-Cheat-Sheet.md) | 📂 Папка: 04-Java-Ecosystem | [🏠 Начало](../README.md) | [➡️ 04-99. Interview QA, Traps & Debugging](04-99-Interview-QA-Traps-And-Debugging.md)
