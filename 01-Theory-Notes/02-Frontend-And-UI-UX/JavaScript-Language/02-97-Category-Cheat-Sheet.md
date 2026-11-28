[⬅️ 02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-98. Common Errors, Edge Cases & Fixes](02-98-Common-Errors-And-Fixes.md)

---

# 🟨 02-97. JavaScript Category Cheat Sheet (Справочник по категории)

## 📑 Съдържание
* [1. Масиви (Arrays — Мутиращи срещу Немутиращи)](#1-масиви-arrays--мутиращи-срещу-немутиращи)
* [2. Обекти (Objects) и Копиране](#2-обекти-objects-и-копиране)
* [3. Колекции: `Map`, `Set`, `WeakMap` и `WeakSet`](#3-колекции-map-set-weakmap-и-weakset)
* [4. Стрингове (Strings) и Regex](#4-стрингове-strings-и-regex)
* [5. Promises & Async Control Flow](#5-promises--async-control-flow)
* [6. Модерни ES2022–ES2025+ Оператори и Идиоми](#6-модерни-es2022es2025-оператори-и-идиоми)

---

## 1. Масиви (Arrays — Мутиращи срещу Немутиращи)

| Метод | Действие | Мутира оригинала? | Връща |
| :--- | :--- | :--- | :--- |
| `map(fn)` | Трансформира всеки елемент | ❌ Не | Нов масив със същата дължина |
| `filter(fn)` | Филтрира по условие | ❌ Не | Нов масив с избраните елементи |
| `reduce(fn, init)` | Натрупва (агрегира) в 1 стойност | ❌ Не | Акумулираната стойност |
| `find(fn)` / `findLast(fn)` | Първи / последен съвпадащ елемент | ❌ Не | Елемента или `undefined` |
| `some(fn)` / `every(fn)` | Поне един / всички отговарят | ❌ Не | `boolean` |
| `flatMap(fn)` | `map()` + изравняване с 1 ниво | ❌ Не | Нов плосък масив |
| `toSorted(cmp)` | Сортира (ES2023+) | ❌ Не | Нов сортиран масив |
| `toReversed()` | Обръща реда (ES2023+) | ❌ Не | Нов обърнат масив |
| `toSpliced(i, del, ...add)` | Изтрива/вмъква на индекс (ES2023+) | ❌ Не | Нов променен масив |
| `with(index, val)` | Заменя елемент на индекс (ES2023+) | ❌ Не | Нов масив с подменен елемент |
| `push` / `pop` / `shift` / `unshift` | Добавяне/премахване в края/началото | ⚠️ **ДА** | Нова дължина или премахнатия елемент |
| `sort(cmp)` / `reverse()` / `splice()` | Сортиране/обръщане/рязане на място | ⚠️ **ДА** | Мутирания оригинален масив |

---

## 2. Обекти (Objects) и Копиране

| Метод / Синтаксис | Предназначение | Пример |
| :--- | :--- | :--- |
| `Object.keys(obj)` | Масив от собствените изброими ключове | `["id", "name"]` |
| `Object.values(obj)` | Масив от стойностите | `[1, "Calc"]` |
| `Object.entries(obj)` | Масив от двойки `[key, value]` | `[["id", 1], ["name", "Calc"]]` |
| `Object.fromEntries(entries)` | Обратно от `[key, value]` в обект | `Object.fromEntries(map)` |
| `Object.groupBy(arr, fn)` | Групира елементи по ключ (ES2024+) | `Object.groupBy(items, x => x.type)` |
| `Object.hasOwn(obj, "k")` | Безопасна проверка за собствено свойство | `true` / `false` |
| `Object.freeze(obj)` | Плитко заключване срещу промени | Предотвратява мутации на 1-во ниво |
| `structuredClone(obj)` | Вградено дълбоко копиране (Deep Copy) | Копира вложени обекти, масиви, `Date`, `Map` |

---

## 3. Колекции: `Map`, `Set`, `WeakMap` и `WeakSet`

| Колекция | Ключове / Стойности | Особености и приложение |
| :--- | :--- | :--- |
| **`Map`** | Всякакъв тип ключове (вкл. обекти и функции) | Запазва реда на вмъкване, има `.size`, бързо добавяне/триене |
| **`Set`** | Само уникални стойности ($O(1)$ търсене) | Премахване на дубликати: `[...new Set(arr)]`, `.union()`, `.intersection()` |
| **`WeakMap`** | Само обекти като ключове (слаби референции) | Не пречи на Garbage Collector-а — идеално за кеш/метаданни към DOM възли |
| **`WeakSet`** | Само обекти (слаби референции) | Маркиране на вече обработени обекти без Memory Leak |

---

## 4. Стрингове (Strings) и Regex

```javascript
const text = "  Formula: SUM(A1:A10)  ";

text.trim();                      // "Formula: SUM(A1:A10)"
text.toLowerCase();               // "  formula: sum(a1:a10)  "
text.includes("SUM");             // true
text.startsWith("  Form");        // true
text.replaceAll("A", "B");        // Заменя ВСИЧКИ срещания (ES2021+)
"7".padStart(3, "0");             // "007"
text.slice(2, 9);                 // "Formula"
```

---

## 5. Promises & Async Control Flow

| Метод | Поведение при успех | Поведение при грешка |
| :--- | :--- | :--- |
| `Promise.all([...])` | Връща масив с всички резултати | Проваля се веднага при 1-вата грешка |
| `Promise.allSettled([...])` | Връща `{ status, value / reason }` за всеки | Никога не се проваля — изчаква всички |
| `Promise.race([...])` | Връща първия приключил Promise | Проваля се, ако първият приключил е грешка |
| `Promise.any([...])` | Връща първия **успешен** Promise | Проваля се с `AggregateError` само ако всички пропаднат |

---

## 6. Модерни ES2022–ES2025+ Оператори и Идиоми

* **Optional Chaining:** `user?.address?.city` и `callback?.()`
* **Nullish Coalescing:** `input ?? "default"` (задейства се само при `null` или `undefined`)
* **Logical Assignment:** `obj.count ??= 0`, `obj.name ||= "Anon"`
* **Numeric Separators:** `const oneMillion = 1_000_000;`
* **Top-Level `await`:** Възможност за директно `await fetch(...)` в ES Modules (`.mjs` / `type="module"`) без обвиваща `async` функция.

---

[⬅️ 02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-98. Common Errors, Edge Cases & Fixes](02-98-Common-Errors-And-Fixes.md)

© 2026 Dev-Library / 02-97. JavaScript Category Cheat Sheet (Справочник по категории)
