[⬅️ 01-02-00. Test Design Техники, Jira, CI/CD & MS Project](01-02-00-Test-Design-Jira-And-MS-Project.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-03-00. Unit Testing (AAA, Assertions & Mocking)](01-03-00-Multi-Language-Unit-Testing.md)

---

# 01-02-01. JQL, Jira Automation, CI/CD & Cheat Sheet (Практика от Нулата)

**Раздел 01:** QA Fundamentals & Manual | **Цел:** Практическо прилагане на **Decision Tables** от нулата, писане на мощни **JQL (Jira Query Language)** заявки за филтриране на дефекти и регресионни тестове, конфигуриране на **Jira Automation** правила без код, интегриране на автоматизирани тестове в **GitHub Actions CI/CD** с **Allure Report**, работа с **Microsoft Project** и бърза справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Практически пример от нулата: Decision Table (Таблица на решенията)](#1-практически-пример-от-нулата-decision-table-таблица-на-решенията)
* [2. JQL (Jira Query Language) от нулата: Готови заявки за QA инженери (Вход / Изход)](#2-jql-jira-query-language-от-нулата-готови-заявки-за-qa-инженери-вход--изход)
* [3. Jira Workflow, WIP Limits и Автоматизации (No-Code Rules)](#3-jira-workflow-wip-limits-и-автоматизации-no-code-rules)
* [4. CI/CD Интеграция: GitHub Actions YAML за автоматични тестове & Allure Report](#4-cicd-интеграция-github-actions-yaml-за-автоматични-тестове--allure-report)
* [5. Microsoft Project: Практически стъпки за изграждане на график и Critical Path](#5-microsoft-project-практически-стъпки-за-изграждане-на-график-и-critical-path)
* [6. Обобщение / Бърз Cheat Sheet](#6-обобщение--бърз-cheat-sheet)

---

## 1. ПРАКТИЧЕСКИ ПРИМЕР ОТ НУЛАТА: DECISION TABLE (ТАБЛИЦА НА РЕШЕНИЯТА)

**Бизнес правило:** Онлайн магазин дава **15% отстъпка**, ако клиентът е *VIP* **ИЛИ** ако *сумата на поръчката е над 200 EUR* и има валиден *Промокод*:

| Условия / Правила (Rules) | Rule 1 | Rule 2 | Rule 3 | Rule 4 | Rule 5 | Rule 6 |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Условие 1:** Клиентът е VIP? | **Да (T)** | Не (F) | Не (F) | Не (F) | Не (F) | **Да (T)** |
| **Условие 2:** Сума > 200 EUR? | — *(Без знач.)* | **Да (T)** | **Да (T)** | Не (F) | Не (F) | **Да (T)** |
| **Условие 3:** Валиден Промокод? | — *(Без знач.)* | **Да (T)** | Не (F) | **Да (T)** | Не (F) | **Да (T)** |
| **Действие:** Прилага се 15% отстъпка? | ✅ **ДА** | ✅ **ДА** | ❌ НЕ | ❌ НЕ | ❌ НЕ | ✅ **ДА** *(без натрупване!)* |

---

## 2. JQL (JIRA QUERY LANGUAGE) ОТ НУЛАТА: ГОТОВИ ЗАЯВКИ ЗА QA ИНЖЕНЕРИ (ВХОД / ИЗХОД)

В Jira полето **Advanced Search** използва езика **JQL**, чийто синтаксис напомня SQL (`Поле + Оператор + Стойност`):

### 🔹 Заявка 1: Всички отворени Critical/High бъгове в текущия спринт, които чакат тестване
* **Вход (JQL):**
  ```sql
  project = "DEVLIB" AND issuetype = Bug AND sprint in openSprints() AND priority in (Highest, High) AND status = "Ready for QA" ORDER BY created DESC
  ```
* **Изход:** Списък с най-спешните дефекти за текущия спринт, подредени от най-новите към по-старите, готов за закачане към QA Dashboard.

### 🔹 Заявка 2: Дефекти, открити от мен през последните 7 дни, които все още не са затворени
* **Вход (JQL):**
  ```sql
  reporter = currentUser() AND issuetype = Bug AND status != Done AND created >= -7d
  ```
* **Изход:** Персонален списък за проследяване на собствените ти бъгове през последната седмица.

### 🔹 Заявка 3: Задачи без назначен изпълнител или липсващ етикет за автоматизация
* **Вход (JQL):**
  ```sql
  project = "DEVLIB" AND assignee is EMPTY AND labels not in ("automated", "wont-automate")
  ```
* **Изход:** Изолира всички неразпределени задачи, които все още не са оценени за автоматизация.

---

## 3. JIRA WORKFLOW, WIP LIMITS И АВТОМАТИЗАЦИИ (NO-CODE RULES)

### 📊 Стандартен QA Workflow в Jira (Колони на дъската):
```text
 [To Do] ──► [In Progress] ──► [Code Review] ──► [Ready for QA] ──► [In QA (WIP: 3)] ──► [Done]
```
*(Поставянето на **WIP Limit = 3** върху колона `In QA` гарантира, че тестерите няма да започнат 10 задачи едновременно, а ще завършват и затварят текущите по-бързо.)*

### ⚙️ Полезни Jira Automation правила (`When` ➔ `If` ➔ `Then`):
1. **Автоматично връщане на родителско Story при отворен Bug:**
   * **When:** Създаден е нов `Bug`, свързан със `Story`.
   * **If:** Приоритетът на бъга е `Highest` или `High`.
   * **Then:** Промени статуса на родителското `Story` от `In QA` обратно на `In Progress` и изпрати известие в Slack/Teams.
2. **Автоматично назначаване на QA при преминаване в `Ready for QA`:**
   * **When:** Статусът на задача се промени на `Ready for QA`.
   * **Then:** Назначи (`Assign`) задачата на водещия QA инженер на екипа и добави коментар *„Готово за регресионна проверка на Staging“*.

---

## 4. CI/CD ИНТЕГРАЦИЯ: GITHUB ACTIONS YAML ЗА АВТОМАТИЧНИ ТЕСТОВЕ & ALLURE REPORT

Когато разработчик направи `git push` или отвори `Pull Request`, CI/CD конвейерът автоматично стартира тестовете и публикува визуален отчет:

```yaml
name: QA Automated Regression Suite

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  run-e2e-tests:
    runs-on: ubuntu-latest
    steps:
      - name: 1. Изтегляне на кода (Checkout)
        uses: actions/checkout@v4

      - name: 2. Настройка на Node.js 20
        uses: actions/setup-node@v4
        with:
          node-version: 20

      - name: 3. Инсталация на зависимости и Playwright браузъри
        run: |
          npm ci
          npx playwright install --with-deps chromium

      - name: 4. Изпълнение на автоматизираните тестове
        run: npx playwright test

      - name: 5. Запазване на тестовия отчет като Artifact (дори при паднал тест!)
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-allure-report
          path: playwright-report/
          retention-days: 14
```

---

## 5. MICROSOFT PROJECT: ПРАКТИЧЕСКИ СТЪПКИ ЗА ИЗГРАЖДАНЕ НА ГРАФИК И CRITICAL PATH

1. **Настройка на работното време:** `Project` ➔ `Project Information` (задаване на начална дата) и `Change Working Time` (въвеждане на официални празници и почивни дни).
2. **Въвеждане на WBS йерархия:** Въведи фазите като **Summary Tasks** и вложи конкретните задачи под тях чрез бутона **Indent Task** (`Alt + Shift + Right`).
3. **Свързване на зависимости:** Маркирай последователни задачи и натисни **`Ctrl + F2`** (`Link the Selected Tasks`), за да създадеш автоматична връзка `Finish-to-Start (FS)`.
4. **Визуализация на критичния път:** Отиди на таб **`Gantt Chart Format`** ➔ сложи отметка на **`Critical Tasks`** — всички задачи с `0` резерв светват в **червено** на диаграмата.
5. **Заключване на Baseline:** Отиди на `Project` ➔ `Set Baseline` ➔ `Set Baseline...`, за да запазиш одобрения план преди старта на изпълнението.

---

## 6. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Инструмент / Техника | Синтаксис / Команда / Клавиш | Предназначение |
| :--- | :--- | :--- |
| **EP (Класове на еквивалентност)** | 1 валиден + N невалидни класа | Намалява броя тестове чрез групиране на еднакво поведение. |
| **BVA (Гранични стойности)** | `Min-1`, `Min`, `Min+1`, `Max-1`, `Max`, `Max+1` | Открива дефекти точно на границите на диапазоните. |
| **Decision Table** | $2^N$ комбинации от условия (`T`/`F`) | Покрива сложни бизнес правила и комбинации от флагове. |
| **JQL: Активен спринт** | `sprint in openSprints()` | Филтрира всички задачи и бъгове в текущия активен спринт. |
| **JQL: Промяна на статус** | `status CHANGED TO "Done" AFTER -7d` | Открива всички задачи, затворени през последните 7 дни. |
| **JQL: Търсене в текст** | `summary ~ "Checkout" OR description ~ "500"` | Търси по ключова дума в заглавието или описанието на тикета. |
| **GitHub Actions `if: always()`** | Поставя се в стъпката за качване на репорт | Гарантира, че скрийншотите и логовете ще се запазят и при провал. |
| **MS Project: Свързване на задачи** | `Ctrl + F2` (`Link Tasks`) | Създава последователна зависимост `Finish-to-Start (FS)`. |

---

[⬅️ 01-02-00. Test Design Техники, Jira, CI/CD & MS Project](01-02-00-Test-Design-Jira-And-MS-Project.md) | [📂 Текуща папка: 01. QA Automation](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 02. TypeScript Language](../02-TypeScript-Language/README.md) | [➡️ 01-03-00. Unit Testing (AAA, Assertions & Mocking)](01-03-00-Multi-Language-Unit-Testing.md)

© 2026 Dev-Library / 01-02-01. JQL, Automation, CI/CD & Cheat Sheet (Практика от Нулата)