[⬅️ 02-09-01. Unit Testing & Playwright E2E — Syntax & C...](02-09-01-Syntax-Testing-And-E2E-Playwright.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-97. JavaScript Category Cheat Sheet](02-97-Category-Cheat-Sheet.md)

---

# 🟨 02-96. JavaScript Performance & Optimizations (с Блок-схеми)

## 📑 Съдържание
* [1. Как V8 оптимизира обекти: Hidden Classes (Shapes) & Inline Caching](#1-как-v8-оптимизира-обекти-hidden-classes-shapes--inline-caching)
* [2. Оптимизация на DOM: Избягване на Layout Thrashing (Forced Reflow)](#2-оптимизация-на-dom-избягване-на-layout-thrashing-forced-reflow)
* [3. Контрол на честотата на събитията: Debouncing vs Throttling](#3-контрол-на-честотата-на-събитията-debouncing-vs-throttling)
* [4. Предотвратяване на Memory Leaks (Течове на памет)](#4-предотвратяване-на-memory-leaks-течове-на-памет)
* [5. Бързи структури от данни (`Map` / `Set` срещу `Object` / `Array`)](#5-бързи-структури-от-данни-map--set-срещу-object--array)

---

## 1. Как V8 оптимизира обекти: Hidden Classes (Shapes) & Inline Caching

Въпреки че JavaScript е динамичен език, V8 създава скрити C++ структури (**Hidden Classes / Shapes**) зад кулисите, за да чете свойствата на обектите с директно отместване в паметта (O(1)), вместо да прави бавно търсене в хеш-таблица.

```text
✅ MONOMORPHIC (Бърз път - TurboFan JIT)        ❌ MEGAMORPHIC (Бавен път - Deoptimization)
const p1 = { x: 1, y: 2 }; // Shape_A            const p1 = { x: 1, y: 2 }; // Shape_A
const p2 = { x: 3, y: 4 }; // Shape_A            const p2 = { y: 4, x: 3 }; // Shape_B (различен ред!)
                                                 delete p1.x;               // Dictionary Mode (бавно!)
```

### Инженерни правила за V8 оптимизация:
1. **Винаги инициализирайте свойствата на обектите в еднакъв ред** (най-добре в `constructor`).
2. **Не добавяйте динамично нови свойства** след създаването на обекта.
3. **Никога не използвайте `delete obj.prop`** върху горещи обекти (това превключва обекта в бавен *Dictionary Mode*). Вместо това задайте `obj.prop = undefined` или използвайте `Map`.

---

## 2. Оптимизация на DOM: Избягване на Layout Thrashing (Forced Reflow)

Браузърът групира DOM промените и ги преизчислява веднъж на кадър (~16.6ms). Но ако в един цикъл редувате **четене на геометрия** (`offsetWidth`, `getBoundingClientRect()`) и **писане в стиловете** (`style.width = ...`), принуждавате браузъра да преизчислява целия Layout на всяка итерация (**Layout Thrashing**):

```text
❌ LAYOUT THRASHING (N пъти Reflow)             ✅ BATCHED READS & WRITES (1 път Reflow)
for (el of boxes) {                              const widths = boxes.map(el => el.offsetWidth); // 1. Всички четения
  const w = el.offsetWidth; // Чете (Reflow!)    boxes.forEach((el, i) => {                      // 2. Всички писания
  el.style.width = w + 10 + "px"; // Пише          el.style.width = widths[i] + 10 + "px";
}                                                });
```

---

## 3. Контрол на честотата на събитията: Debouncing vs Throttling

```text
Събития (напр. писане / scroll):  │─│─│─│───────│─│─│───────────►
Debouncing (изчаква тишина):      └─────────[X]─────────[X]─────► (Търсачка / Auto-save)
Throttling (на фиксиран интервал): [X]───[X]───[X]───[X]───[X]──► (Scroll / Resize / Mousemove)
```

```javascript
// 1. Debounce — изпълнява функцията ЕДВА след като минат delay ms без нови извиквания
function debounce(fn, delayMs = 300) {
  let timerId;
  return (...args) => {
    clearTimeout(timerId);
    timerId = setTimeout(() => fn(...args), delayMs);
  };
}

// 2. Throttle — изпълнява функцията МАКСИМУМ веднъж на всеки intervalMs
function throttle(fn, intervalMs = 100) {
  let lastTime = 0;
  return (...args) => {
    const now = Date.now();
    if (now - lastTime >= intervalMs) {
      lastTime = now;
      fn(...args);
    }
  };
}
```

---

## 4. Предотвратяване на Memory Leaks (Течове на памет)

Garbage Collector-ът на V8 (Mark-and-Sweep) освобождава обекти, до които няма референция от `Root`. Най-честите причини за течове в SPA приложения са:
1. **Неизчистени таймери:** `setInterval(...)` без `clearInterval(id)` при унищожаване на компонент.
2. **Detached DOM Nodes:** Запазена референция в JS променлива към DOM елемент, който вече е премахнат от документа (`el.remove()`).
3. **Непремахнати глобални Event Listeners:** `window.addEventListener("resize", handler)` без `removeEventListener` (или `{ signal: abortController.signal }`).
4. **Неограничени кешове:** `Map` или масив, в който само се добавят записи без лимит (използвайте `WeakMap` когато ключовете са обекти).

---

## 5. Бързи структури от данни (`Map` / `Set` срещу `Object` / `Array`)

| Задача | Бавен подход | Бърз подход | Сложност |
| :--- | :--- | :--- | :--- |
| **Проверка за наличие в списък** | `arr.includes(id)` | `set.has(id)` | $O(n)$ ➔ **$O(1)$** |
| **Премахване на дубликати** | `arr.filter((v, i) => arr.indexOf(v) === i)` | `[...new Set(arr)]` | $O(n^2)$ ➔ **$O(n)$** |
| **Често добавяне/триене по ключ** | `obj[key] = val` / `delete obj[key]` | `map.set(key, val)` / `map.delete(key)` | Оптимизирано за мутации |
| **Сложни анимации в браузъра** | `setInterval(step, 16)` | `requestAnimationFrame(step)` | Синхронизирано с V-Sync |







---

[⬅️ 02-09-01. Unit Testing & Playwright E2E — Syntax & C...](02-09-01-Syntax-Testing-And-E2E-Playwright.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-97. JavaScript Category Cheat Sheet](02-97-Category-Cheat-Sheet.md)

© 2026 Dev-Library / 02-96. JavaScript Performance & Optimizations (с Блок-схеми)
