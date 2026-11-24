[⬅️ 01-01. Ниво 1: Бързите победи](01-01-Level-1-The-Quick-Wins.md) | [🗺️ Пълен Атлас (00)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 03-01. Ниво 3: Промяна на мисленето](03-01-Level-3-Mindset-Shift.md)

---

# 02-01. Ниво 2: Естествени разширения (QA Automation, TypeScript, Python, Java & Spring Boot)

**Цел:** Бързо обобщение на четирите технологии от Ниво 2, подредени спрямо ООП и .NET мисловния модел — с практични примери, типични грешки и директни връзки към главните `README.md` хъбове на всеки подмодул.

## 🧭 Съдържание на темата
* [1. QA Automation (C# + Selenium WebDriver + NUnit + Jira)](#1-qa-automation-c--selenium-webdriver--nunit--jira)
* [2. TypeScript (Строго-типизиран уеб език)](#2-typescript-строго-типизиран-уеб-език)
* [3. Python 3.x (Скриптинг, OOP и Django / FastAPI основи)](#3-python-3x-скриптинг-oop-и-django--fastapi-основи)
* [4. Java 21+ & Spring Boot 3 (Корпоративен ООП бекенд)](#4-java-21--spring-boot-3-корпоративен-ооп-бекенд)
* [5. Всички папки и подмодули в Ниво 2](#5-всички-папки-и-подмодули-в-ниво-2)

---

## 1. QA AUTOMATION (C# + SELENIUM WEBDRIVER + NUNIT + JIRA)

*   **Архитектурна роля:** Писане на автоматизирани End-to-End тестове в C# с **Selenium WebDriver** и **NUnit**, използване на **Page Object Model (POM)**, устойчиви CSS/XPath селектори, Explicit Waits и професионално докладване на дефекти в Jira.
*   **Синтактичен пример (NUnit + Selenium с Explicit Wait):**
    ```csharp
    [Test]
    public void LoginButton_ShouldBeVisible_WhenPageLoads()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var loginBtn = wait.Until(d => d.FindElement(By.CssSelector("button[data-testid='login-btn']")));
        Assert.That(loginBtn.Displayed, Is.True);
    }
    ```
*   **Системен капан и решение:** `NoSuchElementException` или `StaleElementReferenceException` заради `Thread.Sleep()`. Винаги използвайте **`WebDriverWait`** с `ExpectedConditions` и стабилни `data-testid` атрибути.

👉 **За повече информация и пълни лекции:** Вижте главния навигационен индекс в [01-QA-Automation/README.md](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/README.md).

---

## 2. TYPESCRIPT (СТРОГО-ТИПИЗИРАН УЕБ ЕЗИК)

*   **Архитектурна роля:** Надмножество (Superset) на JavaScript, което добавя статична типова система, интерфейси (`interface`), класове, `Generics` и декоратори, откривайки грешките още по време на компилация (Compile-time) вместо пред клиента.
*   **Синтактичен пример (Типизиран интерфейс и Generic функция):**
    ```typescript
    interface ApiResponse<T> {
        status: number;
        data: T;
    }

    function wrapData<T>(payload: T): ApiResponse<T> {
        return { status: 200, data: payload };
    }
    ```
*   **Системен капан и решение:** Използване на типа `any`, който изключва компилатора на TypeScript. Винаги използвайте **`unknown`** с Type Guards или строги интерфейси.

👉 **За повече информация и пълни лекции:** Вижте главния навигационен индекс в [02-TypeScript-Language/README.md](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/README.md).

---

---

## 3. PYTHON 3.X (СКРИПТИНГ, OOP И DJANGO / FASTAPI ОСНОВИ)

*   **Архитектурна роля:** Високонивов, четим интерпретируем език с динамична (но силна) типизация и Type Hints (`def greet(name: str) -> str:`), използван за бекенд (Django, FastAPI), автоматизации, QA и Data Science.
*   **Синтактичен пример (Python 3 клас с Type Hints и List Comprehension):**
    ```python
    class UserService:
        def __init__(self, users: list[str]) -> None:
            self.users = users

        def get_active_upper(self) -> list[str]:
            return [u.upper() for u in self.users if u.strip()]
    ```
*   **Системен капан и решение:** Мутируем аргумент по подразбиране (`def add(item, items=[])`) или споделен списък на ниво клас. Винаги използвайте `items: list[str] | None = None` и инициализирайте `self.items = []` вътре в `__init__`.

👉 **За повече информация и пълни лекции:** Вижте главния навигационен индекс в [03-Python-Scripting/README.md](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/03-Python-Scripting/README.md).

---

## 4. JAVA 21+ & SPRING BOOT 3 (КОРПОРАТИВЕН ООП БЕКЕНД)

*   **Архитектурна роля:** Строго-типизиран, компилиран до байткод ООП език върху JVM (с Virtual Threads в Java 21+), чиято корпоративна екосистема (**Spring Boot 3**, **Hibernate / Spring Data JPA**, **Spring Security 6 + JWT**) съвпада архитектурно 1:1 с C# и ASP.NET Core.
*   **Синтактичен пример (Spring Boot 3 REST Controller с Java `record` DTO):**
    ```java
    public record UserDto(Long id, String email) {}

    @RestController
    @RequestMapping("/api/v1/users")
    public class UserController {
        @GetMapping("/{id}")
        public ResponseEntity<UserDto> getUser(@PathVariable Long id) {
            return ResponseEntity.ok(new UserDto(id, "user@example.com"));
        }
    }
    ```
*   **Системен капан и решение:** `NullPointerException`, `LazyInitializationException` извън `@Transactional` или N+1 заявки при `@OneToMany`. Винаги използвайте `Optional<T>`, `FetchType.LAZY` с `JOIN FETCH` и мапинг към неизменяеми `record` DTO обекти.

👉 **За повече информация и пълни лекции:** Вижте главния навигационен индекс (с всички 5 модула: Java Fundamentals, Java OOP, Spring Data, Spring Fundamentals и Spring Advanced) в [04-Java-Ecosystem/README.md](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/README.md).

---

## 5. ВСИЧКИ ПАПКИ И ПОДМОДУЛИ В НИВО 2

Директна навигация към всички папки, модули и файлове в **`02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/`**:

* 📂 **[Главен индекс на Ниво 2 (README.md)](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/README.md)**
* **01. QA Automation (C# + Selenium WebDriver + NUnit)**
  * 📂 **[Индекс на 01-QA-Automation](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/README.md)**
  * 📄 [01-00. QA Automation And Selenium Basics](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/01-00-QA-Automation-And-Selenium-Basics.md)
  * 📄 [01-01. DOM Selectors And XPath Strategies](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/01-01-DOM-Selectors-And-XPath-Strategies.md)
  * 📄 [01-02. Advanced QA Traps And Snippets](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/01-02-Advanced-QA-Traps-And-Snippets.md)
  * 📄 [01-03. QA Cheat Sheet And Reference Guide](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/01-QA-Automation/01-03-QA-Cheat-Sheet-And-Reference-Guide.md)
* **02. TypeScript Language**
  * 📂 **[Индекс на 02-TypeScript-Language](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/README.md)**
  * 📄 [02-00. TypeScript vs JavaScript](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-00-TypeScript-vs-JavaScript.md)
  * 📄 [02-01. Introduction to TypeScript](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-01-Introduction-to-TypeScript.md)
  * 📄 [02-02. Advanced Data Types](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-02-Advanced-Data-Types.md)
  * 📄 [02-03. OOP in TypeScript](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-03-OOP-in-TypeScript.md)
  * 📄 [02-04. Mastering Generics](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-04-Mastering-Generics.md)
  * 📄 [02-05. TypeScript Decorators](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-05-TypeScript-Decorators.md)
  * 📄 [02-06. TypeScript Traps And Snippets](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-06-TypeScript-Traps-And-Snippets.md)
  * 📄 [02-07. TypeScript Cheat Sheet](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-07-TypeScript-Cheat-Sheet.md)
  * 📄 [02-08. TypeScript Deep Dive Addendum](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/02-TypeScript-Language/02-08-TypeScript-Deep-Dive-Addendum.md)
* **03. Python Scripting & OOP**
  * 📂 **[Индекс на 03-Python-Scripting](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/03-Python-Scripting/README.md)**
  * 📂 [Module 01: Python Basics](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/03-Python-Scripting/Module-01-Basics/README.md)
  * 📂 [Module 02: Python Advanced](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/03-Python-Scripting/Module-02-Advanced/README.md)
  * 📂 [Module 03: Python OOP](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/03-Python-Scripting/Module-03-OOP/README.md)
* **04. Java Ecosystem (Java 21+ & Spring Boot 3)**
  * 📂 **[Индекс на 04-Java-Ecosystem](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/README.md)**
  * 📂 [Module 01: Java Fundamentals](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/Module-01-Java-Fundamentals/README.md)
  * 📂 [Module 02: Java OOP](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/Module-02-Java-OOP/README.md)
  * 📂 [Module 03: Spring Data (Hibernate & JPA)](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/Module-03-Spring-Data/README.md)
  * 📂 [Module 04: Spring Fundamentals (Spring Boot & MVC)](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/Module-04-Spring-Fundamentals/README.md)
  * 📂 [Module 05: Spring Advanced (REST APIs, Security, JWT & Docker)](./02-Level-2-Natural-Extensions-QA-Automation-TypeScript-Python-Java/04-Java-Ecosystem/Module-05-Spring-Advanced/README.md)

---
[⬅️ 01-01. Ниво 1: Бързите победи](01-01-Level-1-The-Quick-Wins.md) | [🗺️ Пълен Атлас (00)](00-Full-Stack-And-QA-Automation-Roadmap.md) | [🏠 Начало (01-Theory-Notes)](../README.md) | [➡️ 03-01. Ниво 3: Промяна на мисленето](03-01-Level-3-Mindset-Shift.md)
