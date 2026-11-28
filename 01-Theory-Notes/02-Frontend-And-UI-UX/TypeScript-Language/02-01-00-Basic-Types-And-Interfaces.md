[⬅️ 02-00. Intro to TypeScript & Compiler (tsc)](02-00-Intro-To-TypeScript-And-Compiler.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-01-01. Syntax-From-Scratch & Types Cheat Sheet](02-01-01-Syntax-Basic-Types-And-Interfaces.md)

---

# 02-01-00. Basic Types, Interfaces & Type Aliases (Теория, Safety & Капани)

**Раздел 01:** TypeScript Core & Type System | **Цел:** Овладяване на примитивните и специалните типове в TypeScript (`string`, `number`, `boolean`, `unknown`, `any`, `void`, `never`), автоматичното извеждане на типове (**Type Inference**), разликите между **`interface`** и **`type`**, и предотвратяване на тихи Runtime сривове.

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: unknown vs any, void vs never)](#1-как-да-се-запомни-по-лесно-мнемоника-unknown-vs-any-void-vs-never)
* [2. Архитектура на типовата безопасност и Блок-схема (JS срещу TS)](#2-архитектура-на-типовата-безопасност-и-блок-схема-js-срещу-ts)
* [3. Сравнителни матрици: interface срещу type & unknown срещу any срещу never](#3-сравнителни-матрици-interface-срещу-type--unknown-срещу-any-срещу-never)
* [4. Плюсове и минуси на строгата типова система](#4-плюсове-и-минуси-на-строгата-типова-система)
* [5. Кодов одит: Грешен (Суров JS / any) срещу Правилен (Строг TypeScript) код](#5-кодов-одит-грешен-суров-js--any-срещу-правилен-строг-typescript-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: UNKNOWN VS ANY, VOID VS NEVER)

* **`any` срещу `unknown` (Отключена врата срещу Сейф с проверка):**
  * **`any`** е като **изключване на алармата** — позволява ти да правиш всичко с променливата без проверка, но връща опасността от Runtime грешки на JavaScript.
  * **`unknown`** е като **заключена кутия** — можеш да сложиш вътре каквото искаш (напр. отговор от `JSON.parse()`), но TypeScript **не ти дава да я отвориш или извикаш метод върху нея**, докато не провериш типа ѝ (`typeof` / `instanceof`).
* **`void` срещу `never` (Празна ръка срещу Безкрайна пропаст):**
  * **`void`:** Функцията завършва нормално и връща управлението, но не връща конкретна стойност (аналог на `void` в C# и Java).
  * **`never`:** Функцията **никога не стига до своя край** — или винаги хвърля грешка (`throw new Error(...)`), или върти безкраен цикъл (`while (true)`).
* **Type Inference (Интелигентно отгатване):** Не е нужно да пишеш `let age: number = 25;` навсякъде. Когато инициализираш променлива директно със стойност (`let age = 25`), TypeScript автоматично заключва типа `number`. Пиши изрични типове върху **параметрите на функции, връщаните стойности и празните масиви** (`const items: string[] = []`).

---

## 2. АРХИТЕКТУРА НА ТИПОВАТА БЕЗОПАСНОСТ И БЛОК-СХЕМА (JS СРЕЩУ TS)

```text
 JAVASCRIPT ПОТОК (Динамичен — рисков):
 [ Писане на .js код ] ──► [ Браузър / Node.js ] ──► 💥 Грешката гърми пред клиента (Runtime)

 TYPESCRIPT ПОТОК (Статичен — защитен):
 [ Писане на .ts код ] ──► [ Компилатор (tsc) ] ──► ✅ Спира грешката в редактора (Compile-time)
                                  │
                                  ▼ (Type Erasure / Транспилация)
                         [ Чист .js код ] ──► [ Браузър / Node.js ]
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: INTERFACE СРЕЩУ TYPE & UNKNOWN СРЕЩУ ANY СРЕЩУ NEVER

### 📊 А. `interface` срещу `type` (Type Alias)

| Критерий | `interface` | `type` (Type Alias) |
| :--- | :--- | :--- |
| **Основно предназначение** | Описване на формата на **обекти и класове** (договори). | Име за **всякакъв тип** — примитиви, обекти, Unions (`\|`), Tuples (`[a, b]`). |
| **Разширяване (Наследяване)** | Чрез ключова дума **`extends`** (`interface IAdmin extends IUser`). | Чрез оператор **Intersection (`&`)** (`type Admin = User & { role: string }`). |
| **Declaration Merging** | ✅ **Да** — два интерфейса с едно и също име се сливат автоматично. | ❌ **Не** — дублирането на име хвърля компилационна грешка. |
| **Имплементация от клас** | `class UserRepo implements IRepo` (стандартен стил). | `class UserRepo implements RepoType` (работи, ако е обектен тип). |
| **Инженерна препоръка** | Използвай за публични API модели, DTO-та и ООП договори. | Използвай за Unions, Tuples, функции и Utility трансформации. |

### 📊 Б. Сравнение на специалните типове: `unknown` срещу `any` срещу `never`

| Тип | Какви стойности приема? | Позволява ли директен достъп до методи (`x.toUpperCase()`)? | Кога се използва? |
| :--- | :--- | :--- | :--- |
| **`unknown`** | Всякакви стойности. | ❌ **Не** — изисква задължителен Type Guard (`typeof`). | Външни API данни, `catch (err: unknown)`, `JSON.parse()`. |
| **`any`** | Всякакви стойности. | ⚠️ **Да** — изключва напълно проверката на компилатора! | Само при временна миграция от стар JavaScript код. |
| **`never`** | **Никакви** (празно множество). | ❌ Недостижим код. | Функции, хвърлящи `throw new Error()`, и изчерпателни `switch` проверки. |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА СТРОГАТА ТИПОВА СИСТЕМА

### 🟢 Предимства (Pros):
* **Ранно улавяне на бъгове (Compile-time Safety):** Хваща ~15–20% от най-честите софтуерни грешки (напр. `user.emial` вместо `user.email` или подаване на стринг вместо число) още по време на писане във VS Code.
* **Самодокументиращ се код (Self-Documenting Code):** Интерфейсите служат като точен договор между фронтенд и бекенд екипите без нужда от отделна документация.

### 🔴 Недостатъци (Cons):
* **Повече начален код (Boilerplate):** Писането на интерфейси и типови анотации отнема малко повече време при много малки прототипи.
* **Липса на Runtime валидация:** Тъй като типовете изчезват при компилация (**Type Erasure**), ако сървърът върне счупен JSON в продукция, TypeScript няма да хвърли грешка сам без библиотека като `Zod`.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (СУРОВ JS / ANY) СРЕЩУ ПРАВИЛЕН (СТРОГ TYPESCRIPT) КОД

### ❌ ГРЕШЕН КОД (Тиха катастрофа във финансовите сметки с JavaScript / `any`):
```typescript
// 🚨 ГРЕШКА 1: Липса на интерфейс — ако от API дойде 'cost' вместо 'price', JS връща NaN!
function applyDiscount(product: any) {
    return product.price - (product.price * product.discount);
}

const item = { name: "Клавиатура", cost: 100, discount: 0.1 };
console.log(applyDiscount(item)); // Изход: NaN (Тиха логическа грешка!)
```

### 🟢 ПРАВИЛЕН КОД (Строг интерфейс IProduct и защита на типовете):
```typescript
interface IProduct {
    readonly id: number;
    name: string;
    price: number;
    discount?: number; // Опционално свойство
}

function applyDiscountSecure(product: IProduct): number {
    const discountRate = product.discount ?? 0; // Nullish coalescing (ако е undefined -> 0)
    return product.price - (product.price * discountRate);
}

const validItem: IProduct = { id: 1, name: "Клавиатура", price: 100, discount: 0.1 };
console.log(`Крайна цена: ${applyDiscountSecure(validItem)} лв.`); // Изход: Крайна цена: 90 лв.

// Ако се опиташ да подадеш обект с 'cost' вместо 'price':
// const brokenItem: IProduct = { id: 2, name: "Мишка", cost: 50 };
// 🛑 VS Code веднага блокира компилацията: 'cost' does not exist in type 'IProduct'!
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Защо масовото използване на типа `any` се счита за тежък анти-патърн и защо `unknown` е правилният избор?“*
  * **Отговор:** Типът `any` изключва напълно проверката на компилатора и позволява извикването на несъществуващи методи върху променливата, което води до Runtime сривове. Типът `unknown` също приема всякакви стойности, но **забранява всяка операция върху тях**, докато програмистът не докаже точния им тип чрез `typeof`, `instanceof` или потребителски Type Guard.
* **Въпрос 2:** *„Каква е разликата между `null` и `undefined` в TypeScript при включен флаг `strictNullChecks: true`?“*
  * **Отговор:** `undefined` означава, че променливата е декларирана или свойството липсва в обекта, но **още не е получила стойност**. `null` е **умишлено присвоена празна стойност** от програмиста. При `strictNullChecks: true` нито `null`, нито `undefined` могат да бъдат присвоени на стандартен тип като `string` или `number`, освен ако не го разрешиш изрично чрез Union (`string | null` или опционално поле `name?: string`).

---

[⬅️ 02-00. Intro to TypeScript & Compiler (tsc)](02-00-Intro-To-TypeScript-And-Compiler.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-01-01. Syntax-From-Scratch & Types Cheat Sheet](02-01-01-Syntax-Basic-Types-And-Interfaces.md)

© 2026 Dev-Library / 02-01-00. Basic Types, Interfaces & Type Aliases (Теория, Safety & Капани)
