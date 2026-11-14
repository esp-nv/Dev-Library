[⬅️ 02-08. TypeScript Deep Dive Addendum](02-08-TypeScript-Deep-Dive-Addendum.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-10. Category Cheat Sheet](02-10-Category-Cheat-Sheet.md)

---

# 02-09. TypeScript Оптимизации: Кое е грешно, защо и как е по-добре (с Блок-схеми)

**Цел:** Инженерен анализ на най-честите грешки в производителността на TypeScript (както по време на компилация `tsc`, така и по време на изпълнение в V8 двигателя), визуални блок-схеми на процесите и оптимизирани решения.

## 🧭 Съдържание на темата
* [1. Оптимизация 1: Търсене в цикъл (`Array.find` / `includes`) срещу `Set` и `Map` (O(n²) срещу O(n))](#1-оптимизация-1-търсене-в-цикъл-arrayfind--includes-срещу-set-и-map-on2-срещу-on)
* [2. Оптимизация 2: Поредица от `await` заявки срещу `Promise.all` (Мрежова латентност)](#2-оптимизация-2-поредица-от-await-заявки-срещу-promiseall-мрежова-латентност)
* [3. Оптимизация 3: Бавна компилация (`interface extends` срещу сложни `type` сечения `&`)](#3-оптимизация-3-бавна-компилация-interface-extends-срещу-сложни-type-сечения-)
* [4. Оптимизация 4: Прекомерно копиране на масиви със Spread (`...acc`) в `.reduce()`](#4-оптимизация-4-прекомерно-копиране-на-масиви-със-spread-acc-в-reduce)

---

## 1. ОПТИМИЗАЦИЯ 1: ТЪРСЕНЕ В ЦИКЪЛ (`ARRAY.FIND` / `INCLUDES`) СРЕЩУ `SET` И `MAP` (O(N²) СРЕЩУ O(N))

### Блок-схема на проблема и решението:
```text
❌ БАВЕН ПОДХОД (Вложен цикъл с Array.includes / Array.find — O(N * M)):
   За всеки от 10 000 потребители ──► Обхожда целия масив от 5 000 забранени ID-та отначало!
   Общо операции: 10 000 * 5 000 = 50 000 000 сравнения! (~450 ms блокиран UI)

✅ БЪРЗ ПОДХОД (Хеш таблица с Set / Map — O(N + M)):
   1. Превръща забранените ID-та в Set<number> (1 минаване)
   2. За всеки потребител проверява bannedSet.has(user.id) ──► Директен O(1) достъп по хеш!
   Общо операции: 15 000 стъпки! (~1.2 ms — 375x по-бързо!)
```

### Кодово сравнение:
```typescript
interface User { id: number; name: string; }

// ❌ ГРЕШНО И БАВНО: O(N * M) сложност
function filterActiveSlow(users: User[], bannedIds: number[]): User[] {
    return users.filter(user => !bannedIds.includes(user.id));
}

// ✅ ПРАВИЛНО И ОПТИМИЗИРАНО: O(N + M) сложност
function filterActiveFast(users: User[], bannedIds: number[]): User[] {
    const bannedSet = new Set<number>(bannedIds);
    return users.filter(user => !bannedSet.has(user.id));
}
```
*   **Защо е по-добре:** `Array.includes()` проверява елементите последователно от индекс `0` до края при всяка итерация. `Set.has()` и `Map.get()` използват вътрешна хеш таблица в V8 и намират елемента за константно време O(1).

---

## 2. ОПТИМИЗАЦИЯ 2: ПОРЕДИЦА ОТ `AWAIT` ЗАЯВКИ СРЕЩУ `PROMISE.ALL` (МРЕЖОВА ЛАТЕНТНОСТ)

### Блок-схема на проблема и решението:
```text
❌ БАВЕН ПОДХОД (Последователен await — Waterfall):
   [GET /user] (200ms) ──► [GET /orders] (250ms) ──► [GET /settings] (150ms) = 600 ms общо!

✅ БЪРЗ ПОДХОД (Паралелно изпълнение с Promise.all):
   ┌──► [GET /user]     (200ms) ──┐
   ├──► [GET /orders]   (250ms) ──┼──► Всички готови за 250 ms (най-бавната заявка)!
   └──► [GET /settings] (150ms) ──┘
```

### Кодово сравнение:
```typescript
// ❌ ГРЕШНО: Независими заявки чакат една след друга
async function loadDashboardSlow(userId: number) {
    const user = await fetchUser(userId);         // 200ms
    const orders = await fetchOrders(userId);     // +250ms
    const settings = await fetchSettings(userId); // +150ms
    return { user, orders, settings };            // Общо: 600ms
}

// ✅ ПРАВИЛНО: Изстрелват се едновременно и се изчакват паралелно
async function loadDashboardFast(userId: number) {
    const [user, orders, settings] = await Promise.all([
        fetchUser(userId),
        fetchOrders(userId),
        fetchSettings(userId)
    ]);
    return { user, orders, settings }; // Общо: 250ms (2.4x по-бързо!)
}
```
*   **Защо е по-добре:** Когато трите заявки не зависят една от друга, последователният `await` държи Event Loop-а в изчакване без причина. `Promise.all` стартира всички I/O операции веднага. *(Забележка: Ако искаш при грешка в една заявка останалите да не пропадат, ползвай `Promise.allSettled`!)*
---

## 3. ОПТИМИЗАЦИЯ 3: БАВНА КОМПИЛАЦИЯ (`INTERFACE EXTENDS` СРЕЩУ СЛОЖНИ `TYPE` СЕЧЕНИЯ `&`)

### Блок-схема на работата на TypeScript компилатора (`tsc`):
```text
❌ БАВЕН ПОДХОД (Intersection Type: type C = A & B & D):
   tsc преизчислява и слива всички свойства от нулата при ВСЯКА проверка ──► Бавно IDE автодопълнение!

✅ БЪРЗ ПОДХОД (Interface Inheritance: interface C extends A, B, D):
   tsc създава единен кеширан обектен тип с фиксиран ID ──► Мигновена проверка и по-ясни грешки!
```

### Кодово сравнение:
```typescript
interface BaseEntity { id: number; createdAt: Date; }
interface Auditable { updatedBy: string; }

// ❌ ПО-БАВНО ЗА КОМПИЛАТОРА ПРИ ГОЛЕМИ ПРОЕКТИ:
type OrderIntersection = BaseEntity & Auditable & { total: number };

// ✅ ОПТИМИЗИРАНО (Официална препоръка от TypeScript Performance Wiki):
interface OrderOptimized extends BaseEntity, Auditable {
    total: number;
}
```
*   **Защо е по-добре:** При `interface extends` компилаторът кешира плоската структура на обекта в паметта и открива конфликти на типове веднага, докато при множество `&` сечения `tsc` ги оценява рекурсивно при всяка употреба.

---

## 4. ОПТИМИЗАЦИЯ 4: ПРЕКОМЕРНО КОПИРАНЕ НА МАСИВИ СЪС SPREAD (`...ACC`) В `.REDUCE()`

### Блок-схема на проблема и решението:
```text
❌ БАВЕН ПОДХОД ([...acc, item] вътре в .reduce() — O(N²) памет и време):
   Стъпка 1: копира 1 елемент   ──► нова алокация в Heap
   Стъпка 2: копира 2 елемента  ──► нова алокация в Heap
   ...
   Стъпка 10 000: копира 10 000 елемента! ──► 50 милиона копирания + тежък Garbage Collector!

✅ БЪРЗ ПОДХОД (Директен acc.push(item) вътре в .reduce() — O(N)):
   Създава 1 масив в началото ──► добавя всеки елемент в края за O(1) ──► 10 000 стъпки общо!
```

### Кодово сравнение:
```typescript
interface Product { id: number; inStock: boolean; name: string; }

// ❌ ГРЕШНО И БАВНО: Алокира нов масив при всяка итерация (O(N²))
function getAvailableNamesSlow(products: Product[]): string[] {
    return products.reduce<string[]>((acc, p) => {
        return p.inStock ? [...acc, p.name] : acc;
    }, []);
}

// ✅ ПРАВИЛНО И ОПТИМИЗИРАНО: Мутира локалния акумулатор (O(N))
function getAvailableNamesFast(products: Product[]): string[] {
    return products.reduce<string[]>((acc, p) => {
        if (p.inStock) {
            acc.push(p.name);
        }
        return acc;
    }, []);
}
```
*   **Защо е по-добре:** Вътре в `.reduce()` масивът `acc` е локален за самата операция — директното `acc.push()` не нарушава функционалната чистота на входния масив `products`, но спестява хиляди излишни копирания в паметта.

---

## 5. БЪРЗ ОДИТ НА ОПТИМИЗАЦИИТЕ В TYPESCRIPT

| Сценарий | Бавен подход | Оптимизиран подход | Печалба |
| :--- | :--- | :--- | :--- |
| **Търсене на дубликати / ID-та** | `array.includes()` в цикъл | `new Set()` / `new Map()` | От $O(n^2)$ на $O(n)$ |
| **Независими API заявки** | Поредица от `await` | `Promise.all([...])` | Сумарно време $\rightarrow$ време на най-бавната |
| **Дефиниране на обектни модели** | Сложни `type A = B & C` | `interface A extends B, C` | По-бърз `tsc` билд и IDE IntelliSense |
| **Агрегиране в `.reduce()`** | `[...acc, newItem]` | `acc.push(newItem)` | Нулеви междинни алокации в паметта |
| **Импортиране от модули** | Големи Barrel файлове (`index.ts`) | Директен импорт от конкретния файл | По-добър **Tree-Shaking** и по-малък Bundle |

---
[⬅️ 02-08. TypeScript Deep Dive Addendum](02-08-TypeScript-Deep-Dive-Addendum.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-10. Category Cheat Sheet](02-10-Category-Cheat-Sheet.md)
