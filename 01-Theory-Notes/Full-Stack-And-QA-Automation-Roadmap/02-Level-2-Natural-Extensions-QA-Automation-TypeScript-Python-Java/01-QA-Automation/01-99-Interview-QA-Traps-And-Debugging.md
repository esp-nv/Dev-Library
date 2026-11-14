[⬅️ 01-98. QA Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 02-TypeScript-Language](../02-TypeScript-Language/README.md)

---

# 01-99. QA & SDET Interview Q&A, Скрити Капани & Debugging Techniques

**Цел:** Подготовка за технически интервюта за **Manual QA, QA Automation и SDET (Software Development Engineer in Test)** позиции, критични подводни камъни при **Flaky тестове**, API контракти и Performance тестове, както и професионални техники за дебъгване с **Playwright Trace Viewer**, **Chrome DevTools (HAR)** и **Apache JMeter**.

## 🧭 Съдържание на темата
* [1. Интервю въпросник (Top 6 QA & SDET въпроса с точни Senior отговори)](#1-интервю-въпросник-top-6-qa--sdet-въпроса-с-точни-senior-отговори)
* [2. Скрити капани в QA Automation (Flaky тестове, `Thread.Sleep`, Споделено състояние, GraphQL `200 OK`)](#2-скрити-капани-в-qa-automation-flaky-тестове-threadsleep-споделено-състояние-graphql-200-ok)
* [3. Debugging Techniques за QA инженери (Playwright Trace Viewer, HAR Files, DevTools Network, JMeter)](#3-debugging-techniques-за-qa-инженери-playwright-trace-viewer-har-files-devtools-network-jmeter)

---

## 1. ИНТЕРВЮ ВЪПРОСНИК (TOP 6 QA & SDET ВЪПРОСА С ТОЧНИ SENIOR ОТГОВОРИ)

1. **Въпрос:** *„Каква е разликата между **Verification** и **Validation**, и между **Severity** и **Priority**?“*
   * **Отговор:**
     * **Verification** (*„Строим ли продукта правилно?“*): Статични проверки (преглед на спецификации, Code Review) преди изпълнение на кода.
     * **Validation** (*„Построили ли сме правилния продукт?“*): Динамично тестване на работещия софтуер спрямо нуждите на потребителя.
     * **Severity** е техническата тежест на бъга върху системата (напр. срив на сървъра = *Blocker/Critical*), докато **Priority** е бизнес спешността за поправка (напр. сгрешено лого на началната страница има нисък *Severity*, но висок *Priority* преди официален релийз).

2. **Въпрос:** *„Какво са **Flaky тестовете** и какви са 4-те основни начина да ги елиминираме?“*
   * **Отговор:** Flaky тест е тест, който при един и същ код ту минава зелено, ту пада червено без промени по приложението. Елиминираме ги чрез:
     1. Премахване на всички фиксирани паузи (`Thread.Sleep` / `waitForTimeout`) и замяна с **Explicit / Auto-Waiting** и `waitForResponse`;
     2. Пълна изолация на тестовите данни (всеки тест създава свои записи с уникален `UUID`);
     3. Стабилни локатори (`data-testid` и ARIA роли вместо динамични ID-та и абсолютен XPath);
     4. Прескачане на бавния UI Login чрез API Login и споделен `storageState`.

3. **Въпрос:** *„Каква е разликата между **Implicit Wait**, **Explicit Wait** и **Fluent Wait** в Selenium?“*
   * **Отговор:**
     * **Implicit Wait:** Глобална настройка на драйвера, която кара `FindElement` да търси елемент в DOM-а за определено време. **Капан:** Никога не смесвайте Implicit и Explicit Wait в един проект, защото таймаутите им се наслагват непредсказуемо!
     * **Explicit Wait (`WebDriverWait`):** Целенасочено изчакване на конкретно условие за конкретен елемент (напр. `ElementToBeClickable`).
     * **Fluent Wait:** Explicit Wait с настройка на интервала на проверка (*Polling Interval*, напр. на всеки 250ms) и списък от изключения, които да се игнорират по време на чакането.

4. **Въпрос:** *„Как се тества **GraphQL API** и коя е най-голямата уловка спрямо REST?“*
   * **Отговор:** Всички GraphQL заявки отиват като `POST` към един ендпойнт (`/graphql`). Най-голямата уловка е, че при грешка в заявката или липсващи права сървърът често връща **HTTP `200 OK`**, а реалната грешка е вътре в масива `"errors": [...]` в JSON отговора. Затова всеки тест задължително проверява липсата на `errors` и валидността на `data`.

5. **Въпрос:** *„Каква е разликата между **Load Testing**, **Stress Testing** и **Soak (Endurance) Testing**?“*
   * **Отговор:** **Load Testing** измерва поведението при нормално очаквано натоварване (`p95` латентност). **Stress Testing** търси точката на пречупване (*Breaking Point*) над максималния капацитет. **Soak Testing** държи умерено натоварване в продължение на часове, за да открие **Memory Leaks** (утечки на памет) и изчерпване на Database Connection Pool-а.

6. **Въпрос:** *„Какво е **Page Object Model (POM)** и какви са основните му предимства?“*
   * **Отговор:** Архитектурен шаблон, при който всяка страница или компонент от UI-а се представя като отделен клас, съдържащ локаторите и методите за взаимодействие с нея, а тестовите класове съдържат само бизнес сценарии и `Assert` проверки. Това премахва дублирането на код и улеснява поддръжката при промени в интерфейса.

---

## 2. СКРИТИ КАПАНИ В QA AUTOMATION (FLAKY ТЕСТОВЕ, `THREAD.SLEEP`, СПОДЕЛЕНО СЪСТОЯНИЕ, GRAPHQL `200 OK`)

1. **Капанът на тестването през UI на неща, които вече са тествани през API:**
   * Ако тестваш 20 комбинации на валидация на парола или регистрационна форма през браузъра, тест пакетът става бавен и крехък.
   * **Решение:** Тествай 18-те гранични комбинации за милисекунди през **API / Unit тестове**, а през **Playwright UI** пусни само 2 E2E сценария (успешна регистрация и визуализация на грешка).

2. **Капанът на Hardcoded URL-и, пароли и среди в тестовия код:**
   * Ако `https://shop.bg` и паролите са записани директно в C# / TS файловете, не можеш да пуснеш същите тестове на `QA` или `Local` среда.
   * **Решение:** Използвай `.env` файлове, `appsettings.json` или **GitHub Actions Secrets** (`process.env.BASE_URL`).

3. **Капанът на `Assert` върху несортирани списъци от база данни:**
   * SQL заявки без `ORDER BY` не гарантират реда на записите. Тест, който проверява `expect(items[0]).toBe("Book A")`, ще пада произволно.
   * **Решение:** Сортирай резултата преди сравнение или ползвай проверки за съдържание (`expect(items).toContain("Book A")`).

---

## 3. DEBUGGING TECHNIQUES ЗА QA ИНЖЕНЕРИ (PLAYWRIGHT TRACE VIEWER, HAR FILES, DEVTOOLS NETWORK, JMETER)

*   **Playwright Trace Viewer (Time-Travel Debugging за CI/CD):**
    * Включи `trace: "on-first-retry"` в `playwright.config.ts`. Когато тест падне в GitHub Actions, свали `trace.zip` и го отвори с `npx playwright show-trace trace.zip`.
    * Получаваш **видео запис стъпка по стъпка**, DOM снимка (Snapshot) преди и след всеки клик, всички конзолни грешки и пълните HTTP заявки/отговори!

*   **Експортиране на `.har` (HTTP Archive) файл при докладване на Bug в Jira:**
    * Отвори Chrome DevTools (`F12`) ➔ таб **Network** ➔ отметни **Preserve log** ➔ възпроизведи бъга ➔ кликни иконата **Export HAR** (стрелка надолу).
    * Прикачи `.har` файла в Jira тикета — така Back-End програмистът вижда точните Request Headers, Payload, Cookies и Response Status на счупената заявка.

*   **Бърза проверка на CSS и XPath локатори директно в Browser Console:**
    * За CSS селектор: напиши `$$("button[data-testid='buy']")` в конзолата на Chrome — връща масив с всички намерени елементи.
    * За XPath: напиши `$x("//button[contains(text(), 'Купи')]")` — мигновена проверка без да рестартираш теста!
    
*   **Диагностика в Apache JMeter (`View Results Tree`):**
    * При разработка на JMeter план използвай **View Results Tree** (табове *Request* и *Response Data*), за да провериш дали сървърът не връща HTML login страница с `200 OK` вместо реални данни при грешен токен.

---
[⬅️ 01-98. QA Common Errors & Fixes](01-98-Common-Errors-And-Fixes.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 02-TypeScript-Language](../02-TypeScript-Language/README.md)
