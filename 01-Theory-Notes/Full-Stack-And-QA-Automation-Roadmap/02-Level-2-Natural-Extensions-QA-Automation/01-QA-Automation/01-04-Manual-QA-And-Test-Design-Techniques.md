[⬅️ 01-03. QA Cheat Sheet & Reference Guide](01-03-QA-Cheat-Sheet-And-Reference-Guide.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-05. Multi-Language Unit Testing](01-05-Multi-Language-Unit-Testing.md)

---

# 01-04. Manual QA Fundamentals & Test Design Techniques (Теория и Практика)

**Цел:** Овладяване на фундаменталните процеси в софтуерното тестване — **STLC (Software Testing Life Cycle)**, тестовата пирамида, нивата и видовете тестване, професионално разписване на **Test Cases** и четирите златни **Test Design техники** за откриване на гранични бъгове.

## 🧭 Съдържание на темата
* [1. Тестовата пирамида и нивата на тестване (Unit ➔ Integration ➔ System ➔ UAT)](#1-тестовата-пирамида-и-нивата-на-тестване-unit--integration--system--uat)
* [2. Видове тестване (Functional vs Non-Functional, Black-Box vs White-Box, Smoke vs Regression)](#2-видове-тестване-functional-vs-non-functional-black-box-vs-white-box-smoke-vs-regression)
* [3. Четирите златни Test Design техники (EP, BVA, Decision Tables, State Transition)](#3-четирите-златни-test-design-техники-ep-bva-decision-tables-state-transition)
* [4. Структура на професионален Test Case и Bug Report (Jira стандарт)](#4-структура-на-професионален-test-case-и-bug-report-jira-стандарт)

---

## 1. ТЕСТОВАТА ПИРАМИДА И НИВАТА НА ТЕСТВАНЕ (UNIT ➔ INTEGRATION ➔ SYSTEM ➔ UAT)

```text
                    /\
                   /  \       4. UAT / E2E UI Tests (Бавни, скъпи, малко на брой — реални браузъри)
                  /────\
                 /      \     3. System & API Tests (Средна скорост — Postman, RestSharp, REST Assured)
                /────────\
               /          \   2. Integration Tests (Свързване на модули, БД с Testcontainers)
              /────────────\
             /              \ 1. Unit Tests (Мигновени, евтини, най-много на брой — NUnit, JUnit, pytest, Vitest)
            /────────────────\
```

*   **1. Unit Testing (Компонентно тестване):** Тества изолирано един единствен метод или клас (без реална база данни или мрежа). Изпълнява се за милисекунди.
*   **2. Integration Testing (Интеграционно тестване):** Проверява как два или повече компонента работят заедно (напр. `OrderService` + `PostgreSQL` база данни).
*   **3. System Testing (Системно тестване):** Тества цялостната завършена система от край до край спрямо бизнес изискванията.
*   **4. Acceptance Testing (UAT — Приемателно тестване):** Финална проверка от клиента или Product Owner-а преди пускане в продукция (*Go-Live*).

---

## 2. ВИДОВЕ ТЕСТВАНЕ (FUNCTIONAL VS NON-FUNCTIONAL, BLACK-BOX VS WHITE-BOX, SMOKE VS REGRESSION)

| Класификация | Вид тестване | Какво проверява |
| :--- | :--- | :--- |
| **Според достъпа до кода** | **Black-Box** срещу **White-Box** | **Black-Box:** Тестваш само вход и изход без да виждаш кода. **White-Box:** Познаваш вътрешната логика, клоновете (`if/else`) и покритието на кода (*Code Coverage*). |
| **Според изпълнението** | **Static** срещу **Dynamic** | **Static:** Преглед на изисквания, Code Review, статични анализатори (`SonarQube`, `ESLint`) без стартиране на програмата. **Dynamic:** Реално изпълнение на кода с тестови данни. |
| **Според целта** | **Functional** срещу **Non-Functional** | **Functional:** *Какво* прави системата (вход, плащане, търсене). **Non-Functional:** *Как* го прави (скорост, натоварване с K6/JMeter, сигурност, достъпност). |
| **При нов билд** | **Smoke** срещу **Sanity** | **Smoke:** Бърза проверка на най-критичните функции (*„Пуши ли сървърът при старт?“*). **Sanity:** Тясна проверка на конкретно поправен бъг. |
| **След промени** | **Regression Testing** | Повторно изпълнение на съществуващите тестове, за да гарантираме, че новата функционалност не е счупила стара. |

---

## 3. ЧЕТИРИТЕ ЗЛАТНИ TEST DESIGN ТЕХНИКИ (EP, BVA, DECISION TABLES, STATE TRANSITION)

### 1. Equivalence Partitioning (EP — Класове на еквивалентност)
Разделяме входните данни на групи, за които системата трябва да се държи по еднакъв начин, и тестваме по **1 представител** от всеки клас.
*   *Пример:* Поле за възраст приема валидни стойности от **18 до 65 години**:
    *   Невалиден клас 1: `< 18` (напр. `12`)
    *   Валиден клас: `18 – 65` (напр. `35`)
    *   Невалиден клас 2: `> 65` (напр. `80`)

### 2. Boundary Value Analysis (BVA — Анализ на граничните стойности)
Повечето бъгове се крият **точно на границите** (`<` вместо `<=`). За всяка граница тестваме точната стойност, стойността `-1` и стойността `+1`!
*   *Пример за диапазон `18 – 65` (6 гранични теста):*
    *   Долна граница (`18`): **`17`** (Невалидно), **`18`** (Валидно), **`19`** (Валидно)
    *   Горна граница (`65`): **`64`** (Валидно), **`65`** (Валидно), **`66`** (Невалидно)

### 3. Decision Table Testing (Таблица на решенията)
Използва се при сложна бизнес логика с няколко комбинирани условия ($2^N$ комбинации).

| Условие / Действие | Правило 1 | Правило 2 | Правило 3 | Правило 4 |
| :--- | :---: | :---: | :---: | :---: |
| **Регистриран клиент (`isMember`)** | Да | Да | Не | Не |
| **Поръчка над 100 лв. (`over100`)** | Да | Не | Да | Не |
| **Резултат: Отстъпка (%)** | **15%** | **5%** | **10%** | **0%** |

### 4. State Transition Testing (Преходи на състоянията)
Проверява валидните и **забранените** преходи на един обект (напр. поръчка: `New ➔ Paid ➔ Shipped ➔ Delivered`, но преходът `Delivered ➔ Cancelled` трябва да бъде блокиран!).

---

## 4. СТРУКТУРА НА ПРОФЕСИОНАЛЕН TEST CASE И BUG REPORT (JIRA СТАНДАРТ)

### Шаблон за Test Case:
*   **Test Case ID:** `TC-AUTH-001`
*   **Title:** Успешен вход с валидни имейл и парола
*   **Preconditions:** Потребителят има активен акаунт `user@test.bg`
*   **Test Steps:**
    1. Отвори `/login`
    2. Въведи `user@test.bg` в полето Email и `Pass123!` в Password
    3. Натисни бутона `Login`
*   **Expected Result:** Пренасочване към `/dashboard` и визуализиране на профилното меню.

### Шаблон за Bug Report в Jira:
*   **Summary (Къде + Какво + Кога):** `[Checkout] Бутонът "Плати" връща грешка 500 при въвеждане на валиден промо код с малки букви`
*   **Environment:** Chrome 131 / Windows 11 / Staging API v2.4
*   **Severity vs Priority:**
    *   **Severity (Тежест — техническо въздействие):** *Critical* (блокира плащането).
    *   **Priority (Приоритет — бизнес спешност):** *High* (трябва да се оправи веднага преди релийз).
*   **Steps to Reproduce** + **Actual Result** vs **Expected Result** + **Attachments** (Screenshot / HAR файл от Network таба).

---
[⬅️ 01-03. QA Cheat Sheet & Reference Guide](01-03-QA-Cheat-Sheet-And-Reference-Guide.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-05. Multi-Language Unit Testing](01-05-Multi-Language-Unit-Testing.md)
