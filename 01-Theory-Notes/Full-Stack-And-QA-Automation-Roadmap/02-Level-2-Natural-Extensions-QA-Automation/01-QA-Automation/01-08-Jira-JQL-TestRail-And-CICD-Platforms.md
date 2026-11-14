[⬅️ 01-07. API (REST & GraphQL) & Performance Testing](01-07-API-And-Performance-Testing.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-96. QA Performance & Optimizations](01-96-Performance-And-Optimizations.md)

---

# 01-08. Индустриални QA Платформи: Jira Software, JQL, Xray/TestRail & CI/CD

**Цел:** Пълно практическо ръководство за ежедневна работа с индустриалните платформи за управление на проекти, дефекти и тестове — **Jira Software** (проекти, schemes, bug workflows, Scrum/Kanban boards, JQL, automations), **Xray / Zephyr Scale**, **TestRail**, **Azure DevOps** и **GitHub Actions CI/CD**.

## 🧭 Съдържание на темата
* [1. Jira Software: Създаване на проект, Йерархия на Issues & Bug Lifecycle Workflow](#1-jira-software-създаване-на-проект-йерархия-на-issues--bug-lifecycle-workflow)
* [2. Работа с Boards (Scrum срещу Kanban) & Настройка на Quick Filters и Dashboards](#2-работа-с-boards-scrum-срещу-kanban--настройка-на-quick-filters-и-dashboards)
* [3. Jira Query Language (JQL): Синтаксис, Оператори и Практически заявки за QA](#3-jira-query-language-jql-синтаксис-оператори-и-практически-заявки-за-qa)
* [4. Jira Automations & Test Management Платформи (Xray / Zephyr / TestRail / Azure DevOps)](#4-jira-automations--test-management-платформи-xray--zephyr--testrail--azure-devops)
* [5. Интеграция на автоматизирани тестове в GitHub Actions CI/CD Pipeline](#5-интеграция-на-автоматизирани-тестове-в-github-actions-cicd-pipeline)
* [6. MICROSOFT PROJECT (MS PROJECT): ПЛАНИРАНЕ НА QA ФАЗИ, WBS, GANTT & CRITICAL PATH](#6-microsoft-project-ms-project-планиране-на-qa-фази-wbs-gantt--critical-path)

---

## 1. JIRA SOFTWARE: СЪЗДАВАНЕ НА ПРОЕКТ, ЙЕРАРХИЯ НА ISSUES & BUG LIFECYCLE WORKFLOW

### 1.1 Видове проекти и Schemes (Схеми за конфигурация)
* **Team-managed Project:** Екипът сам управлява колоните, полетата и правилата. Подходящ е за малки самостоятелни екипи без нужда от споделени корпоративни стандарти.
* **Company-managed Project (Enterprise стандарт):** Управлява се от администратор чрез централизирани **Schemes**, които се споделят между много проекти.
  * **Issue Type Scheme:** Определя наличните типове тикети (`Epic`, `Story`, `Task`, `Bug`, `Sub-task`).
  * **Workflow Scheme:** Определя статусите и позволените преходи (*Transitions*) за всеки тип тикет.
  * **Screen & Field Configuration Scheme:** Определя кои полета са задължителни при създаване и редакция на Bug (напр. *Environment*, *Steps to Reproduce*, *Severity*, *Affects Version*).

### 1.2 Йерархия на работата и връзки между тикети
* **Epic:** Голяма бизнес цел или модул (напр. *„Система за онлайн плащания“*).
* **User Story:** Конкретна функционалност от гледна точка на потребителя (напр. *„Плащане с дебитна карта“*).
* **Task / Sub-task:** Технически подзадачи за разработка или писане на тестове.
* **Bug:** Открит дефект, който се свързва към Story-то чрез **Issue Links** (`blocks`, `is blocked by`, `is caused by`, `relates to`).

### 1.3 Жизнен цикъл на бъга (Bug Workflow)
* **Стандартен поток:** `Open / To Do` ➔ `In Progress` (Dev поправка) ➔ `Ready for QA` (качен на тестова среда) ➔ `In Testing` (QA проверка) ➔ `Done / Closed` (потвърден) или `Reopened` (ако бъгът все още се възпроизвежда).
* **Златно правило за QA:** QA инженерът никога не трие невалидни или дублирани бъгове и не затваря чужди тикети без ретест. Дублираните тикети се затварят със статус **`Closed`** и резолюция **`Duplicate`**, **`Cannot Reproduce`** или **`Won't Do`**.

---

## 2. РАБОТА С BOARDS (SCRUM СРЕЩУ KANBAN) & НАСТРОЙКА НА QUICK FILTERS И DASHBOARDS

### 2.1 Scrum Board срещу Kanban Board

| Критерий | 🏃 Scrum Board | 📋 Kanban Board |
| :--- | :--- | :--- |
| **Ритъм на работа** | Фиксирани времеви цикли (**Sprints** от 1–2 седмици) | Непрекъснат поток (**Continuous Flow**) без спринтове |
| **Планиране** | Задачи се прехвърлят от **Backlog** в активен Sprint | Новите задачи влизат директно в колона `To Do` по приоритет |
| **Контрол на натоварването** | Чрез **Story Points** и среден капацитет (**Velocity**) | Чрез **WIP Limits** (максимален брой задачи в една колона) |
| **Приложение в QA** | Продуктова разработка с регулярни релийзи | Поддръжка, DevOps и екипи за бързи поправки (Hotfixes) |

### 2.2 Стъпка по стъпка: Настройка на QA дъска (`Board Settings`)
1. **Mapping на колоните (`Columns`):** Разделете колоната за проверка на **`Ready for QA`** (чакащи тест) и **`In Testing`** (в процес на тест). Поставете **WIP Limit** (напр. максимум 3 тикета в `In Testing`), за да не се натрупва незавършена работа.
2. **Настройка на Swimlanes:** В `Board Settings ➔ Swimlanes` изберете групиране по **`Stories`** (всяко User Story става хоризонтален ред със своите Sub-tasks и Bugs отдолу) или по **`Assignees`**.
3. **Създаване на Quick Filters:** Добавете бързи бутони над дъската за мигновено филтриране по време на Daily Standup:
   * `Only Bugs`: `issuetype = Bug`
   * `My QA Queue`: `status = "Ready for QA" AND assignee = currentUser()`
   * `Critical & High`: `priority in (Highest, High)`

### 2.3 Изграждане на QA Dashboard & Gadgets
В менюто **Dashboards ➔ Create dashboard** добавете следните четири графични джаджи (**Gadgets**) за следене на качеството:
* **Pie Chart:** Разпределение на отворените бъгове по `Priority` или `Status`.
* **Created vs. Resolved Chart:** Сравнява темпото на откриване спрямо темпото на затваряне на дефекти за последните 14/30 дни.
* **Filter Results:** Таблица с всички тикети в статус `Ready for QA`, подредени по приоритет.
* **Two Dimensional Filter Statistics:** Матрица между `Assignee` (по редове) и `Status` (по колони).

---

## 3. JIRA QUERY LANGUAGE (JQL): СИНТАКСИС, ОПЕРАТОРИ И ПРАКТИЧЕСКИ ЗАЯВКИ ЗА QA

JQL заявките следват структурата **`Field` + `Operator` + `Value`**, комбинирани с логическите думи `AND`, `OR`, `NOT` и клаузата `ORDER BY`.

### 3.1 Най-важните JQL оператори и функции

| Оператор / Функция | Предназначение | Пример |
| :--- | :--- | :--- |
| **`=` / `!=`** | Точно съвпадение или изключване | `issuetype = Bug AND status != Closed` |
| **`IN` / `NOT IN`** | Проверка в списък от стойности | `priority IN (Highest, High)` |
| **`~` (Contains)** | Търсене на дума или фраза в текст | `summary ~ "checkout" OR description ~ "timeout"` |
| **`WAS` / `CHANGED`** | Проверка на историята на промените | `status WAS "In Testing" AND status CHANGED TO "Reopened"` |
| **`IS EMPTY` / `IS NOT EMPTY`** | Проверка за непопълнено поле | `fixVersion IS EMPTY` |
| **`currentUser()`** | Текущо логнатият потребител | `reporter = currentUser()` |
| **`openSprints()`** | Всички активни спринтове в момента | `sprint IN openSprints()` |
| **`-7d` / `-24h`** | Относителен период назад във времето | `created >= -7d` |

### 3.2 Готови JQL заявки за ежедневна QA работа
```sql
-- 1. Всички критични и високи отворени бъгове в проекта
project = "SHOP" AND issuetype = Bug AND priority IN (Highest, High) AND status NOT IN (Done, Closed) ORDER BY priority DESC, created ASC

-- 2. Моята лична опашка от задачи, готови за тестване
project = "SHOP" AND status = "Ready for QA" AND assignee = currentUser()

-- 3. Бъгове, докладвани от мен през последните 7 дни
issuetype = Bug AND reporter = currentUser() AND created >= -7d ORDER BY created DESC

-- 4. Бъгове, които са били върнати обратно за поправка (Reopened) в текущия спринт
project = "SHOP" AND sprint IN openSprints() AND status CHANGED TO "Reopened"

-- 5. User Stories в спринта, които нямат нито един свързан тест или подзадача
project = "SHOP" AND sprint IN openSprints() AND issuetype = Story AND issueLinkType IS EMPTY
```
---

## 4. JIRA AUTOMATIONS & TEST MANAGEMENT ПЛАТФОРМИ (XRAY / ZEPHYR / TESTRAIL / AZURE DEVOPS)

### 4.1 Jira Automation Правила (No-Code Workflow Engine)
Всяко автоматизационно правило в менюто **Project Settings ➔ Automation** се изгражда от три основни компонента: **Trigger** (Събитие) ➔ **Condition** (Условие) ➔ **Action** (Действие).

* **Правило 1 (Автоматично възлагане на QA при готов код):**
  * **Trigger:** `Issue transitioned` ➔ от `In Progress` към `Ready for QA`.
  * **Condition:** `Issue fields condition` ➔ `Issue Type` е равно на `Bug` или `Story`.
  * **Action:** `Assign issue` ➔ възлага задачата на QA инженера в екипа и изпраща нотификация в Slack/Teams.
* **Правило 2 (Автоматично затваряне на Parent Story при готови подзадачи):**
  * **Trigger:** `Issue transitioned` ➔ към статус `Done`.
  * **Branch / Related Issues:** `For Parent` ➔ с условие `All sub-tasks match status = Done`.
  * **Action:** `Transition issue` ➔ премества главното `User Story` автоматично в `Done`.

### 4.2 Сравнение на най-търсените Test Management платформи

| Платформа | Интеграция | Основни обекти и работен поток | Кога се избира |
| :--- | :--- | :--- | :--- |
| **Xray / Zephyr Scale** | Директно вътре в **Jira** | `Test` ➔ `Test Set` ➔ `Test Plan` ➔ `Test Execution` (всички обекти са Jira Issues). | Когато екипът изисква 100% проследимост (*Requirements Traceability*) директно вътре в Jira. |
| **TestRail** | Външна уеб платформа + Jira API | `Milestones` ➔ `Test Suites & Cases` ➔ `Test Runs` (свързва се с Jira тикети чрез ID). | Когато има стотици ръчни и автоматизирани тестове и трябват детайлни отчети за мениджмънта. |
| **Azure DevOps Test Plans** | Вградена в **Azure DevOps** | `Test Plan` ➔ `Test Suite` ➔ `Test Case` + директна връзка с Azure Pipelines. | Златен стандарт в компании, които работят с C# / .NET и Microsoft екосистемата. |
| **Allure Report / TestOps** | Open-source репортер за CI/CD | Генерира интерактивен HTML отчет със стъпки, скрийншоти, видео и логове от Playwright, JUnit или pytest. | Най-добрият визуален отчет за автоматизирани тестове в GitHub Actions и GitLab CI. |

---

## 5. ИНТЕГРАЦИЯ НА АВТОМАТИЗИРАНИ ТЕСТОВЕ В GITHUB ACTIONS CI/CD PIPELINE

При всеки `git push` или `Pull Request` към основния клон `main`, CI/CD сървърът автоматично вдига чиста виртуална машина, инсталира зависимостите, стартира тестовете и блокира сливането на кода (*Merge*), ако дори един тест се провали.

```yaml
# .github/workflows/qa-automation.yml
name: QA E2E & API Regression Tests

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  playwright-e2e:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup Node.js 20
        uses: actions/setup-node@v4
        with:
          node-version: 20

      - name: Install dependencies & Playwright Browsers
        run: |
          npm ci
          npx playwright install --with-deps chromium

      - name: Run Playwright E2E & API Tests
        run: npx playwright test
        env:
          BASE_URL: \${{ secrets.STAGING_URL }}

      - name: Upload Playwright HTML Report & Traces on Failure
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: playwright-report/
          retention-days: 14
```

---

---

## 6. MICROSOFT PROJECT (MS PROJECT): ПЛАНИРАНЕ НА QA ФАЗИ, WBS, GANTT & CRITICAL PATH

Докато **Jira** управлява ежедневната гъвкава разработка (Scrum спринтове и бъгове), **Microsoft Project** е индустриалният стандарт за цялостно планиране на графици, ресурси, бюджети и зависимости при големи проекти (**Waterfall** или **Hybrid Agile-Waterfall**).

### 6.1 Ключови концепции в MS Project за QA и Tech Leads

| Понятие в MS Project | Какво представлява | Приложение при планиране на QA дейности |
| :--- | :--- | :--- |
| **WBS (Work Breakdown Structure)** | Йерархично разбиване на проекта на фази, пакети от задачи и подзадачи. | Фаза 1: *Test Planning* ➔ Фаза 2: *Test Case Design* ➔ Фаза 3: *Test Execution & Regression* ➔ Фаза 4: *UAT & Sign-off*. |
| **Task Dependencies (Зависимости)** | Логически връзки между задачите: **FS** (*Finish-to-Start*), **SS** (*Start-to-Start*), **FF** (*Finish-to-Finish*). | *QA Execution* не може да започне преди *Dev Feature Freeze* (**FS**), но *Writing Test Cases* започва паралелно с разработката (**SS**). |
| **Gantt Chart (Диаграма на Гант)** | Визуална времева линия с ленти за продължителността на всяка задача, етапи и връзки. | Дава мигновен поглед на мениджмънта кога започва регресионното тестване и кога е датата за продукционен релийз. |
| **Critical Path (Критичен път)** | Най-дългата верига от зависими задачи, чието закъснение с 1 ден отлага крайния срок на целия проект (**0 дни буфер / Slack**). | Помага на QA мениджъра да види кои тестови задачи са на критичния път и не търпят отлагане. |
| **Milestones (Ключови етапи)** | Контролни точки с продължителност **0 дни** (отбелязани с ромб на диаграмата). | Примери: `Code Freeze (0d)`, `QA Sign-Off (0d)`, `Go-Live Production Release (0d)`. |
| **Resource Allocation & Overallocation** | Разпределяне на QA инженери и тестови среди по задачи и следене на претоварване (червено човече). | Предотвратява ситуация, в която един QA инженер е разпределен на 200% в два паралелни модула. |
| **Baseline (Базова линия)** | Замразена „снимка“ на първоначалния одобрен план (дати, часове, разходи). | Позволява сравнение между **Планирано (Baseline)** и **Реално (Actual)** време за тестване. |

### 6.2 Стъпка по стъпка: Как се изгражда QA график в MS Project

1. **Настройка на календара (`Project ➔ Change Working Time`):** Задайте официалните почивни дни и работните часове на екипа (напр. 8 часа/ден), за да изчислява MS Project реалните крайни дати.
2. **Избор на режим `Auto Scheduled`:** Задайте всички задачи да бъдат **Auto Scheduled** (а не *Manually Scheduled*), за да може при промяна на една дата целият график надолу да се преизчисли автоматично.
3. **Въвеждане на WBS и Milestones:** Създайте главна задача (*Summary Task*) `4. Quality Assurance & Testing` и въведете подзадачите с точни оценки (*Duration* в дни) и финалния етап `QA Sign-Off` с продължителност `0 days`.
4. **Свързване на зависимостите (`Predecessors`):** Попълнете колоната *Predecessors* (напр. `12FS` означава, че задачата стартира веднага след приключване на ред 12) и включете отметката **`Critical Tasks`** в таба *Gantt Chart Format*, за да видите критичния път в червено.
5. **Запазване на Baseline (`Project ➔ Set Baseline`):** Преди началото на тестовете запазете `Baseline 0`, за да следите отклоненията при добавяне на нови бъгове и ретестове.

### 6.3 Jira срещу Microsoft Project (Кога кое се използва?)

| Критерий | 🔷 Jira Software | 📊 Microsoft Project |
| :--- | :--- | :--- |
| **Основен фокус** | Ежедневно проследяване на задачи, бъгове, спринтове и код | Цялостно планиране на графици, ресурси, критичен път и бюджети |
| **Основен изглед** | Scrum / Kanban дъски, Backlog и JQL филтри | Диаграма на Гант (Gantt Chart), WBS таблица и Resource Sheet |
| **Хоризонт на планиране** | Краткосрочен (1–2 седмици в спринт) | Дългосрочен (месеци и години напред с Baseline контрол) |
| **В реалната практика** | Използва се от разработчиците и QA инженерите всеки час | Използва се от Project Managers, QA Leads и CTO за управление на срокове и капацитет |


---
[⬅️ 01-07. API (REST & GraphQL) & Performance Testing](01-07-API-And-Performance-Testing.md) | 📂 Папка: 01-QA-Automation | [🏠 Начало](../README.md) | [➡️ 01-96. QA Performance & Optimizations](01-96-Performance-And-Optimizations.md)
