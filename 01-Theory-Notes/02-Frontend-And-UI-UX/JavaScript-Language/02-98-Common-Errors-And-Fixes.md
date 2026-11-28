[⬅️ 02-97. JavaScript Category Cheat Sheet](02-97-Category-Cheat-Sheet.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-99. Interview Q&A, Скрити Капани & Debugging Tech...](02-99-Interview-QA-Traps-And-Debugging.md)

---

# 🟨 02-98. Common Errors, Edge Cases & Fixes (Каталог на грешките)

## 📑 Съдържание
* [1. Фази на възникване на грешките (Parse-Time vs Runtime vs Async)](#1-фази-на-възникване-на-грешките-parse-time-vs-runtime-vs-async)
* [2. `TypeError` — Най-честите причини и решения](#2-typeerror--най-честите-причини-и-решения)
* [3. `ReferenceError` — Hoisting, TDZ и липсващи променливи](#3-referenceerror--hoisting-tdz-и-липсващи-променливи)
* [4. Гранични случаи с числа (`NaN`, IEEE 754, `Number.isNaN`)](#4-гранични-случаи-с-числа-nan-ieee-754-numberisnan)
* [5. Тихи грешки с мутации и референции (Silent Bugs)](#5-тихи-грешки-с-мутации-и-референции-silent-bugs)
* [6. Бърза диагностична таблица (Симптом ➔ Причина ➔ Решение)](#6-бърза-диагностична-таблица-симптом--причина--решение)

---

## 1. Фази на възникване на грешките (Parse-Time vs Runtime vs Async)

| Фаза | Кога се случва? | Типични грешки |
| :--- | :--- | :--- |
| **1. Parse-Time (Creation Phase)** | Преди изпълнението на първия ред код (при AST парсване) | `SyntaxError` (незатворена скоба, дублиран `let x`, достъп до несъществуващо `#private` поле) |
| **2. Synchronous Runtime** | По време на изпълнение в Call Stack | `TypeError`, `ReferenceError`, `RangeError` (`Maximum call stack size exceeded`) |
| **3. Asynchronous Runtime** | При приключване на `Promise` или събитие | `UnhandledPromiseRejection`, `AbortError`, мрежови грешки в `fetch()` |

---

## 2. `TypeError` — Най-честите причини и решения

* **`Cannot read properties of undefined / null (reading 'x')`**
  * *Причина:* Опит за достъп до свойство върху `null` или `undefined` (напр. отговор от API, който не е дошъл, или несъществуващ DOM елемент от `querySelector`).
  * *Решение:* Използвайте Optional Chaining (`user?.profile?.avatar`) и защитни проверки (`if (!el) return;`).
* **`x is not a function`**
  * *Причина:* Извиквате стойност, която не е функция (напр. `forEach` върху `NodeList` в стар браузър или обект вместо масив, или метод, презаписан от свойство със същото име).
  * *Решение:* Проверете типа с `Array.isArray(items)` или използвайте безопасно извикване `onSuccess?.(data)`.
* **`Assignment to constant variable`**
  * *Причина:* Опит за презаписване на референцията на променлива, декларирана с `const` (напр. `count++` върху `const count = 0`).
  * *Решение:* Сменете `const` с `let`, ако стойността трябва да се променя.

---

## 3. `ReferenceError` — Hoisting, TDZ и липсващи променливи

* **`Cannot access 'x' before initialization`**
  * *Причина:* Променлива с `let`, `const` или `class` се достъпва в **Temporal Dead Zone (TDZ)** — преди реда на нейната декларация.
  * *Решение:* Винаги декларирайте променливите и класовете в началото на обхвата, преди първото им използване.
* **`Must call super constructor in derived class before accessing 'this'`**
  * *Причина:* В конструктор на наследник (`class B extends A`) използвате `this` преди `super()`.
  * *Решение:* Преместете `super(...)` като първи ред в конструктора.

---

## 4. Гранични случаи с числа (`NaN`, IEEE 754, `Number.isNaN`)

* **Проблемът с глобалното `isNaN("abc")`:**
  * Глобалното `isNaN("abc")` първо преобразува входа в число (`Number("abc") -> NaN`) и връща `true` за обикновен стринг!
  * *Решение:* Винаги използвайте строгото **`Number.isNaN(val)`**, което връща `true` **само** ако стойността реално е `NaN`.
* **Грешки при десетични дроби (`0.1 + 0.2 === 0.30000000000000004`):**
  * *Причина:* Двоично представяне с плаваща запетая (IEEE 754).
  * *Решение:* За финансови суми изчислявайте в цели стотинки/центове (`10 + 20 = 30`) или закръглявайте с `Number((0.1 + 0.2).toFixed(2))` (или `Math.abs(a - b) < Number.EPSILON` при сравнения).
* **Парсване на числа с водеща нула (`parseInt("08")`):**
  * *Решение:* Винаги подавайте основа 10: `parseInt(str, 10)` или `Number(str)`.

---

## 5. Тихи грешки с мутации и референции (Silent Bugs)

1. **`Array.prototype.sort()` без компаратор сортира числа като стрингове!**
   * `[10, 2, 30].sort()` връща **`[10, 2, 30]`** (защото `"10" < "2"` по UTF-16 код!).
   * *Решение:* Винаги подавайте числов компаратор и използвайте немутиращото `toSorted`: `nums.toSorted((a, b) => a - b)`.
2. **Плитко копиране (`{ ...obj }`) при вложени обекти:**
   * `{ ...user }` копира само първото ниво — промяна в `copy.settings.theme` ще промени и оригиналния `user.settings.theme`!
   * *Решение:* Използвайте вграденото **`structuredClone(user)`** за дълбоко копиране.
3. **Забравен `return` в стрелкова функция с къдрави скоби:**
   * `arr.map(x => { x * 2 })` връща `[undefined, undefined]`!
   * *Решение:* Или премахнете къдравите скоби (`x => x * 2`), или напишете изрично `return` (`x => { return x * 2; }`).

---

## 6. Бърза диагностична таблица (Симптом ➔ Причина ➔ Решение)

| Грешка / Симптом | Най-честа причина | Бързо решение |
| :--- | :--- | :--- |
| `TypeError: Cannot read properties of null` | `querySelector` не е намерил елемента или скриптът е в `<head>` без `defer` | Добавете `defer` на `<script>` и проверка `if (!el) return;` |
| `SyntaxError: Unexpected token '<'` при `res.json()` | Сървърът е върнал HTML страница (404/500) вместо JSON | Проверете `if (!res.ok)` преди `await res.json()` |
| `TypeError: Converting circular structure to JSON` | Обектът съдържа циклична референция (`a.self = a`) | Изчистете цикличните връзки или подайте `replacer` в `JSON.stringify` |
| `UnhandledPromiseRejection` | Липсва `try/catch` около `await` или `.catch()` след Promise | Обвийте `await` блока в `try { ... } catch (err) { ... }` |
| `this` е `undefined` вътре в метод | Методът е подаден като callback (`el.onclick = obj.save`) | Използвайте `() => obj.save()` или `obj.save.bind(obj)` |







---

[⬅️ 02-97. JavaScript Category Cheat Sheet](02-97-Category-Cheat-Sheet.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-99. Interview Q&A, Скрити Капани & Debugging Tech...](02-99-Interview-QA-Traps-And-Debugging.md)

© 2026 Dev-Library / 02-98. Common Errors, Edge Cases & Fixes (Каталог на грешките)
