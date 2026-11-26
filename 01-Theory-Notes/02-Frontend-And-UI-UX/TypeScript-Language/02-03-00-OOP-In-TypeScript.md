[⬅️ 02-02-01. Advanced Types & Utility Cheat Sheet](02-02-01-Syntax-From-Scratch.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-03-01. OOP in TypeScript Cheat Sheet](02-03-01-Syntax-From-Scratch.md)

---

# 02-03-00. Обектно-Ориентирано Програмиране (OOP) в TypeScript (Дълбока Архитектура & SOLID)

**Раздел 02:** Object-Oriented TypeScript & Generics | **Цел:** Дълбоко овладяване на 4-те стълба на ООП в TypeScript (**Енкапсулация, Наследяване, Полиморфизъм и Абстракция**), разликата между TS `private` и ECMAScript `#private`, **SOLID принципите**, **Structural срещу Nominal Typing**, капаните със загуба на контекста **`this`** при подаване на методи като callbacks и **Type Erasure** при интерфейсите.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: 4-те стълба на ООП, TS private vs JS # & Duck Typing)](#1-как-да-се-запомни-по-лесно-мнемоника-4-те-стълба-на-ооп-ts-private-vs-js--duck-typing)
* [2. Архитектура на ООП йерархията, Прототипите и Type Erasure (Блок-схема)](#2-архитектура-на-ооп-йерархията-прототипите-и-type-erasure-блок-схема)
* [3. Сравнителни матрици: Abstract Class срещу Interface & Модификатори за достъп](#3-сравнителни-матрици-abstract-class-срещу-interface--модификатори-за-достъп)
* [4. SOLID принципите в TypeScript и Плюсове/Минуси на ООП](#4-solid-принципите-в-typescript-и-плюсовеминуси-на-ооп)
* [5. Кодов одит: Грешен (Счупена капсулация & Загубен `this`) срещу Правилен ООП дизайн](#5-кодов-одит-грешен-счупена-капсулация--загубен-this-срещу-правилен-ооп-дизайн)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: 4-ТЕ СТЪЛБА НА ООП, TS PRIVATE VS JS # & DUCK TYPING)

* **Четирите стълба на ООП в TypeScript:**
  1. **Енкапсулация (Капсулиране):** Скриваш вътрешното състояние (`#balance` / `private`) и позволяваш промяна само през валидирани методи или `set` аксесори.
  2. **Абстракция:** Дефинираш *КАКВО* трябва да прави даден компонент чрез `interface` или `abstract class`, без да разкриваш *КАК* го прави отвътре.
  3. **Наследяване (`extends`):** Дъщерният клас получава свойствата и методите на родителя и надгражда върху тях чрез `super()` и `override`.
  4. **Полиморфизъм (Многоформие):** Различни класове (`CreditCardPayment`, `PayPalPayment`) имплементират един и същ интерфейс `IPaymentMethod` и се обработват еднакво през обща променлива от тип `IPaymentMethod`.
* **TS `private` срещу JS `#private` (Хартиена ограда срещу Бетонна стена):**
  * Ключовата дума `private` на TypeScript е **хартиена ограда (Soft Privacy)** — пази те само в редактора (Compile-time). След компилация до JavaScript тя изчезва и в браузъра свойството е напълно видимо и достъпно!
  * Символът диез `#password` е **бетонна стена (Hard Privacy)** от стандарта ECMAScript — V8 двигателят реално заключва полето в паметта в Runtime и хвърля `SyntaxError` при опит за външен достъп.
* **Parameter Properties (Бърз конструктор в стил C# Primary Constructors):** Когато поставиш модификатор за достъп директно пред параметър в конструктора (`constructor(public readonly id: number)`), TypeScript автоматично създава полето в класа и му присвоява стойността без повторение на `this.id = id;`.

---

## 2. АРХИТЕКТУРА НА ООП ЙЕРАРХИЯТА, ПРОТОТИПИТЕ И TYPE ERASURE (БЛОК-СХЕМА)

Под капака JavaScript няма класически класове като C# и Java — `class` в TypeScript/ES6 е елегантен синтаксис върху **прототипното наследяване (Prototype Chain)**, докато интерфейсите съществуват само по време на компилация:

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  COMPILE-TIME (TypeScript Анализатор)                                   │
 │  • interface IEvaluator { calculate(): string; }  ──► Изтрива се!       │
 │  • abstract class TeamMember { ... }              ──► Става JS клас     │
 │  • class SeniorStudent extends TeamMember implements IEvaluator         │
 └────────────────────────────────────┬────────────────────────────────────┘
                                      │ tsc (Type Erasure)
                                      ▼
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  RUNTIME (Чист ES2022 JavaScript — Prototype Chain в паметта)           │
 │  [candidate: SeniorStudent]                                             │
 │     ├── собствени полета: id, name, scoreMetrics, #secretApiKey         │
 │     └── [[Prototype]] ──► SeniorStudent.prototype (calculateFinalStatus)│
 │                             └── [[Prototype]] ──► TeamMember.prototype  │
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: ABSTRACT CLASS СРЕЩУ INTERFACE & МОДИФИКАТОРИ ЗА ДОСТЪП

### 📊 А. `abstract class` срещу `interface` (Кога кое да избера?)

| Критерий | `abstract class` | `interface` |
| :--- | :--- | :--- |
| **Наличие в Runtime (JS)** | ✅ **Да** — компилира се до реален JavaScript клас (`instanceof` работи!). | ❌ **Не** — изтрива се напълно при компилация (Type Erasure). |
| **Готова имплементация на методи** | ✅ **Да** — може да съдържа готови методи + `abstract` методи без тяло. | ❌ **Не** — съдържа само сигнатури (договор без код). |
| **Конструктор и полета с данни** | ✅ **Да** — може да има `constructor`, `protected` и `#private` полета. | ❌ **Не** — няма конструктор и не пази състояние. |
| **Множествено наследяване** | ❌ **Не** — един клас може да наследи само **1** базов клас (`extends`). | ✅ **Да** — един клас може да имплементира много интерфейси (`implements A, B, C`). |
| **Инженерно правило** | Използвай за споделена базова логика и общо състояние между близки класове. | Използвай за разкачване на зависимости (Dependency Inversion / SOLID). |

### 📊 Б. Модификатори за достъп и защита на данните

| Модификатор | Достъп от самия клас | Достъп от наследници (`extends`) | Достъп отвън | Защита в Runtime (JS)? |
| :--- | :---: | :---: | :---: | :---: |
| **`public`** *(по подразбиране)* | ✅ Да | ✅ Да | ✅ Да | Публично поле |
| **`protected`** | ✅ Да | ✅ Да | ❌ Не | Изтрива се в Runtime |
| **`private`** *(TS keyword)* | ✅ Да | ❌ Не | ❌ Не | ❌ **Не** (видимо в JS) |
| **`#field`** *(ES Private)* | ✅ Да | ❌ Не | ❌ Не | ✅ **Да** (заключено в V8) |
| **`readonly`** | Само в конструктора | ❌ Не (за запис) | Само за четене | Изтрива се в Runtime |

---

## 4. SOLID ПРИНЦИПИТЕ В TYPESCRIPT И ПЛЮСОВЕ/МИНУСИ НА ООП

* **S — Single Responsibility Principle (SRP):** Класът `InvoiceCalculator` смята само суми и данъци; класът `InvoiceRepository` записва в базата данни; класът `InvoicePrinter` генерира PDF.
* **O — Open/Closed Principle (OCP):** Отворен за разширение, затворен за промяна — добавяш нов метод за плащане (`CryptoPayment implements IPayment`), без да пипаш `if/else` вериги в `CheckoutService`.
* **L — Liskov Substitution Principle (LSP):** Всеки наследник (`FileLogger extends BaseLogger`) трябва да може да замени родителя си, без да чупи очакванията на програмата или да хвърля неочаквани изключения.
* **I — Interface Segregation Principle (ISP):** Вместо един гигантски интерфейс `IWorker` с 15 метода, раздели го на малки фокусирани договори (`ICoder`, `ITester`, `IDeployer`).
* **D — Dependency Inversion Principle (DIP):** Класовете от високо ниво (`OrderService`) зависят от абстракция (`interface IOrderRepository`), а не от конкретен клас (`PostgresOrderRepository`), което прави Unit тестването с мокове тривиално.

### 🟢 Предимства и 🔴 Недостатъци на ООП в TypeScript:
* **🟢 Плюсове:** Отлична организация на големи корпоративни проекти (Angular, NestJS), лесно прилагане на Design Patterns (Factory, Strategy, Singleton, Observer) и пълна компилационна безопасност.
* **🔴 Минуси:** Дълбоките йерархии от наследяване (`A extends B extends C extends D`) правят кода твърд и труден за промяна — затова модерният TypeScript предпочита **Композиция пред Наследяване (Composition over Inheritance)**.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (СЧУПЕНА КАПСУЛАЦИЯ & ЗАГУБЕН `THIS`) СРЕЩУ ПРАВИЛЕН ООП ДИЗАЙН

### ❌ ГРЕШЕН КОД (Липсваща имплементация, фалшив `private` и загубен контекст `this` при Callback):
```typescript
interface IRepository {
    saveData(): void;
}

// 🚨 ГРЕШКА 1: Класът не имплементира метода saveData() от интерфейса!
class BadStudentRepository implements IRepository {
    // 🚨 ГРЕШКА 2: 'private' изчезва в JS — за истинска секретност ползвай '#connectionString'!
    private connectionString: string = "Server=localhost;Password=Secret123;";
    public tableName: string = "Students";

    public printTable() {
        console.log(this.tableName);
    }
}

// 🚨 ГРЕШКА 3: Подаване на обикновен метод като callback губи контекста 'this' -> TypeError в Runtime!
// setTimeout(repo.printTable, 1000); // 'this' става undefined!
```

### 🟢 ПРАВИЛЕН КОД (SOLID Dependency Inversion, Абстрактен клас, Полиморфизъм & Защитен `this`):
```typescript
// 1. Договори (Interfaces) за Полиморфизъм и Dependency Inversion
interface IEvaluator {
    calculateFinalStatus(): string;
}

interface INotifier {
    notify(message: string): void;
}

// 2. Абстрактен базов клас за споделено състояние и капсулация
abstract class TeamMember {
    constructor(
        public readonly id: number,
        protected readonly name: string
    ) {}

    abstract printCredentials(): void;
}

// 3. Конкретен клас с истинско частно поле (#), override и Arrow метод (запазва 'this')
class SeniorStudent extends TeamMember implements IEvaluator {
    #secretApiKey: string = "KEY_9981";
    private scoreMetrics: number[];

    constructor(id: number, name: string, scores: number[]) {
        super(id, name);
// 3. Конкретен клас с истинско частно поле (#), override и Arrow метод (запазва 'this')
class SeniorStudent extends TeamMember implements IEvaluator {
    #secretApiKey: string = "KEY_9981";
    private scoreMetrics: number[];

    constructor(id: number, name: string, scores: number[]) {
        super(id, name);
        this.scoreMetrics = scores;
    }

    public override printCredentials(): void {
        console.log(`[Инженерна Справка] ID: ${this.id} | Курсист: ${this.name}`);
    }

    // Arrow функция в класа — автоматично заключва 'this' към инстанцията!
    public logStatusAsync = (): void => {
        console.log(`[Async Callback] Статус на ${this.name}: ${this.calculateFinalStatus()}`);
    };

    public calculateFinalStatus(): string {
        const sum = this.scoreMetrics.reduce((acc, curr) => acc + curr, 0);
        const average = sum / this.scoreMetrics.length;

        return average >= 80
            ? "Успешно преминаване към Ниво 2."
            : "Необходима е допълнителна рефакторизация.";
    }
}

// 4. Композиция и Dependency Injection (Сервизът зависи от интерфейс, не от конкретен клас)
class EvaluationService {
    constructor(private readonly notifier: INotifier) {}

    public processCandidate(candidate: IEvaluator): void {
        const decision = candidate.calculateFinalStatus();
        this.notifier.notify(`Краен Резултат: ${decision}`);
    }
}

const candidate = new SeniorStudent(105, "Елизабет", [85, 90, 78]);
candidate.printCredentials();

const service = new EvaluationService({ notify: (msg) => console.log(msg) });
service.processCandidate(candidate);
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Защо следният код е напълно валиден и не хвърля грешка при компилация в TypeScript?“*
  ```typescript
  class DbUser { name!: string; }
  class WebProduct { name!: string; }
  const entity: DbUser = new WebProduct();
  ```
  * **Отговор:** Защото TypeScript използва **Structural Typing (Duck Typing)** вместо Nominal Typing. И двата класа притежават напълно идентична структура (едно поле `name: string`), затова компилаторът ги третира като 100% съвместими типове, независимо от различните им имена.
* **Въпрос 2:** *„Каква е разликата между стандартния метод `myMethod() {}` и стрелковата функция `myMethod = () => {}` вътре в TypeScript клас?“*
  * **Отговор:** Стандартният метод се записва **веднъж върху `Class.prototype`** (пести RAM при хиляди инстанции), но губи референцията `this`, ако бъде подаден директно като callback (напр. `button.addEventListener('click', obj.myMethod)`). Стрелковата функция създава **отделно копие в паметта за всяка инстанция** на класа, но лексикално заключва `this` към обекта завинаги.
* **Въпрос 3:** *„Какво прави удивителният знак `!` след име на поле в клас (`private _name!: string;`) и кога се използва?“*
  * **Отговор:** Това е **Definite Assignment Assertion**. При включен строг режим (`strictPropertyInitialization: true`), TypeScript изисква всяко поле да получи начална стойност в конструктора. Знакът `!` казва на компилатора: *„Гарантирам, че това поле ще бъде инициализирано чрез помощен метод или Dependency Injection преди да се прочете“*.

---

[⬅️ 02-02-01. Advanced Types & Utility Cheat Sheet](02-02-01-Syntax-From-Scratch.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-03-01. OOP in TypeScript Cheat Sheet](02-03-01-Syntax-From-Scratch.md)

© 2026 Dev-Library / 02-03-00. Обектно-Ориентирано Програмиране (OOP) в TypeScript
