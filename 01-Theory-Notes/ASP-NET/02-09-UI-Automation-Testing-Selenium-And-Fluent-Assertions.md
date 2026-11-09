[⬅️ 02-08. Мрежов достъп през CORS](02-08-Cross-Origin-Resource-Sharing-CORS-Infrastructure.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-10. Оптимизация през MessagePack](02-10-MessagePack-Binary-Serialization-In-SignalR.md)

---

# 02-09. Автоматизирано тестване: Selenium WebDriver и MyTested.AspNetCore.Mvc

**Цел:** Изучаване на методите за автоматизиран качествен контрол (QA Automation), симулиране на реално потребителско поведение през виртуални браузърни двигатели със Selenium и писане на флуентни уеб асерции.

## 🧭 Съдържание на темата
* [1. Интеграционно тестване: Защо ни е нужен Selenium WebDriver?](#1-интеграционно-тестване-защо-ни-е-нужен-selenium-webdriver)
* [2. Ръководство: Писане на първия Selenium тест в C#](#2-ръководство-писане-на-първия-selenium-тест-в-c)
* [3. Флуентни изпитни асерции през MyTested.AspNetCore.Mvc](#3-флуентни-изпитни-асерции-през-mytestedaspnetcoremvc)
* [4. Системни капани: Сривове при липсващ браузърен драйвер](#4-системни-капани-сривове-при-липсващ-браузърен-драйвер)

---

## 1. ИНТЕГРАЦИОННО ТЕСТВАНЕ: ЗАЩО НИ Е НУЖЕН SELENIUM WEBDRIVER?

Докато класическите Unit Тестове извличат и тестват само един C# метод в пълна изолация (използвайки фалшиви Mocks), **Интеграционните уеб тестове (End-to-End / E2E)** изпробват цялата работеща софтуерна система в реални бойни условия.

**Selenium WebDriver** е индустриалният стандарт за автоматизация на браузъри. Той работи като софтуерен робот — отваря истински браузър (Chrome, Firefox), намира HTML бутоните по техните идентификатори (`Id`, `XPath`), кликва върху тях, попълва текстови форми, улавя статус кодовете на мрежата и проверява дали финалният изход на екрана съвпада с очаквания бизнес резултат.

---

## 2. РЪКОВОДСТВО: ПИСАНЕ НА ПЪРВИЯ SELENIUM ТЕСТ В C#

За целта се създава отделен тестов проект, използващ **NUnit** или **xUnit** среди.

*   **NuGet пакети за инсталация:** `Selenium.WebDriver` и `Selenium.WebDriver.ChromeDriver`.

Искаме автоматично да тестваме дали формата ни за вход в профил връща верен текст:

### 📄 1. Софтуерен C# код на Автоматизирания тест (LoginUiTests.cs):
```csharp
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

[TestFixture]
public class LoginUiTests
{
    private IWebDriver _driver;

    [SetUp]
    public void SetupBrowser()
    {
        // Стартираме скрит, фонов или видим екземпляр на браузъра Chrome
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Напреднал режим: пуска браузъра невидимо в RAM паметта без графика

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // Спасително чакане при бавна мрежа
    }

    [Test]
    public void Test_ValidLogin_ShouldDisplayWelcomeMessage()
    {
        // 1. Роботът отваря нашия работещ локално ASP.NET Core уеб сайт
        _driver.Navigate().GoToUrl("https://localhost:7214/Account/Login");

        // 2. Намираме HTML елементите по техните ID-та и попълваме текстови данни
        IWebElement emailInput = _driver.FindElement(By.Id("EmailField"));
        emailInput.SendKeys("softuni_student@abv.bg");

        IWebElement passwordInput = _driver.FindElement(By.Id("PasswordField"));
        passwordInput.SendKeys("SecurePass123");

        // 3. Кликваме върху бутона за изпращане
        IWebElement loginButton = _driver.FindElement(By.Id("SubmitBtn"));
        loginButton.Click();

        // 4. Проверяваме дали финалният отпечатан текст на екрана съвпада с очаквания успех
        IWebElement welcomeHeading = _driver.FindElement(By.Id("WelcomeMessage"));
        string actualText = welcomeHeading.Text;

        Assert.That(actualText, Is.EqualTo("Добре дошли в системата!"));
    }

    [TearDown]
    public void CloseBrowser()
    {
        // Задължително затваряме процеса в Windows/Linux паметта, за да не изтече RAM ресурса
        _driver.Quit(); 
        _driver.Dispose();
    }
}
```

---

## 3. ФЛУЕНТНИ ИЗПИТНИ АСЕРЦИИ ПРЕЗ MYTESTED.ASPNETCORE.MVC

На изпитите за напреднали в SoftUni, за тестване на контролери, маршрути, валидации и сесии се използва мощната флуентна библиотека **`MyTested.AspNetCore.Mvc`**. Тя ни позволява да разписваме декларативни тестове без никакво софтуерно усложняване:

```csharp
[Test]
public void ProductsController_DetailsAction_ShouldReturnValidViewModel()
    => MyMvc
        .Controller<ProductsController>() // 1. Избираме контролера
        .Calling(c => c.Details(5))       // 2. Викаме екшън метода
        .ShouldReturn()
        .View(view => view                // 3. Асерция: Трябва да върне ViewResult с модел!
            .WithModelOfType<ProductDetailsViewModel>()
            .Passing(model => model.Id == 5));
```

---

## 4. СИСТЕМНИ КАПАНИ: СРИВОВЕ ПРИ ЛИПСВАЩ БРАУЗЪРЕН ДРАЙВЕР

### 🚨 Капан №1: Срив на съвместимостта на версиите (Driver Failure)
*   **Какво се случва:** Стартираш уеб теста, но той крашва веднага, без изобщо да отвори Chrome прозорец.
*   **Точен софтуерен краш (Exception Message) в конзолата на NUnit:**
    ```text
    WebDriverException: unknown error: cannot find Chrome binary (SessionNotCreatedException: session not created: This version of ChromeDriver only supports Chrome version 120)
    ```
*   **Защо се случва:** Твоят реален браузър Google Chrome на компютъра ти се е обновил автоматично до по-нова версия (напр. версия 124), но NuGet пакетът в проекта ти `Selenium.WebDriver.ChromeDriver` е останал стара версия (напр. 120). Понеже версиите им се разминават, софтуерният мост се счупва.
*   **Как да го поправиш:** Обнови NuGet пакета `Selenium.WebDriver.ChromeDriver` до последната актуална версия през пакения мениджър, за да се изравнят дигиталните им подписи.

---
[⬅️ 02-08. Мрежов достъп през CORS](02-08-Cross-Origin-Resource-Sharing-CORS-Infrastructure.md) |  [📂 02-00. ASP.NET Core Advanced (Кратък справочник)](02-00-ASP-NET-CORE-ADVANCED-INTRO.md)  | [🏠 Начало (ASP.NET)](README.md) | [➡️ 02-10. Оптимизация през MessagePack](02-10-MessagePack-Binary-Serialization-In-SignalR.md)

© 2026 Dev-Library / QA Автоматизация: Selenium и уеб среди
