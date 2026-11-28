[⬅️ 01-04-00. Selenium WebDriver, DOM Locators & POM](01-04-00-Selenium-WebDriver-And-POM.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-05-00. Playwright, AJAX/jQuery & Mobile Appium](01-05-00-Playwright-AJAX-And-Appium.md)

---

# 01-04-01. Selenium & Locators Cheat Sheet (Синтаксис от Нулата)

**Раздел 03:** Web UI & Mobile Automation | **Цел:** Практически шаблони от нулата за настройка на **Selenium WebDriver (C# и Java)** с `ChromeOptions` (Headless режим за CI/CD), работа с падащи менюта (`SelectElement`), прозорци/табове, `iframes`, JavaScript Alerts, правене на скрийншоти при паднал тест и пълна 3-колонна справочна таблица (**CSS Selectors & XPath Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Настройка на WebDriver от нулата (C# & Java с Headless за CI/CD)](#1-настройка-на-webdriver-от-нулата-c--java-с-headless-за-cicd)
* [2. Специални UI взаимодействия: Select, Alerts, Iframes, Windows & Screenshots](#2-специални-ui-взаимодействия-select-alerts-iframes-windows--screenshots)
* [3. CSS Selectors & XPath от нулата (Вход / Изход примери)](#3-css-selectors--xpath-от-нулата-вход--изход-примери)
* [4. Обобщение / Бърз Cheat Sheet (Locators & Selenium API)](#4-обобщение--бърз-cheat-sheet-locators--selenium-api)

---

## 1. НАСТРОЙКА НА WEBDRIVER ОТ НУЛАТА (C# & JAVA С HEADLESS ЗА CI/CD)

```csharp
// C# (.NET 8 + Selenium.WebDriver 4.x)
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var options = new ChromeOptions();
options.AddArgument("--window-size=1920,1080");
// Задължителни флагове за Linux Docker / GitHub Actions CI/CD среди:
if (Environment.GetEnvironmentVariable("CI") == "true")
{
    options.AddArgument("--headless=new");
    options.AddArgument("--no-sandbox");
    options.AddArgument("--disable-dev-shm-usage");
}

IWebDriver driver = new ChromeDriver(options);
```

---

## 2. СПЕЦИАЛНИ UI ВЗАИМОДЕЙСТВИЯ: SELECT, ALERTS, IFRAMES, WINDOWS & SCREENSHOTS

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

// 1. Падащо меню (<select>) чрез SelectElement (NuGet: Selenium.Support)
var countryDropdown = new SelectElement(driver.FindElement(By.Id("country")));
countryDropdown.SelectByValue("BG");
countryDropdown.SelectByText("Bulgaria");

// 2. Браузърни JavaScript Alerts (alert / confirm / prompt)
IAlert alert = driver.SwitchTo().Alert();
string alertText = alert.Text;
alert.Accept();  // Натиска OK (или alert.Dismiss() за Cancel)

// 3. Превключване към <iframe> и връщане обратно в главния документ
driver.SwitchTo().Frame("payment-iframe");
driver.FindElement(By.Id("card-number")).SendKeys("4111111111111111");
driver.SwitchTo().DefaultContent(); // ЗАДЪЛЖИТЕЛНО връщане обратно!

// 4. Превключване към нов таб / прозорец
string originalWindow = driver.CurrentWindowHandle;
driver.FindElement(By.CssSelector("a[target='_blank']")).Click();
foreach (string handle in driver.WindowHandles)
{
    if (handle != originalWindow)
    {
        driver.SwitchTo().Window(handle);
        break;
    }
}

// 5. Сложни миши действия (Hover, Drag & Drop) чрез Actions API
var actions = new Actions(driver);
actions.MoveToElement(driver.FindElement(By.Id("profile-menu"))).Perform();

// 6. Заснемане на Screenshot при паднал тест в [TearDown]
if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
{
    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
    screenshot.SaveAsFile(\$"fail_{TestContext.CurrentContext.Test.Name}.png");
}
```

---

## 3. CSS SELECTORS & XPATH ОТ НУЛАТА (ВХОД / ИЗХОД ПРИМЕРИ)

### 🔹 Пример А: Намиране на бутон „Добави в количката“ за конкретен продукт в таблица/карта
* **Вход (HTML структура):**
  ```html
  <div class="product-card">
      <h3>Лаптоп ThinkPad X1</h3>
      <span class="price">2400 EUR</span>
      <button class="btn-buy">Купи</button>
  </div>
  ```
* **Изход (Устойчив XPath по съседство / родител):**
  ```xpath
  //div[@class='product-card'][.//h3[text()='Лаптоп ThinkPad X1']]//button[contains(@class, 'btn-buy')]
  ```

---

## 4. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET (LOCATORS & SELENIUM API)

| Търсене / Действие | CSS Selector Синтаксис | XPath Синтаксис |
| :--- | :--- | :--- |
| **По ID или Клас** | `#submit-btn` / `.btn-primary` | `//*[@id='submit-btn']` / `//*[contains(@class,'btn-primary')]` |
| **По произволен атрибут** | `input[data-testid='email']` | `//input[@data-testid='email']` |
| **Започва с / Завършва с / Съдържа** | `[id^='user_']` / `[id$='_box']` / `[href*='login']` | `//*[starts-with(@id,'user_')]` / `//*[contains(@href,'login')]` |
| **Директно дете / произволен наследник** | `ul.nav > li` / `div.card p` | `//ul[@class='nav']/li` / `//div[@class='card']//p` |
| **По точен или частичен текст** | *(Не се поддържа в чист CSS)* | `//button[normalize-space()='Изпрати']`<br>`//span[contains(text(),'Успешно')]` |
| **Качване към родител (Upward)** | *(Не се поддържа в CSS)* | `//td[text()='Ivan']/parent::tr`<br>`//input[@id='agree']/ancestor::form` |
| **Пореден елемент (Индекс)** | `ul > li:first-child` / `li:nth-child(2)` | `(//ul/li)[1]` / `(//ul/li)[2]` *(XPath брои от 1!)* |

---

[⬅️ 01-04-00. Selenium WebDriver, DOM Locators & POM](01-04-00-Selenium-WebDriver-And-POM.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-05-00. Playwright, AJAX/jQuery & Mobile Appium](01-05-00-Playwright-AJAX-And-Appium.md)

© 2026 Dev-Library / 01-04-01. Selenium & Locators Cheat Sheet (Синтаксис от Нулата)