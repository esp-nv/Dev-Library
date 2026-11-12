[⬅️ 01-02. Инженерен Справочник](01-02-Advanced-QA-Traps-And-Snippets.md) | 📂 Папка: 1-QA-Automation/Cheat-Sheet | [🏠 Начало](../../../README.md)

---

# 10-03. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 10:** QA Automation | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за селектори, XPath направления, синхронизационни състояния и готов за внедряване автоматизиран C# софтуерен тестов шаблон.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Справочник на XPath осите и селекторите](#1-global-reference-guide-справочник-на-xpath-осите-и-селекторите)
* [2. QA AUTOMATION CHEAT SHEET (Бърз синтактичен пищов)](#2-qa-automation-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI & AUTOMATION SNIPPET: "Професионален C# NUnit + Selenium Тест"](#3-готов-ui--automation-snippet-професионален-c-nunit--selenium-тест)

---

## 1. GLOBAL REFERENCE GUIDE: Справочник на XPath осите и селекторите

### 🗛 А. Синтактични XPath Референции за Текст и Атрибути:
*   `//button[text()='Влез']` ➔ Открива бутон с абсолютно точно съвпадение на низа.
*   `//input[contains(@placeholder, 'имейл')]` ➔ Търси частично съвпадение навсякъде в атрибута.
*   `//input[@required]` ➔ Локализира елементи, притежаващи декларативния HTML5 валидационен маркер.

### 🗛 Б. Навигационни направления (XPath Axes):
*   `/parent::*` ➔ Навигира една стъпка нагоре по DOM дървото до директния родителски контейнер.
*   `/following-sibling::*` ➔ Маркира следващия съседен елемент на същото йерархично ниво.
*   `/preceding-sibling::*` ➔ Връща предходния съседен брат в текущата кутия.

---

## 2. QA AUTOMATION CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `driver.Navigate().GoToUrl("URL");` ➔ Команда за начално навигиране и извикване на мрежовия поток.
*   `ExpectedConditions.ElementToBeClickable(By)` ➔ **Най-важното Експлицитно изчакване** против Flaky сривове при асинхронни бутони.
*   `driver.Quit();` ➔ **Задължителна софтуерна хигиена** — затваря всички прозорци, убива процесите на драйвера и освобождава RAM паметта на сървъра.
*   `Assert.That(actual, Is.EqualTo(expected));` ➔ Нативен NUnit одит за валидиране на крайния бизнес резултат.

---

## 3. ГОТОВ UI & AUTOMATION SNIPPET: "ПРОФЕСИОНАЛЕН C# NUNIT + SELENIUM ТЕСТ"

Копирай този готов, напълно валидиран и застрахован против асинхронни дупки автоматизиран тестов клас. Той имплементира правилния Жизнен цикъл (`[SetUp]`, `[Test]`, `[TearDown]`), интелигентен `WebDriverWait` и чиста защита против уеб изтичания на ресурси:

```csharp
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace DevLibrary.Automation.Tests
{
    [TestFixture]
    public class SoftUniFormAutomationTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private readonly string _targetUrl = "http://127.0.0"; // Локален Live Server порт

        [SetUp]
        public void AssembleWebDriverEnvironment()
        {
            // Стартиране на сигурна Chrome сесия с headless аргументи за CI/CD конвейера
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless=new"); // Спира графичния интерфейс на сървъра за разработка
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");

            _driver = new ChromeDriver(chromeOptions);
            
            // Заковаване на Експлицитно изчакване с 10 секунди хардуерен таван
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Navigate().GoToUrl(_targetUrl);
        }

        [Test]
        public async Task Execute_SecuredAuthLogin_ShouldSucceedWithValidModel()
        {
            // Step 1: Асинхронно изчакване на елементите да излязат на екрана (Борба против Flaky поведение)
            IWebElement emailInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("InputEmail")));
            IWebElement passwordInput = _driver.FindElement(By.Id("InputPassword"));
            IWebElement submitButton = _driver.FindElement(By.XPath("//button[@type='submit']"));

            // Step 2: Роботът въвежда данни и почиства буферите в паметта
            emailInput.Clear();
            emailInput.SendKeys("student@softuni.bg");

            passwordInput.Clear();
            passwordInput.SendKeys("SecretPassword123");

            // Step 3: Изчакване на бутона да бъде кликаем и задействане на Submit жизнения цикъл
            _wait.Until(ExpectedConditions.ElementToBeClickable(submitButton));
            submitButton.Click();

            // Step 4: Валидация на крайния резултат (Одит на URL дестинацията)
            _wait.Until(ExpectedConditions.UrlContains("/dashboard"));
            string actualUrl = _driver.Url;

            Assert.That(actualUrl, Does.Contain("/dashboard"), "🚨 КРАХ: Формата не пренасочи към защитеното софтуерно табло!");
        }

        [TearDown]
        public void DemolishWebDriverEnvironment()
        {
            if (_driver != null)
            {
                // Изрично прекратяване на нишките и изчистване на RAM паметта на машината
                _driver.Quit();
                _driver.Dispose();
            }
        }
    }
}
```

---
[⬅️ 01-02. Инженерен Справочник](01-02-Advanced-QA-Traps-And-Snippets.md) | [📂 Папка: 01-QA-Automation](README.md) | [🏠 Начало](../../../README.md)
