[⬅️ 02-00. Intro to JavaScript & V8 Engine](02-00-Intro-To-JavaScript-And-V8-Engine.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-01-01 Variables, Types & Coercion — Syntax](02-01-00-Variables-Types-And-Coercion.md)

---

# 🟨 02-01-00. Variables, Data Types & Type Coercion (Теория и Капани)

## 📑 Съдържание
* [1. Деклариране на променливи: `var`, `let` и `const`](#1-деклариране-на-променливи-var-let-и-const)
* [2. Hoisting и Temporal Dead Zone (TDZ)](#2-hoisting-и-temporal-dead-zone-tdz)
* [3. Типовата система на JavaScript: 7 Примитива vs Обекти](#3-типовата-система-на-javascript-7-примитива-vs-обекти)
* [4. Stack срещу Heap: Стойност срещу Референция](#4-stack-срещу-heap-стойност-срещу-референция)
* [5. Type Coercion (Принудително преобразуване) и `==` срещу `===`](#5-type-coercion-принудително-преобразуване-и--срещу-)
* [6. Скрити капани и Edge Cases](#6-скрити-капани-и-edge-cases)

---

## 1. Деклариране на променливи: `var`, `let` и `const`

В съвременния JavaScript (ES6+) **никога не използваме `var`** в нов код. Изборът винаги е: **`const` по подразбиране** и **`let` само когато стойността ще се презаписва**.

| Характеристика | `var` (Legacy ES5) | `let` (Modern ES6+) | `const` (Modern ES6+) |
| :--- | :--- | :--- | :--- |
| **Обхват (Scope)** | Function-scoped (или Global) | Block-scoped (`{ ... }`) | Block-scoped (`{ ... }`) |
| **Hoisting** | Да (инициализира се като `undefined`) | Да (но влиза в **TDZ**) | Да (но влиза в **TDZ**) |
| **Повторна декларация** | Позволена в същия scope | ❌ `SyntaxError` | ❌ `SyntaxError` |
| **Презаписване (Reassignment)** | Позволено | Позволено | ❌ `TypeError` |
| **Закача се за `window`** | Да (в глобален scope) | Не | Не |

> **⚠️ Важно уточнение за `const`:** `const` **не прави стойността неизменна (immutable)**, а прави **връзката (референцията)** постоянна. Можете свободно да променяте свойствата на `const` обект или елементите на `const` масив (`user.name = "Ana"`, `arr.push(4)`), но не можете да презапишете самата променлива (`user = {}`). За пълна неизменност се използва `Object.freeze()`.

---

## 2. Hoisting и Temporal Dead Zone (TDZ)

Преди V8 енджинът да изпълни кода ред по ред (Execution Phase), той преминава през **Creation Phase (Memory Allocation)**, в която сканира всички декларации в текущия обхват:

* **`function` декларациите** се качват изцяло в паметта (могат да се извикат преди реда на дефиниция).
* **`var` променливите** се регистрират и веднага получават стойност `undefined`.
* **`let` и `const` променливите** също се регистрират в паметта, но остават **неинициализирани** в т.нар. **Temporal Dead Zone (TDZ)** от началото на блока `{` до реда на тяхната декларация. Опит за достъп в TDZ хвърля `ReferenceError: Cannot access '...' before initialization`.

```text
{  <── Начало на блока (Влизане в TDZ за 'age')
   │
   │   console.log(age);  // ❌ ReferenceError (TDZ!)
   │
   ├── const age = 25;    // <── Край на TDZ (Променливата е инициализирана)
   │
   │   console.log(age);  // ✅ 25
}
```

---

## 3. Типовата система на JavaScript: 7 Примитива vs Обекти

JavaScript е **динамично типизиран** език — типът е закачен за **стойността**, а не за променливата.

### Седемте Примитивни типа (Immutable / Неизменни):
1. **`number`**: Всички цели и дробни числа (64-битови IEEE 754 числа с плаваща запетая), плюс специалните стойности `Infinity`, `-Infinity` и `NaN` (*Not-a-Number*).
2. **`bigint`**: Цели числа с произволна дължина (`9007199254740991n`), за числа над `Number.MAX_SAFE_INTEGER` (2⁵³ - 1).
3. **`string`**: Неизменни текстови низове (UTF-16).
4. **`boolean`**: Логически стойности `true` или `false`.
5. **`undefined`**: Декларирана променлива, на която все още не е присвоена стойност (от системата).
6. **`null`**: Експлицитно зададена от програмиста „празна“ или липсваща стойност.
7. **`symbol`**: Уникален и неизменен идентификатор, използван за ключове на обекти без риск от колизия.

### Референтни типове (`object`):
Всичко останало е обект: обикновени обекти (`{}`), масиви (`[]`), функции (`function`), дати (`Date`), регулярни изрази (`RegExp`), `Map`, `Set`.

---

## 4. Stack срещу Heap: Стойност срещу Референция

```text
CALL STACK (Бърза памет)               MEMORY HEAP (Динамична памет)
┌───────────────────────────┐          ┌──────────────────────────────┐
│ let a = 10;               │          │                              │
│ let b = a; // Копие: 10   │          │  0x001F: { name: "Elena" }   │
├───────────────────────────┤          │               ▲              │
│ const u1 = 0x001F ────────┼──────────┼───────────────┘              │
│ const u2 = 0x001F ────────┼──────────┘ (Двете сочат към 1 обект!)   │
└───────────────────────────┘          └──────────────────────────────┘
```

* **Примитивите** се копират **по стойност (By Value)** — промяната на `b` не влияе на `a`.
* **Обектите** се копират **по референция (By Reference)** — `u1` и `u2` пазят един и същ адрес в Heap паметта. Промяната през `u2.name = "Ivan"` се вижда и през `u1.name`.

---

## 5. Type Coercion (Принудително преобразуване) и `==` срещу `===`

* **Loose Equality (`==`):** Извършва **имплицитно преобразуване на типовете (Type Coercion)** преди сравнението, което води до неочаквани резултати (`"5" == 5` ➔ `true`, `"" == 0` ➔ `true`, `null == undefined` ➔ `true`).
* **Strict Equality (`===`):** Проверява **едновременно стойността И типа** без никакво преобразуване (`"5" === 5` ➔ `false`). **Винаги използвайте `===` и `!==`!**

### 8-те Falsy стойности в JavaScript (всичко останало е Truthy!):
`false`, `0`, `-0`, `0n`, `""` (празен стринг), `null`, `undefined`, `NaN`.
*(Внимание: Празен масив `[]` и празен обект `{}` са **Truthy**!)*

---

## 6. Скрити капани и Edge Cases

1. **Историческият бъг `typeof null === "object"`:** Поради архитектурата на първия JS интерпретатор от 1995 г., `typeof null` връща `"object"`. Винаги проверявайте за `null` директно с `value === null`.
2. **Капанът `NaN !== NaN`:** `NaN` е единствената стойност в JavaScript, която не е равна на себе си. Използвайте `Number.isNaN(val)` (не глобалното `isNaN()`, което преобразува стрингове).
3. **Плаваща запетая (`0.1 + 0.2 === 0.30000000000000004`):** Както в C# (`double`), двоичното представяне по IEEE 754 води до малка неточност. За финансови изчисления работете в стотинки (цели числа) или сравнявайте с `Math.abs(a - b) < Number.EPSILON`.
4. **Операторът `+` срещу `-` при стрингове:**
   * `"5" + 2` ➔ `"52"` (`+` предпочита конкатенация, ако единият операнд е стринг).
   * `"5" - 2` ➔ `3` (`-`, `*`, `/` винаги преобразуват към числа).

---

[⬅️ 02-00. Intro to JavaScript & V8 Engine](02-00-Intro-To-JavaScript-And-V8-Engine.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-01-01 Variables, Types & Coercion — Syntax](02-01-00-Variables-Types-And-Coercion.md)

© 2026 Dev-Library / 02-01-00. Variables, Data Types & Type Coercion (Теория и Капани)
