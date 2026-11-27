[⬅️ Назад към TypeScript Хъба](README.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-01-00. Basic Types, Interfaces & Type Aliases](02-01-00-Basic-Types-And-Interfaces.md)

---

# 02-00. Intro to TypeScript, Compiler (tsc) & Architecture

**Сектор:** TypeScript Language | **Цел:** Разбиране на архитектурата на **TypeScript** като строго-типизирано надмножество (Superset) на JavaScript, жизнения цикъл на компилацията (`tsc` ➔ **Type Erasure** ➔ чист ES2022+ JavaScript), разликата между **Structural Typing** (TypeScript) и **Nominal Typing** (C# / Java), ключовите настройки в `tsconfig.json` и пътната карта на темите.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Superset, Type Erasure & Duck Typing)](#1-как-да-се-запомни-по-лесно-мнемоника-superset-type-erasure--duck-typing)
* [2. Архитектура на компилатора tsc и Блок-схема (Build Time срещу Runtime)](#2-архитектура-на-компилатора-tsc-и-блок-схема-build-time-срещу-runtime)
* [3. Сравнителна матрица: TypeScript 5.x срещу JavaScript (ES6+) и C# (.NET)](#3-сравнителна-матрица-typescript-5x-срещу-javascript-es6-и-c-net)
* [4. Плюсове, минуси и критичните настройки в tsconfig.json](#4-плюсове-минус-и-критичните-настройки-в-tsconfigjson)
* [5. Пътна карта на обучението в TypeScript сектора](#5-пътна-карта-на-обучението-в-typescript-сектора)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: SUPERSET, TYPE ERASURE & DUCK TYPING)

* **Superset (Надмножество на JavaScript):** Всеки валиден `.js` файл е автоматично валиден `.ts` файл! TypeScript не заменя JavaScript, а слага **защитен архитектурен слой отгоре му** по време на писане и компилация.
* **Type Erasure (Изтриване на типовете при компилация):** Браузърите и Node.js **не могат да изпълняват TypeScript директно** (освен най-новите версии с вграден strip-types). Компилаторът `tsc` проверява всички типове и след това **изтрива напълно** всички `: string`, `interface` и `type` анотации, оставяйки чист JavaScript код с **0 байта и 0ms допълнителен товар в Runtime**!
* **Structural Typing (Duck Typing — *„Ако ходи като патица и квака като патица...“*):** За разлика от C# и Java, където два класа са съвместими само ако наследяват един и същ интерфейс по име (**Nominal Typing**), в TypeScript два обекта са съвместими, ако имат **еднаква форма (еднакви свойства и типове)**, независимо как се казват!

---

## 2. АРХИТЕКТУРА НА КОМПИЛАТОРА TSC И БЛОК-СХЕМА (BUILD TIME СРЕЩУ RUNTIME)

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  1. SOURCE CODE (Build / Compile Time)                                  │
 │     [index.ts / user.service.ts]  (Съдържа interface, type, generics)   │
 └────────────────────────────────────┬────────────────────────────────────┘
                                      │ tsc (TypeScript Compiler) / Vite
                                      ▼
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  2. TYPE CHECKER (Статичен анализ без изпълнение на кода)               │
 │     • Открива печатни грешки, null/undefined достъп и несъвместими      │
 │       типове ОЩЕ ВЪВ VS CODE преди да стартираш приложението!           │
 └────────────────────────────────────┬────────────────────────────────────┘
                                      │ Type Erasure (Изтрива типовете)
                                      ▼
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  3. EMITTED OUTPUT (Runtime в Браузър или Node.js)                      │
 │     [index.js] + [index.js.map] (Source Map за дебъгване) + [index.d.ts]│
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНА МАТРИЦА: TYPESCRIPT 5.X СРЕЩУ JAVASCRIPT (ES6+) И C# (.NET)

| Характеристика | 🔷 TypeScript 5.x | 🟡 JavaScript (ES6+) | 🟣 C# 12+ (.NET) |
| :--- | :--- | :--- | :--- |
| **Типова система** | Статична (по избор, с Type Inference) | Динамична (проверява се чак при срив) | Статична и строга (Compile-Time + Runtime) |
| **Модел на съвместимост** | **Structural Typing** (по форма на обекта) | Duck Typing (в Runtime) | **Nominal Typing** (по име на клас/интерфейс) |
| **Наличие на типове в Runtime** | ❌ **Не** (Type Erasure — изтриват се) | ❌ Няма | ✅ **Да** (Reflection, `typeof(T)`, `is`) |
| **Null безопасност** | `strictNullChecks: true` (`string \| null`) | Всичко може да е `undefined` / `null` | Nullable Reference Types (`string?`) |
| **Generics** | Да (`<T extends Base>`) | ❌ Не | Да (`where T : Base`) |
| **Къде се използва?** | Angular, React, Vue, Node.js, Playwright | Браузъри, малки скриптове | Бекенд API, Desktop, Unity, Cloud |

---

## 4. ПЛЮСОВЕ, МИНУСИ И КРИТИЧНИТЕ НАСТРОЙКИ В TSCONFIG.JSON

### 🟢 Предимства и 🔴 Недостатъци:
* **🟢 Плюсове:** Хваща ~15–20% от най-честите бъгове (`Cannot read properties of undefined`) още по време на писане във VS Code; осигурява перфектен IntelliSense и безопасен рефакторинг в големи екипи.
* **🔴 Минуси:** Изисква стъпка за компилация (Build Step); в Runtime **не валидира външни данни** от API или форми (за Runtime валидация се използват библиотеки като **Zod** или **Valibot**).

### ⚙️ Златният минимум в `tsconfig.json`:
```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "NodeNext",
    "strict": true,               // 🔒 Включва strictNullChecks, noImplicitAny и др.
    "noUncheckedIndexedAccess": true, // 🛡️ Предпазва при достъп до масив arr[99] -> T | undefined
    "sourceMap": true,            // 🐛 Позволява дебъгване на .ts файловете директно в Chrome/VS Code
    "skipLibCheck": true,
    "outDir": "./dist"
  },
  "include": ["src/**/*"]
}
```

---

## 5. ПЪТНА КАРТА НА ОБУЧЕНИЕТО В TYPESCRIPT СЕКТОРА

1. **Тема 02-01 (`-00` / `-01`):** Basic Types, Type Inference, `unknown` срещу `any` и `interface` срещу `type`.
2. **Тема 02-02 (`-00` / `-01`):** Advanced Types — Union (`|`), Intersection (`&`), Discriminated Unions, Type Guards и вградени Utility Types (`Partial`, `Pick`, `Omit`, `Record`, `Readonly`).
3. **Тема 02-03 (`-00` / `-01`):** OOP in TypeScript — класове, модификатори за достъп (`public`, `private`, `#private`, `protected`, `readonly`), абстрактни класове и имплементация на интерфейси.
4. **Тема 02-04 (`-00` / `-01`):** Mastering Generics — генерични функции, класове, ограничения (`extends`), `keyof` и Mapped Types.
5. **Тема 02-05 (`-00` / `-01`):** Decorators, ES Modules, `async/await` Promises и типизирана обработка на грешки.

---

[⬅️ Назад към TypeScript Хъба](README.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-01-00. Basic Types, Interfaces & Type Aliases](02-01-00-Basic-Types-And-Interfaces.md)

© 2026 Dev-Library / 02-00. Intro to TypeScript, Compiler (tsc) & Architecture
