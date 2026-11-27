[⬅️ 02-03-01. OOP in TypeScript Cheat Sheet](02-03-01-OOP-In-TypeScript-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-04-01. Generics & Conditional Types Cheat Sheet](02-04-01-Mastering-Generics-Syntax.md)

---

# 02-04-00. Овладяване на Дженерици (Generics, Constraints & Conditional Types)

**Раздел 02:** Object-Oriented TypeScript & Generics | **Цел:** Овладяване на параметризираните типове (**Generics `<T>`**) във функции, интерфейси и класове, налагане на типови ограничения (**Generic Constraints чрез `extends` и `keyof`**), условни типове (**Conditional Types**) и филтриране на ключове чрез **`never`**.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Типов Шаблон `<T>`, Constraints `extends` & `keyof`)](#1-как-да-се-запомни-по-лесно-мнемоника-типов-шаблон-t-constraints-extends--keyof)
* [2. Архитектура на Generics и Блок-схема (Един код ➔ Много типове)](#2-архитектура-на-generics-и-блок-схема-един-код--много-типове)
* [3. Сравнителни матрици: Generics срещу `any` срещу Function Overloads & Паралел с C#](#3-сравнителни-матрици-generics-срещу-any-срещу-function-overloads--паралел-с-c)
* [4. Плюсове и минуси на Generics и Conditional Types](#4-плюсове-и-минуси-на-generics-и-conditional-types)
* [5. Кодов одит: Грешен (Липсващ Constraint) срещу Правилен (Защитен Generic склад) код](#5-кодов-одит-грешен-липсващ-constraint-срещу-правилен-защитен-generic-склад-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: ТИПОВ ШАБЛОН `<T>`, CONSTRAINTS `EXTENDS` & `KEYOF`)

* **Generic `<T>` (Кутия с прозрачен етикет):**
  * Ако направиш кутия от тип `any`, слагаш вътре лаптоп, но когато го извадиш, компилаторът е забравил какво има вътре и не ти дава подсказки (IntelliSense).
  * Ако направиш кутия от тип `Box<T>`, слагаш вътре `Laptop` и компилаторът автоматично залепва етикет `Box<Laptop>` — когато извадиш елемента, имаш **100% типова безопасност и автодопълнение**!
* **Generic Constraint (`<T extends ILengthwise>` — Филтър на входа):** По подразбиране `<T>` може да бъде *всичко* (дори `number` или `boolean`). Когато напишеш `T extends ILengthwise`, казваш на компилатора: *„Приемам всеки тип `T`, но само при условие, че има поне свойството `length: number`“* (аналог на `where T : ILengthwise` в C#).
* **`<T, K extends keyof T>` (Заключване на ключ към обект):** Гарантира, че вторият параметър `key` може да бъде **единствено реално съществуващо име на поле** от обекта `obj` (предотвратява грешки от типа `getProperty(user, "emial")`).

---

## 2. АРХИТЕКТУРА НА GENERICS И БЛОК-СХЕМА (ЕДИН КОД ➔ МНОГО ТИПОВЕ)

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  ДЕФИНИЦИЯ НА GENERIC КОМПОНЕНТ (Compile-Time Шаблон)                   │
 │  class DataRepositoryHub<T> { push(item: T): void; get(i: number): T; } │
 └────────────────────────────────────┬────────────────────────────────────┘
                                      │
                 ┌────────────────────┴────────────────────┐
                 ▼                                         ▼
 ┌───────────────────────────────┐         ┌───────────────────────────────┐
 │  Инстанция 1:                 │         │  Инстанция 2:                 │
 │  new DataRepositoryHub<User>()│         │  new DataRepositoryHub<Order>()│
 │  • .get(0) връща точно User!  │         │  • .get(0) връща точно Order! │
 └───────────────────────────────┘         └───────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: GENERICS СРЕЩУ `ANY` СРЕЩУ OVERLOADS & ПАРАЛЕЛ С C#

### 📊 А. Generics срещу `any` срещу Function Overloads

| Критерий | Generics (`<T>`) | Тип `any` | Function Overloads |
| :--- | :--- | :--- | :--- |
| **Запазва ли типа на изхода?** | ✅ **Да** — изходът знае точния тип на входа. | ❌ **Не** — връща `any` и губи IntelliSense. | ✅ **Да** — но за фиксиран списък от типове. |
| **Брой редове код** | **Минимален** (1 обща имплементация). | Минимален (но опасен). | **Висок** (пишеш отделна сигнатура за всеки тип). |
| **Работи с бъдещи нови типове?** | ✅ **Да** — без никаква промяна по кода. | Да (без проверка). | ❌ **Не** — трябва да добавяш нови Overloads. |

### 📊 Б. Синтактичен паралел: TypeScript Generics срещу C# (.NET) Generics

| Концепция | 🔷 TypeScript 5.x | 🟣 C# 12+ (.NET) |
| :--- | :--- | :--- |
| **Генерична функция** | `function identity<T>(arg: T): T` | `T Identity<T>(T arg)` |
| **Ограничение (Constraint)** | `<T extends IBaseEntity>` | `where T : IBaseEntity` |
| **Стойност по подразбиране** | `<T = string>` | *(Не се поддържа директно)* |
| **Създаване на нова инстанция** | `function create<T>(ctor: new () => T): T` | `where T : new()` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА GENERICS И CONDITIONAL TYPES

### 🟢 Предимства (Pros):
* **Максимална преизползваемост (DRY):** Един клас като `ApiResponse<T>` или `DataRepositoryHub<T>` обслужва стотици различни бизнес модели със 100% проверка на типовете.
* **Мощни типови трансформации:** Чрез Conditional Types (`T extends U ? X : Y`) и `infer` библиотеките (като Zod, Prisma, tRPC) автоматично изчисляват типовете на сложни API заявки.

### 🔴 Недостатъци (Cons):
* **Трудна четимост при прекаляване:** Вложени генерици с 3–4 параметъра (`<T, K extends keyof T, V = T[K]>`) и сложни условни типове могат да направят грешките на компилатора трудни за разчитане.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (ЛИПСВАЩ CONSTRAINT) СРЕЩУ ПРАВИЛЕН (ЗАЩИТЕН GENERIC СКЛАД) КОД

### ❌ ГРЕШЕН КОД (Извикване на свойство върху неограничен тип `T`):
```typescript
function getLengthProperty<T>(input: T): number {
    // 🚨 КРАХ: Компилаторът хвърля грешка на секундата!
    // Той не знае какъв е типът 'T' и дали притежава свойството .length.
    return input.length;
}
```

### 🟢 ПРАВИЛЕН КОД (Generic Constraint чрез `extends` и типизиран склад `DataRepositoryHub<T>`):
```typescript
interface ILengthwise {
    length: number;
}

// 1. Дженерик функция с изрично ограничение чрез 'extends'
function getLengthSecure<T extends ILengthwise>(input: T): number {
    return input.length; // Напълно защитено!
}

// 2. Типизиран Generic склад за данни в паметта
class DataRepositoryHub<T> {
    private entityStore: T[] = [];

    public pushToMemory(item: T): void {
        this.entityStore.push(item);
    }

    public extractFromMemory(index: number): T {
        if (index < 0 || index >= this.entityStore.length) {
            throw new Error("Софтуерен срив: Индексът е извън обхвата на RAM масива.");
        }
        return this.entityStore[index];
    }
}

type StudentModuleRecord = { name: string; score: number };
const repo = new DataRepositoryHub<StudentModuleRecord>();

repo.pushToMemory({ name: "Елизабет", score: 95 });
repo.pushToMemory({ name: "Георги", score: 88 });

const evaluatedCandidate = repo.extractFromMemory(0);
console.log(`Курсист: ${evaluatedCandidate.name} | Успех: ${evaluatedCandidate.score}%`);
console.log(`Дължина на низа: ${getLengthSecure("Успешен тест на дженерик филтър")} символа.`);
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Защо в TypeScript не можем да напишем `return new T();` директно вътре в генерична функция, както в C# с `where T : new()`?“*
  * **Отговор:** Заради **Type Erasure**! В Runtime `T` не съществува като обект или конструктор, затова JavaScript не знае кой клас да инстанцира. За да създадеш нова инстанция вътре в генерична функция в TypeScript, трябва да подадеш самия конструктор като аргумент: `function createInstance<T>(ctor: new (...args: any[]) => T): T { return new ctor(); }`.
* **Въпрос 2:** *„Как работи филтрирането на ключове чрез `never` в Mapped & Conditional Types?“*
  * **Отговор:** Когато обхождаме ключовете на тип `[K in keyof T]: T[K] extends number ? K : never` и след това индексираме с `[keyof T]`, TypeScript автоматично премахва всички клонове, които са оценени на `never` (тъй като `never` е празното множество в обединенията — `"age" | never | "salary"` се опростява до `"age" | "salary"`).

---

[⬅️ 02-03-01. OOP in TypeScript Cheat Sheet](02-03-01-OOP-In-TypeScript-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-04-01. Generics & Conditional Types Cheat Sheet](02-04-01-Mastering-Generics-Syntax.md)

© 2026 Dev-Library / 02-04-00. Овладяване на Дженерици (Generics, Constraints & Conditional Types)
