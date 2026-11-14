[⬅️ 02-11. TypeScript Common Errors & Fixes](02-11-Common-Errors-And-Fixes.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 03-Python-Scripting](../03-Python-Scripting/README.md)

---

# 02-12. TypeScript Interview Q&A, Скрити Капани & Debugging Techniques

**Цел:** Подготовка за технически интервюта по TypeScript (Junior до Senior ниво), критични подводни камъни в типовата система, архитектурно управление на грешки (`Custom Errors` и `cause` вериги) и професионални техники за дебъгване в VS Code и браузър DevTools.

## 🧭 Съдържание на темата
* [1. Интервю въпросник (Top 6 въпроса с точни Senior отговори)](#1-интервю-въпросник-top-6-въпроса-с-точни-senior-отговори)
* [2. Скрити капани в TypeScript (Структурна типизация, `enum` капани, `any` срещу `unknown`)](#2-скрити-капани-в-typescript-структурна-типизация-enum-капани-any-срещу-unknown)
* [3. Професионално управление на изключения (Custom Errors & Exception Chaining с `cause`)](#3-професионално-управление-на-изключения-custom-errors--exception-chaining-с-cause)
* [4. Debugging Techniques в TypeScript (VS Code Breakpoints, Source Maps & Chrome DevTools)](#4-debugging-techniques-в-typescript-vs-code-breakpoints-source-maps--chrome-devtools)

---

## 1. ИНТЕРВЮ ВЪПРОСНИК (TOP 6 ВЪПРОСА С ТОЧНИ SENIOR ОТГОВОРИ)

1. **Въпрос:** *„Каква е разликата между **`unknown`** и **`any`** в TypeScript?“*
   * **Отговор:** `any` изключва напълно проверката на типове (позволява да викаш несъществуващи методи без грешка при компилация). `unknown` е типово безопасният аналог на `Object` — приема всякаква стойност, но **забранява** всякакви операции върху нея, докато не стесниш типа чрез `typeof`, `instanceof` или Custom Type Guard.

2. **Въпрос:** *„Какво е **Type Erasure** и какво означава за Runtime изпълнението?“*
   * **Отговор:** Всички `interface`, `type` и генерични параметри (`<T>`) съществуват **само по време на компилация**. При компилиране до JavaScript те се изтриват напълно — затова не можеш да напишеш `if (data instanceof MyInterface)` в Runtime.

3. **Въпрос:** *„Каква е разликата между **Structural Typing (Duck Typing)** в TypeScript и **Nominal Typing** в C# и Java?“*
   * **Отговор:** В C# и Java два класа `Person` и `Customer` с еднакви полета са несъвместими, освен ако не наследяват общ интерфейс. В TypeScript типовете се сравняват по **формата (структурата) на свойствата им** — ако два типа имат еднакви свойства и типове, те са напълно взаимозаменяеми.

4. **Въпрос:** *„Каква е разликата между **`interface`** и **`type`**?“*
   * **Отговор:** `interface` описва структурата на обекти/класове, поддържа `extends` и **Declaration Merging** (сливане на две декларации с едно и също име). `type` може да описва примитиви, Union (`A | B`), Intersection (`A & B`) и Tuples, но не може да се декларира повторно.

5. **Въпрос:** *„Как работят **`satisfies`** и **`as const`** заедно?“*
   * **Отговор:** `as const` замразява стойностите до най-тесните им литерални типове (`readonly`), а `satisfies` проверява дали обектът отговаря на даден договор/интерфейс, **без да разширява (губи)** точните литерални типове на свойствата.

6. **Въпрос:** *„Кога да използваме **Mapped Types** (`[K in keyof T]`)?“*
   * **Отговор:** Когато искаме автоматично да създадем нов тип на базата на съществуващ (например да направим всички полета на форма `boolean` флагове за валидност или да генерираме гетъри).

---

## 2. СКРИТИ КАПАНИ В TYPESCRIPT (СТРУКТУРНА ТИПИЗАЦИЯ, `ENUM` КАПАНИ, `ANY` СРЕЩУ `UNKNOWN`)

1. **Капанът на класическия `enum`:**
   * Числовият `enum Role { User, Admin }` генерира обратен мапинг в JavaScript обект и допуска присвояване на произволно число в старите версии, а `const enum` чупи изолацията на модулите (`isolatedModules: true` в Vite / esbuild).
   * **Решение:** Използвай **`as const` обект + Union тип**:
     ```typescript
     const Roles = { User: "USER", Admin: "ADMIN" } as const;
     type Role = typeof Roles[keyof typeof Roles]; // "USER" | "ADMIN"
     ```
2. **Капанът на `Object.keys(obj)`:**
   * В TypeScript `Object.keys(user)` връща `string[]`, а не `(keyof User)[]`, защото в Runtime обектът може да има допълнителни скрити свойства заради структурната типизация.
3. **Капанът на `this` загубата при подаване на метод като Callback:**
   * Ако подадеш `onClick={service.save}` без `.bind(service)` или стрелкова функция `save = () => { ... }`, вътре в метода `this` ще бъде `undefined`!

---

## 3. DEBUGGING TECHNIQUES В TYPESCRIPT (VS CODE BREAKPOINTS, SOURCE MAPS & CHROME DEVTOOLS)

*   **Source Maps (`"sourceMap": true` в `tsconfig.json`):** Тъй като Node.js и браузърът изпълняват компилирания JavaScript, `.js.map` файловете свързват всеки ред от JS обратно с оригиналния `.ts` файл и точния номер на реда.
*   **Conditional Breakpoints (Условни точки на прекъсване):** В VS Code кликни с десен бутон върху червената точка (Breakpoint) ➔ *Edit Breakpoint* ➔ въведи условие (напр. `user.id === 999` или `items.length === 0`). Програмата ще спре **само** при граничния случай, без да цъкаш F10 сто пъти в цикъл!
*   **Logpoints (Логове без промяна на кода):** Десен бутон в полето за Breakpoints ➔ *Add Logpoint* ➔ `{user.email} - {items.length}`. Принтира в конзолата без да спира изпълнението и без да замърсяваш кода с `console.log()`.
*   **Watch Window & Call Stack:** В панела *Run and Debug* (`Ctrl+Shift+D`) добави сложни изрази в **Watch** (напр. `JSON.stringify(state)`) и проследи откъде е извикана функцията през **Call Stack**.

---
[⬅️ 02-11. TypeScript Common Errors & Fixes](02-11-Common-Errors-And-Fixes.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 03-Python-Scripting](../03-Python-Scripting/README.md)
