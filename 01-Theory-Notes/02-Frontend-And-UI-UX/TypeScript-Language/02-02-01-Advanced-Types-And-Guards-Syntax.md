[⬅️ 02-02-00. Advanced Types, Guards & Utility Types](02-02-00-Advanced-Types-And-Guards.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-03-00. OOP in TypeScript](02-03-00-OOP-In-TypeScript.md)

---

# 02-02-01. Advanced Types & Utility Cheat Sheet (Синтаксис от Нулата)

**Раздел 01:** TypeScript Core & Type System | **Цел:** Практически шаблони от нулата за **Type Predicates (`val is string`)**, операторите **`as const`**, **`satisfies`**, **`keyof`**, **`in`** и **`typeof`**, **Mapped и Recursive Types**, вградените **Utility Types** (`Partial`, `Pick`, `Omit`, `Readonly`, `Record`) и 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Потребителски Type Predicates (`val is T`) и `in` Guard](#1-потребителски-type-predicates-val-is-t-и-in-guard)
* [2. Модерни оператори: `as const`, `satisfies`, `keyof` и `typeof`](#2-модерни-оператори-as-const-satisfies-keyof-и-typeof)
* [3. Mapped Types, Indexed Access Types и Recursive Types](#3-mapped-types-indexed-access-types-и-recursive-types)
* [4. Вградени Utility Types в действие (Вход / Изход трансформации)](#4-вградени-utility-types-в-действие-вход--изход-трансформации)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПОТРЕБИТЕЛСКИ TYPE PREDICATES (`VAL IS T`) И `IN` GUARD

```typescript
// 1. Потребителски Type Predicate (връща boolean, но убеждава компилатора за типа)
function isString(val: unknown): val is string {
    return typeof val === "string";
}

let incomingData: unknown = "SoftUni TypeScript";
if (isString(incomingData)) {
    console.log(incomingData.toUpperCase()); // ✅ 100% сигурно!
}

// 2. Разграничаване на обектни типове с оператора 'in'
type Cat = { meow: () => void };
type Dog = { bark: () => void };

function makeNoise(pet: Cat | Dog): void {
    if ("bark" in pet) {
        pet.bark(); // TypeScript знае, че pet е Dog
    } else {
        pet.meow(); // TypeScript знае, че pet е Cat
    }
}
```

---

## 2. МОДЕРНИ ОПЕРАТОРИ: `AS CONST`, `SATISFIES`, `KEYOF` И `TYPEOF`

```typescript
// 1. 'as const' — заключва обекта като неизменяем (readonly) с точни литерални стойности
const appConfig = {
    env: "production",
    port: 8080
} as const;
// appConfig.port = 3000; // ❌ Грешка: Cannot assign to 'port' because it is a read-only property

// 2. 'satisfies' — валидира спрямо договор, без да губи конкретните типове на полетата
type UserContract = { id: number; name: string };

const userObj = {
    id: 1,
    name: "Елизабет",
    role: "Admin"
} satisfies UserContract; // ✅ Проверява id и name, но пази и достъпа до userObj.role!

// 3. 'keyof' и 'typeof' на ниво типове
type Point = { x: number; y: number };
type PointKeys = keyof Point; // Резултат: "x" | "y"

const samplePoint = { x: 10, y: 20 };
type ExtractedPointType = typeof samplePoint; // Резултат: { x: number; y: number }
```

---

## 3. MAPPED TYPES, INDEXED ACCESS TYPES И RECURSIVE TYPES

```typescript
// 1. Indexed Access Type (извлича типа на конкретно поле)
type Person = { name: string; age: number };
type PersonNameType = Person["name"]; // string

// 2. Mapped Type (обхожда всички ключове на T и създава нов тип)
type MakeReadonly<T> = {
    readonly [K in keyof T]: T[K];
};

// 3. Recursive Type (саморефериращ се тип за дървовидни структури и менюта)
type TreeNode = {
    value: number;
    left?: TreeNode;
    right?: TreeNode;
};
```

---

## 4. ВГРАДЕНИ UTILITY TYPES В ДЕЙСТВИЕ (ВХОД / ИЗХОД ТРАНСФОРМАЦИИ)

* **Вход (Базов интерфейс):**
  ```typescript
  interface IStudentEvaluation {
      id: string;
      studentName: string;
      score: number;
      notes?: string;
      createdAt: Date;
  }

  // Генериране на специализирани DTO типове без дублиране на код:
  type EvaluationPatchDto = Partial<IStudentEvaluation>;
  type EvaluationLogToken = Omit<IStudentEvaluation, "notes" | "createdAt">;
  type EvaluationHeader   = Pick<IStudentEvaluation, "id" | "studentName">;
  type LockedEvaluation   = Readonly<IStudentEvaluation>;
  ```
* **Изход (Резултатни типове в паметта на компилатора):**
  * `EvaluationLogToken` ➔ `{ id: string; studentName: string; score: number }`
  * `EvaluationHeader` ➔ `{ id: string; studentName: string }`
  * `EvaluationPatchDto` ➔ Всички 5 полета стават опционални (`?`).

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Оператор | Синтаксис | Пример и Приложение |
| :--- | :--- | :--- |
| **Union / Intersection** | `A \| B` \| `A & B` | `type Id = number \| string;` / `type Admin = User & { rights: string[] };` |
| **Tuple (Кортеж)** | `[Type1, Type2]` | `type ApiResult = [boolean, string];` (фиксиран брой и ред). |
| **Type Predicate** | `(x: unknown): x is T` | Потребителска функция за Runtime проверка на тип. |
| **`as const` / `satisfies`** | `obj as const` \| `obj satisfies T` | `as const` = deep readonly литерали; `satisfies` = проверка без загуба на тип. |
| **`keyof` / `typeof`** | `keyof T` \| `typeof variable` | `keyof` вади ключовете като Union; `typeof` вади типа от променлива. |
| **`Partial<T>` / `Required<T>`** | `Partial<User>` | Прави всички полета опционални (`?`) или задължителни. |
| **`Pick<T, K>` / `Omit<T, K>`** | `Omit<User, "password">` | Избира само посочените полета или изтрива ненужните. |
| **`Record<K, V>`** | `Record<string, number>` | Създава типизиран речник (Key-Value обект). |

---

[⬅️ 02-02-00. Advanced Types, Guards & Utility Types](02-02-00-Advanced-Types-And-Guards.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-03-00. OOP in TypeScript](02-03-00-OOP-In-TypeScript.md)

© 2026 Dev-Library / 02-02-01. Advanced Types & Utility Cheat Sheet (Синтаксис от Нулата)
