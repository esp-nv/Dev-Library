[⬅️ 02-09. TypeScript Оптимизации](02-09-Performance-And-Optimizations.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-11. Common Errors And Fixes](02-11-Common-Errors-And-Fixes.md)

---

# 02-10. TypeScript Category Cheat Sheet: Пълно обобщение по категории

**Цел:** Бърза синтактична и архитектурна справка на едно място за всички ключови категории в модерния **TypeScript 5.x+** — типове, обектни договори, Generics, вградени Utility Types, Type Narrowing и конфигурация на компилатора.

## 🧭 Съдържание по категории
* [1. Категория 1: Примитивни, колекционни и специални типове](#1-категория-1-примитивни-колекционни-и-специални-типове)
* [2. Категория 2: `interface` срещу `type`, Модификатори и Класове](#2-категория-2-interface-срещу-type-модификатори-и-класове)
* [3. Категория 3: Вградени Utility Types (Инструменти за трансформация на типове)](#3-категория-3-вградени-utility-types-инструменти-за-трансформация-на-типове)
* [4. Категория 4: Generics, Type Narrowing (Type Guards) & `as const`](#4-категория-4-generics-type-narrowing-type-guards--as-const)
* [5. Категория 5: Критични настройки в `tsconfig.json`](#5-категория-5-критични-настройки-в-tsconfigjson)
* [6. КАТЕГОРИЯ 6: ВАЖНИ БИБЛИОТЕКИ В TYPESCRIPT ЕКОСИСТЕМАТА (ИМПОРТ, ИЗВИКВАНЕ И ПРИЛОЖЕНИЕ)](#6-категория-6-важни-библиотеки-в-typescript-екосистемата-импорт-извикване-и-приложение)

---

## 1. КАТЕГОРИЯ 1: ПРИМИТИВНИ, КОЛЕКЦИОННИ И СПЕЦИАЛНИ ТИПОВЕ

| Категория / Тип | Синтаксис и Пример | Кога се използва |
| :--- | :--- | :--- |
| **Примитиви** | `string`, `number`, `boolean`, `bigint`, `symbol` | Основни стойности (винаги с малка буква — никога `String` или `Number`!) |
| **Масиви** | `number[]` или `Array<number>` | Списъци от еднотипни елементи (`readonly string[]` за неизменяемост) |
| **Tuple (Кортеж)** | `type Pair = [string, number];` | Масив с фиксирана дължина и точен тип на всяка позиция (напр. React `useState`) |
| **`unknown` (Безопасен)** | `let input: unknown = fetchJson();` | Когато не знаеш типа предварително — **задължава** те да го провериш преди употреба |
| **`any` (Избягвай!)** | `let legacy: any = ...;` | Изключва напълно проверката на типовете в TypeScript |
| **`void` срещу `never`** | `function log(): void` / `function fail(): never` | `void` = не връща стойност; `never` = никога не завършва нормално (хвърля грешка или безкраен цикъл) |

---

## 2. КАТЕГОРИЯ 2: `INTERFACE` СРЕЩУ `TYPE`, МОДИФИКАТОРИ И КЛАСОВЕ

```typescript
// 1. Interface (Предпочитан за обектни модели, DTO-та и класови договори)
interface UserContract {
    readonly id: number;        // Не може да се променя след създаване
    username: string;
    bio?: string;               // Опционално свойство (string | undefined)
}

// 2. Type Alias (Задължителен за Union, Intersection, Primitives и Tuples)
type UserRole = "ADMIN" | "EDITOR" | "VIEWER";
type ApiResult<T> = { ok: true; data: T } | { ok: false; error: string };

// 3. Модерен TS клас с Parameter Properties (автоматично създава полета!)
class AccountService {
    constructor(
        private readonly apiUrl: string,
        protected timeoutMs: number = 5000
    ) {}
}
```

---

## 3. КАТЕГОРИЯ 3: ВГРАДЕНИ UTILITY TYPES (ИНСТРУМЕНТИ ЗА ТРАНСФОРМАЦИЯ НА ТИПОВЕ)

| Utility Type | Какво прави | Практически пример |
| :--- | :--- | :--- |
| **`Partial<T>`** | Прави всички свойства на `T` опционални (`?`) | `function updateUser(id: number, patch: Partial<User>)` |
| **`Required<T>`** | Прави всички свойства на `T` задължителни | `type StrictConfig = Required<AppConfig>;` |
| **`Readonly<T>`** | Забранява промяната на всички свойства на `T` | `const state: Readonly<AppState> = ...;` |
| **`Pick<T, K>`** | Избира **само** посочените свойства от `T` | `type UserPreview = Pick<User, "id" \| "username">;` |
| **`Omit<T, K>`** | Премахва посочените свойства от `T` | `type CreateUserDto = Omit<User, "id" \| "createdAt">;` |
| **`Record<K, V>`** | Създава обект-речник с ключове `K` и стойности `V` | `const rolePermissions: Record<UserRole, string[]> = ...;` |
| **`ReturnType<Fn>`** | Извлича типа, който дадена функция връща | `type ServiceOutput = ReturnType<typeof buildReport>;` |
| **`Awaited<T>`** | Разопакова типа вътре в `Promise<T>` | `type UserData = Awaited<ReturnType<typeof fetchUser>>;` |

---

## 4. КАТЕГОРИЯ 4: GENERICS, TYPE NARROWING (TYPE GUARDS) & `AS CONST`

```typescript
// 1. Generic функция с ограничение (Constraint: T трябва да има поле id)
function findById<T extends { id: number }>(items: T[], targetId: number): T | undefined {
    return items.find(item => item.id === targetId);
}

// 2. Custom Type Guard (is оператор за стесняване на unknown / Union)
interface AdminUser { role: "ADMIN"; permissions: string[]; }
interface RegularUser { role: "USER"; }

function isAdmin(user: AdminUser | RegularUser): user is AdminUser {
    return user.role === "ADMIN";
}

// 3. `as const` + `satisfies` (Точни литерални типове + проверка на договора!)
const HTTP_STATUS = {
    OK: 200,
    NOT_FOUND: 404,
    SERVER_ERROR: 500
} as const satisfies Record<string, number>;
// HTTP_STATUS.OK има точен тип 200 (а не общ number)!
```

---

## 5. КАТЕГОРИЯ 5: КРИТИЧНИ НАСТРОЙКИ В `TSCONFIG.JSON`

| Настройка в `compilerOptions` | Препоръчителна стойност | Защо е критична |
| :--- | :--- | :--- |
| **`"strict"`** | `true` | Активира всички строги проверки (`strictNullChecks`, `noImplicitAny` и др.) |
| **`"noUncheckedIndexedAccess"`** | `true` | Прави `arr[0]` тип `T \| undefined` вместо `T` — спасява от `Cannot read properties of undefined`! |
| **`"exactOptionalPropertyTypes"`** | `true` | Разграничава липсващо свойство от свойство с изрична стойност `undefined` |
| **`"noImplicitReturns"`** | `true` | Хваща функции, които връщат стойност в `if`, но забравят `return` в `else` |
| **`"moduleResolution"`** | `"bundler"` или `"NodeNext"` | Модерно резолвиране на ES модули (Vite, Next.js, Node 20+) |

---

---

## 6. КАТЕГОРИЯ 6: ВАЖНИ БИБЛИОТЕКИ В TYPESCRIPT ЕКОСИСТЕМАТА (ИМПОРТ, ИЗВИКВАНЕ И ПРИЛОЖЕНИЕ)

| Библиотека | Инсталация | Импорт и Извикване (Синтаксис) | За какво се използва |
| :--- | :--- | :--- | :--- |
| **`zod`** | `npm i zod` | `import { z } from "zod";`<br>`const User = z.object({ id: z.number() });`<br>`const parsed = User.parse(json);` | Валидация на данни в Runtime с автоматично извличане на TypeScript тип (`z.infer<typeof User>`). |
| **`axios`** / `fetch` | `npm i axios` | `import axios from "axios";`<br>`const { data } = await axios.get<User[]>("/api/users");` | Типизирани HTTP заявки с интерсептори, автоматичен JSON парсинг и таймаути. |
| **`date-fns`** | `npm i date-fns` | `import { format, addDays } from "date-fns";`<br>`const nextWeek = format(addDays(new Date(), 7), "yyyy-MM-dd");` | Лека, модулна (Tree-shakeable) библиотека за форматиране и изчисления с дати. |
| **`prisma`** | `npm i -D prisma && npm i @prisma/client` | `import { PrismaClient } from "@prisma/client";`<br>`const users = await prisma.user.findMany();` | Типово-безопасен ORM за Node.js/TypeScript с автоматично генерирани типове от схема. |
| **`vitest`** | `npm i -D vitest` | `import { describe, it, expect } from "vitest";`<br>`expect(sum(2, 2)).toBe(4);` | Свръхбърз Unit & Integration тестови фреймуърк с нативна поддръжка на TypeScript. |
| **`playwright`** | `npm i -D @playwright/test` | `import { test, expect } from "@playwright/test";`<br>`await page.goto("https://example.com");` | Модерен инструмент за End-to-End UI и API автоматизация в реални браузъри. |
| **`dotenv`** | `npm i dotenv` | `import "dotenv/config";`<br>`const port = process.env.PORT ?? "3000";` | Зареждане на конфигурационни променливи от `.env` файлове в `process.env`. |

---
[⬅️ 02-09. TypeScript Оптимизации](02-09-Performance-And-Optimizations.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-11. Common Errors And Fixes](02-11-Common-Errors-And-Fixes.md)
