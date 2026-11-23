[⬅️ 01-01-00. SDLC, STLC, Нива и Видове Тестване](01-01-00-SDLC-STLC-And-Testing-Types.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-02-00. Test Design Техники, Jira, JQL & MS Project](01-02-00-Test-Design-Jira-And-MS-Project.md)

---

# 01-01-01. Шаблони & Cheat Sheet (Test Plan, Test Case, RTM & Bug Lifecycle)

**Раздел 01:** QA Fundamentals & Manual | **Цел:** Готови, стандартизирани инженерни шаблони от нулата за **Test Plan (по IEEE 829)**, **Test Case**, **Requirement Traceability Matrix (RTM)**, пълен жизнен цикъл на дефектите (**Bug Lifecycle**) и бърза 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Шаблон 1: Инженерен Test Plan (Стандартен скелет)](#1-шаблон-1-инженерен-test-plan-стандартен-скелет)
* [2. Шаблон 2: Позитивен и Негативен Test Case (Вход / Очакван изход)](#2-шаблон-2-позитивен-и-негативен-test-case-вход--очакван-изход)
* [3. Шаблон 3: Requirement Traceability Matrix (RTM — Проследимост)](#3-шаблон-3-requirement-traceability-matrix-rtm--проследимост)
* [4. Жизнен цикъл на дефекта (Bug Life Cycle Diagram)](#4-жизнен-цикъл-на-дефекта-bug-life-cycle-diagram)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ШАБЛОН 1: ИНЖЕНЕРЕН TEST PLAN (СТАНДАРТЕН СКЕЛЕТ)

Всеки професионален Test Plan отговаря на въпросите **КАКВО, КАК, КОГА и КОЙ** ще тества:

```markdown
# Test Plan: [Име на проекта / Модул] (v1.0)

## 1. Обхват на тестването (Scope)
* **In-Scope (Какво тестваме):** Регистрация, Вход (Login), Продуктов каталог, Количка и REST API `/api/v1/orders`.
* **Out-of-Scope (Какво НЕ тестваме):** Външния платежен шлюз на Stripe (използваме Sandbox Mock), стари версии на IE11.

## 2. Тестова стратегия и видове тестове
* **Functional:** Smoke, Sanity, Integration и System E2E тестове.
* **Non-Functional:** Load тестване с K6 (до 500 едновременни потребители).

## 3. Тестова среда и инструменти (Environment & Tools)
* **Среда:** Staging (`https://app.local`), PostgreSQL 16, Redis 7.
* **Инструменти:** Jira (Bug Tracking), TestRail (Test Management), Playwright (UI), Postman (API).

## 4. Критерии за вход и изход (Entry & Exit Criteria)
* **Entry Criteria:** Кодът е компилиран успешно, преминал е 100% от Unit тестовете и е деплойнат на Staging.
* **Exit Criteria:** Изпълнени 100% от планираните Test Cases; **0 отворени Critical/Blocker бъгове**; над 95% Pass Rate.
```

---

## 2. ШАБЛОН 2: ПОЗИТИВЕН И НЕГАТИВЕН TEST CASE (ВХОД / ОЧАКВАН ИЗХОД)

| Поле | Позитивен сценарий (`TC-AUTH-001`) | Негативен сценарий (`TC-AUTH-002`) |
| :--- | :--- | :--- |
| **Заглавие (Title)** | Успешен вход в системата с валиден имейл и парола | Отказ за достъп при въвеждане на грешна парола |
| **Priority** | High (P1) | High (P1) |
| **Preconditions** | Съществува активен профил `elena@devlib.bg` с парола `Pass1234!`. | Съществува активен профил `elena@devlib.bg`. |
| **Test Steps** | 1. Отвори `/login`.<br>2. Въведи `elena@devlib.bg` в поле Email.<br>3. Въведи `Pass1234!` в поле Password.<br>4. Натисни бутон `Login`. | 1. Отвори `/login`.<br>2. Въведи `elena@devlib.bg` в поле Email.<br>3. Въведи `WrongPass999` в поле Password.<br>4. Натисни бутон `Login`. |
| **Expected Result** | Системата пренасочва към `/dashboard`, издава валиден JWT токен и показва името на потребителя. | Потребителят остава на `/login`; показва се общо съобщение `"Invalid email or password"` (без да разкрива кое точно е грешно). |

---

## 3. ШАБЛОН 3: REQUIREMENT TRACEABILITY MATRIX (RTM — ПРОСЛЕДИМОСТ)

Матрицата **RTM** гарантира, че **всяко бизнес изискване** е покрито с поне един тестов случай и нищо не е забравено:

| Requirement ID | Описание на бизнес изискването | Свързани Test Cases | Статус на покритие | Свързан Bug ID |
| :--- | :--- | :--- | :--- | :--- |
| **REQ-01** | Потребителят може да се регистрира с валиден имейл и парола (мин. 8 символа). | `TC-REG-001`, `TC-REG-002` | ✅ Covered (100%) | Няма |
| **REQ-02** | Прилагане на промокод за отстъпка при поръчки над 100 EUR. | `TC-CHK-005`, `TC-CHK-006` | ⚠️ Failed | `BUG-1042` (Open) |
| **REQ-03** | Заключване на акаунта след 5 неуспешни опита за вход (Brute-Force защита). | `TC-SEC-001` | ✅ Covered (100%) | Няма |

---

## 4. ЖИЗНЕН ЦИКЪЛ НА ДЕФЕКТА (BUG LIFE CYCLE DIAGRAM)

```text
  [New / Open] ──► (Team Lead / PM валидира) ──► [Assigned] ──► (Developer поправя) ──► [Fixed / Resolved]
       │                                                                                         │
       ├──► [Rejected / Invalid] (Ако не е бъг, а очаквано поведение)                            ▼
       ├──► [Duplicate] (Ако вече е докладван)                                           [Ready for QA / Retest]
       └──► [Deferred] (Отложен за следващ релийз)                                               │
                                                                                   ┌─────────────┴─────────────┐
                                                                                   ▼ (Тестът минава)           ▼ (Тестът пак пада)
                                                                             [Verified / Closed]          [Reopened] ──► (Обратно към Dev)
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Понятие / Документ | Основна цел | Ключов пример / Правило |
| :--- | :--- | :--- |
| **QA vs. QC vs. Testing** | Превенция срещу Инспекция срещу Изпълнение | QA = процеси; QC = продукт; Testing = пускане на тестове. |
| **Verification vs. Validation** | Проверка на спецификация срещу проверка на реален софтуер | Verification = статично (ревю); Validation = динамично (изпълнение). |
| **Smoke vs. Sanity** | Широка базова проверка срещу дълбока проверка на фикс | Smoke = „работи ли изобщо билдът?“; Sanity = „оправен ли е конкретният фикс?“. |
| **Severity vs. Priority** | Техническо въздействие върху системата срещу бизнес спешност | Определят се независимо (може да има High Severity + Low Priority). |
| **Test Plan** | Документ за стратегия, обхват, среда и критерии | Дефинира ясни **Entry Criteria** и **Exit Criteria** преди релийз. |
| **Test Case** | Стъпка по стъпка сценарий с входни данни и очакван резултат | Винаги включва **Preconditions**, **Steps** и **Expected Result**. |
| **RTM (Traceability Matrix)** | Таблица за връзка между бизнес изискванията и тестовете | Гарантира 100% покритие на изискванията и проследимост на бъговете. |
| **Bug Report (Златно правило)** | Всяка грешка се описва така, че да се възпроизведе от първия път | Задължително съдържа **Environment**, **Steps**, **Expected** и **Actual Result**. |

---

[⬅️ 01-01-00. SDLC, STLC, Нива и Видове Тестване](01-01-00-SDLC-STLC-And-Testing-Types.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-02-00. Test Design Техники, Jira, JQL & MS Project](01-02-00-Test-Design-Jira-And-MS-Project.md)

© 2026 Dev-Library / 01-01-01. Шаблони & Cheat Sheet (Test Plan, Test Case, RTM & Bug Lifecycle)