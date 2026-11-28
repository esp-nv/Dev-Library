[⬅️ 01-03-01. NUnit, JUnit 5, Vitest & pytest: Синтаксис...](01-03-01-Syntax-Multi-Language-Unit-Testing.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-04-01. Selenium & Locators Cheat Sheet](01-04-01-Syntax-Selenium-WebDriver-And-POM.md)

---

# 01-04-00. Selenium WebDriver, DOM Locators, Explicit Waits & POM

**Раздел 03:** Web UI & Mobile Automation | **Цел:** Овладяване на вътрешната архитектура на **Selenium WebDriver (W3C WebDriver Protocol)**, пирамидата на устойчивите DOM локатори (CSS Selectors срещу XPath), трите механизма за синхронизация (**Implicit, Explicit и Fluent Waits**), архитектурния шаблон **Page Object Model (POM)** и предотвратяване на `StaleElementReferenceException` и `NoSuchElementException`.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Locators, Waits & POM)](#1-как-да-се-запомни-по-лесно-мнемоника-locators-waits--pom)
* [2. Архитектура на Selenium WebDriver и W3C Протокол (Блок-схема)](#2-архитектура-на-selenium-webdriver-и-w3c-протокол-блок-схема)
* [3. Сравнителни матрици: Пирамида на Локаторите & Видове Изчаквания (Waits)](#3-сравнителни-матрици-пирамида-на-локаторите--видове-изчаквания-waits)
* [4. Плюсове и минуси на Selenium WebDriver & Page Object Model (POM)](#4-плюсове-и-минуси-на-selenium-webdriver--page-object-model-pom)
* [5. Кодов одит: Грешен (Thread.Sleep & Крехък XPath) срещу Правилен (POM + Explicit Wait) тест](#5-кодов-одит-грешен-threadsleep--крехък-xpath-срещу-правилен-pom--explicit-wait-тест)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: LOCATORS, WAITS & POM)

* **Пирамидата на локаторите (От най-бързия към най-крехкия):**
  1. **`Id` / `data-testid`:** Златният стандарт — уникален идентификатор директно в DOM хеш-таблицата.
  2. **`Name` / `CSS Selector`:** Изключително бързи, четими и нативно поддържани от рендериращия двигател на браузъра.
  3. **Относителен `XPath` (`//button[text()='Купи']`):** Използва се само когато трябва да търсим по **видим текст** или да се качим нагоре към родителски елемент (`..` / `ancestor::`).
  4. **Абсолютен `XPath` (`/html/body/div[2]/div[1]/button`):** **Абсолютно забранен!** Промяната на един `<div>` в дизайна чупи теста моментално.
* **Три вида изчаквания (Светофарът на синхронизацията):**
  * **`Thread.Sleep(5000)` (Hard Wait):** Червен светофар — спира нишката насилствено за 5 секунди, дори страницата да е заредила за `0.2s`.
  * **`Implicit Wait`:** Глобален таймаут за целия драйвър при търсене на елемент в DOM (по подразбиране `0` секунди).
  * **`Explicit Wait` (`WebDriverWait`):** Интелигентен радар — проверява на всеки `500ms` (Polling Interval) дали конкретно условие е изпълнено (напр. `ElementToBeClickable`) и продължава в същата милисекунда!
* **Page Object Model (POM — Разделение на отговорностите):**
  * **Page класовете** знаят *КАК* да намерят елементите и да кликнат върху тях (съдържат локатори и методи, **без `Assert`**!).
  * **Test класовете** знаят *КАКВО* да проверят (извикват методите на Page класа и съдържат **`Assert`**!).

---

## 2. АРХИТЕКТУРА НА SELENIUM WEBDRIVER И W3C ПРОТОКОЛ (БЛОК-СХЕМА)

От Selenium 4 насам комуникацията между тестовия код и браузъра минава директно през стандартизирания **W3C WebDriver HTTP/JSON протокол**, а **Selenium Manager** автоматично изтегля правилната версия на браузърния драйвър:

```text
 ┌──────────────────────────┐    W3C WebDriver (HTTP/JSON)   ┌──────────────────────────┐
 │  Тестов Код (C# / Java / │ ─────────────────────────────► │     Browser Driver       │
 │  Python / TypeScript)    │   POST /session/{id}/element   │ (chromedriver.exe /      │
 │  • Page Objects (POM)    │ ◄───────────────────────────── │  geckodriver / edgedriver)│
 │  • NUnit / JUnit / pytest│   200 OK { "element-6066":.. } └────────────┬─────────────┘
 └──────────────────────────┘                                             │ Native CDP /
                                                                          ▼ IPC Команди
                                                             ┌──────────────────────────┐
                                                             │     Реален Браузър       │
                                                             │ (Chrome / Firefox / Edge)│
                                                             └──────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: ПИРАМИДА НА ЛОКАТОРИТЕ & ВИДОВЕ ИЗЧАКВАНИЯ (WAITS)

### 📊 А. Сравнение на CSS Selectors срещу XPath

| Критерий | CSS Selector | XPath |
| :--- | :--- | :--- |
| **Скорост на изпълнение** | 🚀 **По-бърз** (браузърът има вграден CSS парсер). | 🟡 **Малко по-бавен** (изисква XPath двигател). |
| **Посока на търсене** | Само **надолу** по DOM дървото (от родител към дете). | **Двупосочна** — надолу и **нагоре** (`parent::`, `ancestor::`). |
| **Търсене по видим текст** | ❌ Не поддържа директно търсене по текст. | ✅ Нативна функция `//button[normalize-space()='Вход']`. |
| **Синтаксис и четимост** | Кратък и чист (`form#login > input[name='email']`). | По-дълъг (`//form[@id='login']/input[@name='email']`). |

### 📊 Б. Сравнение на механизмите за изчакване (Waits)

| Механизъм | Обхват | Какво проверява? | Инженерна препоръка |
| :--- | :--- | :--- | :--- |
| **`Thread.Sleep(ms)`** | Еднократно | Нищо — замразява изпълнението за фиксирано време. | ❌ **Никога** не се използва в реални тестове. |
| **`Implicit Wait`** | Глобален за сесията | Само дали елементът съществува в DOM (дори да е скрит!). | ⚠️ Не се смесва с Explicit Wait! |
| **`Explicit Wait` (`WebDriverWait`)** | Конкретен елемент | Видимост, кликаемост, наличие на текст, изчезване на спинър. | ✅ **Златен стандарт** за стабилни E2E тестове. |
| **`Fluent Wait`** | Конкретен елемент | Като Explicit Wait + персонализиран Polling (напр. `100ms`) и игнориране на изключения. | ✅ При тежки динамични SPA таблици. |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА SELENIUM WEBDRIVER & PAGE OBJECT MODEL (POM)

### 🟢 Предимства (Pros):
* **Истинска мултиезична и крос-браузър поддръжка:** Един и същ W3C стандарт работи на C#, Java, Python, JS/TS, Ruby върху Chrome, Firefox, Edge и Safari.
* **Лесна поддръжка чрез POM:** Ако дизайнерът смени `id` на бутона за вход, променяш селектора на **едно-единствено място** в `LoginPage.cs`, вместо в 80 отделни теста.

### 🔴 Недостатъци (Cons):
* **Липса на вграден Auto-Waiting:** За разлика от Playwright, Selenium не чака автоматично елементът да спре да се анимира — инженерът сам управлява `WebDriverWait`.
* **По-бавно изпълнение от API/Unit тестове:** Всеки тест стартира реален браузърен процес и рендерира графика.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (THREAD.SLEEP & КРЕХЪК XPATH) СРЕЩУ ПРАВИЛЕН (POM + EXPLICIT WAIT) ТЕСТ

### ❌ ГРЕШЕН КОД (Hardcoded Sleep, Абсолютен XPath и Assertions в Page класа):
```csharp
[Test]
public void BadLoginTest()
{
    driver.Navigate().GoToUrl("https://app.local");
    // 🚨 ГРЕШКА 1: Абсолютен XPath -> ще се счупи при първата промяна в HTML!
    driver.FindElement(By.XPath("/html/body/div[1]/form/div[1]/input")).SendKeys("admin");
    driver.FindElement(By.XPath("/html/body/div[1]/form/div[2]/input")).SendKeys("1234");
    driver.FindElement(By.XPath("/html/body/div[1]/form/button")).Click();

    // 🚨 ГРЕШКА 2: Thread.Sleep забавя теста изкуствено или пада при бавен сървър!
    Thread.Sleep(3000);
    Assert.That(driver.Url.Contains("dashboard"), Is.True);
}
```

### 🟢 ПРАВИЛЕН КОД (Чист POM + Устойчив CSS/Data-TestId локатор + Explicit Wait):
```csharp
// 1. Page Object Клас (LoginPage.cs — без Assert!)
public class LoginPage(IWebDriver driver)
{
    private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(10));

    private By EmailInput => By.CssSelector("[data-testid='login-email']");
    private By PasswordInput => By.CssSelector("[data-testid='login-password']");
    private By SubmitButton => By.CssSelector("[data-testid='login-submit']");

    public void LoginAs(string email, string password)
    {
        _wait.Until(d => d.FindElement(EmailInput)).SendKeys(email);
        driver.FindElement(PasswordInput).SendKeys(password);
        _wait.Until(d => {
            var btn = d.FindElement(SubmitButton);
            return (btn.Displayed && btn.Enabled) ? btn : null;
        })!.Click();
    }
}

// 2. Test Клас (LoginTests.cs — четим бизнес сценарий с Assert!)
[Test]
public void Login_ValidCredentials_RedirectsToDashboard()
{
    var loginPage = new LoginPage(_driver);
    _driver.Navigate().GoToUrl("https://app.local");

    loginPage.LoginAs("elena@devlib.bg", "Pass1234!");

    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    bool redirected = wait.Until(d => d.Url.Contains("/dashboard"));
    Assert.That(redirected, Is.True);
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Какво представлява **`StaleElementReferenceException`** и как се решава?“*
  * **Отговор:** Това изключение възниква, когато си намерил елемент с `FindElement(...)` и си го запазил в променлива, но след това страницата се е презаредила или React/Vue/AJAX е прерисувал тази част от DOM дървото. Референцията в паметта сочи към стария, вече изтрит DOM възел. **Решение:** Не кеширай `IWebElement` в полета на класа — използвай `By` локатори (или презареди елемента вътре в `WebDriverWait`).
* **Въпрос 2:** *„Защо категорично се забранява смесването на **Implicit Wait** и **Explicit Wait** в една и съща WebDriver сесия?“*
  * **Отговор:** Официалната документация на Selenium предупреждава, че комбинирането им води до непредсказуемо натрупване на времената за изчакване (например при проверка за *изчезване* на елемент с Explicit Wait от 5s и Implicit Wait от 10s, тестът може да блокира за 15–20 секунди заради вътрешните повторни опити на драйвъра). Използвай **само Explicit Wait** (`ImplicitWait = TimeSpan.Zero`).







---

[⬅️ 01-03-01. NUnit, JUnit 5, Vitest & pytest: Синтаксис...](01-03-01-Syntax-Multi-Language-Unit-Testing.md) | [📂 Назад към QA Automation & SDET Engineering Хъба](README.md) | [🌐 Домейн: 05. QA и Автоматизирано Тестване](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: QA Automation & SDET Engineering](README.md) | [🌐 Следващ домейн: ️ 06. Облачна Инфраструктура, DevOps и Системна Архи...](../../06-Cloud-DevOps-And-Architecture/README.md) | [➡️ 01-04-01. Selenium & Locators Cheat Sheet](01-04-01-Syntax-Selenium-WebDriver-And-POM.md)

© 2026 Dev-Library / 01-04-00. Selenium WebDriver, DOM Locators, Explicit Waits & POM
