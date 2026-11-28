[⬅️ 02-01-01. Basic Types & Interfaces Cheat Sheet](02-01-01-Basic-Types-And-Interfaces-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-02-01. Advanced Types & Utility Cheat Sheet](02-02-01-Advanced-Types-And-Guards-Syntax.md)

---

# 02-02-00. Advanced Types: Обединения, Type Guards, Трансформации & Utility Types

**Раздел 01:** TypeScript Core & Type System | **Цел:** Овладяване на сложните типови конструкции в TypeScript — Union (`|`) и Intersection (`&`) типове, Tuples (кортежи), стесняване на типове (**Type Narrowing & Type Predicates `val is T`**), модерните оператори **`satisfies`**, **`as const`** и **`keyof`**, рекурсивни и Mapped типове, както и вградените **Utility Types** (`Partial`, `Pick`, `Omit`, `Readonly`, `Record`).

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: Union vs Intersection, Narrowing & satisfies)](#1-как-да-се-запомни-по-лесно-мнемоника-union-vs-intersection-narrowing--satisfies)
* [2. Архитектура на Type Narrowing и Блок-схема (Свиване на типове)](#2-архитектура-на-type-narrowing-и-блок-схема-свиване-на-типове)
* [3. Сравнителни матрици: Оператори за стесняване & Вградени Utility Types](#3-сравнителни-матрици-оператори-за-стесняване--вградени-utility-types)
* [4. Плюсове и минуси на разширените типови трансформации](#4-плюсове-и-минуси-на-разширените-типови-трансформации)
* [5. Кодов одит: Грешен (Без Type Guard) срещу Правилен (Narrowing & Literal Union) код](#5-кодов-одит-грешен-без-type-guard-срещу-правилен-narrowing--literal-union-код)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: UNION VS INTERSECTION, NARROWING & SATISFIES)

* **Union (`|`) срещу Intersection (`&`):**
  * **Union (`A | B` — „ИЛИ“):** Променливата е **или** `A`, **или** `B`. Достъпни са само методите, които са **общи и за двата типа**, докато не направиш проверка!
  * **Intersection (`A & B` — „И“):** Комбинира всички свойства на `A` и всички свойства на `B` в **един общ обект** (много прилича на наследяване).
* **Tuple (Кортеж — Твърдо заключен масив):** За разлика от обикновения масив (`string[]`), който може да има 0 или 1000 елемента, кортежът `[boolean, string]` има **строго фиксирана дължина и точен тип на всяка позиция** (като `ValueTuple` в C#).
* **`as const` срещу `satisfies` (Замразяване срещу Проверка без загуба):**
  * **`as const`** превръща целия обект или масив в **неизменяем (`readonly`)** и заковава стойностите като точни литерали (напр. `"Admin"`, а не общ `string`).
  * **`satisfies`** проверява дали обектът отговаря на даден договор, **без да изтрива** конкретната информация за точните му свойства и типове!

---

## 2. АРХИТЕКТУРА НА TYPE NARROWING И БЛОК-СХЕМА (СВИВАНЕ НА ТИПОВЕ)

Когато функция приеме Union тип (`number | string`), компилаторът стеснява (narrows) типа стъпка по стъпка през условните проверки:

```text
  Входящ параметър: id: number | string
                 │
                 ▼
       ┌───────────────────┐
       │ typeof id ===     │     ДА (True)     ┌──────────────────────────────────┐
       │     "string" ?    ├──────────────────► │ Тук id е 100% 'string'!          │
       └─────────┬─────────┘                    │ Позволява: id.trim().toUpperCase()│
                 │ НЕ (False)                   └──────────────────────────────────┘
                 ▼
       ┌───────────────────────────────────┐
       │ Тук id автоматично е 100% 'number'│
       │ Позволява: id.toFixed(2)          │
       └───────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: ОПЕРАТОРИ ЗА СТЕСНЯВАНЕ & ВГРАДЕНИ UTILITY TYPES

### 📊 А. Четирите основни Type Guards за стесняване на типове

| Механизъм | Синтаксис | Кога се използва? |
| :--- | :--- | :--- |
| **`typeof` Guard** | `if (typeof x === "string")` | За примитивни типове (`string`, `number`, `boolean`, `symbol`, `bigint`, `undefined`). |
| **`instanceof` Guard** | `if (err instanceof Error)` | За инстанции на реални класове (`Date`, `Error`, `HTMLElement` — **не** работи за `interface`!). |
| **`in` Operator Guard** | `if ("bark" in pet)` | За разграничаване на два обектни типа/интерфейса по уникално свойство. |
| **Type Predicate (`is`)** | `function isUser(x: unknown): x is IUser` | Потребителска функция за проверка в Runtime, която убеждава компилатора какъв е типът. |

### 📊 Б. Справочник на вградените Utility Types

| Utility Type | Какво прави? | Практическо приложение (C# / REST API аналог) |
| :--- | :--- | :--- |
| **`Partial<T>`** | Прави **всички** свойства на `T` опционални (`?`). | `PATCH /api/users/1` (изпращаш само променените полета). |
| **`Required<T>`** | Прави **всички** свойства на `T` задължителни (маха `?`). | Финална валидация на конфигурационен обект преди старт. |
| **`Readonly<T>`** | Маркира **всички** полета като `readonly` (неизменяеми). | Защита на State обекти в Redux / Pinia / Angular Signals. |
| **`Pick<T, Keys>`** | Създава нов тип **само** с изброените ключове. | Леко `UserSummaryDto` за списъци и таблици. |
| **`Omit<T, Keys>`** | Създава нов тип, като **изтрива** посочените ключове. | `CreateUserDto = Omit<User, "id" \| "createdAt">` (без автоматичните полета). |
| **`Record<K, V>`** | Създава обектен речник с ключове `K` и стойности `V`. | `Record<UserRole, string[]>` (мапинг на роли към права). |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА РАЗШИРЕНИТЕ ТИПОВИ ТРАНСФОРМАЦИИ

### 🟢 Предимства (Pros):
* **DRY на ниво типове (Single Source of Truth):** Вместо да поддържаш 5 отделни интерфейса (`User`, `CreateUserDto`, `UpdateUserDto`, `UserPreview`), генерираш всички автоматично от един базов модел чрез `Pick`, `Omit` и `Partial`.
* **Изчерпателна защита (Exhaustiveness Checking):** С Discriminated Unions и `never` компилаторът сигнализира веднага, ако добавиш нов статус (напр. `"Cancelled"`) и забравиш да го обработиш в `switch` блока.

### 🔴 Недостатъци (Cons):
* **Вградените `enum` генерират излишен JS код:** За разлика от `type Role = "Admin" | "User"` (което изчезва напълно след компилация), стандартният `enum` създава реален JavaScript обект с двупосочно мапиране в крайния файл.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (БЕЗ TYPE GUARD) СРЕЩУ ПРАВИЛЕН (NARROWING & LITERAL UNION) КОД

### ❌ ГРЕШЕН КОД (Извикване на специфичен метод върху Union без проверка):
```typescript
type IdType = number | string;

function processRecord(id: IdType) {
    // 🚨 КРАХ при компилация! Свойството .toUpperCase() не съществува над типа 'number'.
    return id.toUpperCase();
}
```

### 🟢 ПРАВИЛЕН КОД (Literal Union + Tuple + `typeof` Type Guard):
```typescript
type UserRole = "Admin" | "Teacher" | "Student";

type UserProfile = {
    readonly id: number | string;
    username: string;
    role: UserRole;
    phoneNumber?: string;
};

type DatabaseResponse = [boolean, string];

function authenticateUser(profile: UserProfile): DatabaseResponse {
    if (typeof profile.id === "string") {
        console.log(`Проверка на низово ID: ${profile.id.trim()}`);
    }

    if (profile.role === "Admin") {
        return [true, "Достъпът е разрешен за администраторски одит."];
    }

    return [false, "Грешка: Недостатъчни софтуерни права."];
}

const activeUser: UserProfile = {
    id: "  AUTH_8971  ",
    username: "esp_nv",
    role: "Admin"
};

const [isAuthorized, message] = authenticateUser(activeUser);
console.log(`Статус: ${isAuthorized} | Съобщение: ${message}`);
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Каква е разликата между Type Assertion (`val as string`) и Type Guard (`typeof val === 'string'`)?“*
  * **Отговор:** **Type Guard** извършва реална проверка в Runtime (докато кодът работи) и гарантира безопасност. **Type Assertion (`as`)** е само команда към компилатора (*„Повярвай ми, знам какво правя“*) — тя **не извършва никаква проверка или конвертиране в Runtime** и ако стойността всъщност е число, приложението ще гръмне пред потребителя!
* **Въпрос 2:** *„Какво представляват **Discriminated Unions** и как се използва `never` за изчерпателна проверка?“*
  * **Отговор:** Discriminated Union е обединение от обектни типове, които споделят едно общо поле-дискриминатор с уникална литерална стойност (напр. `status: "loading" | "success" | "error"`). В `default:` клона на `switch (state.status)` присвояваме променливата на тип `never` (`const _exhaustive: never = state;`). Ако в бъдеще някой добави четвърто състояние `"timeout"` и забрави да добави `case "timeout":`, TypeScript моментално спира билда с компилационна грешка!

---

[⬅️ 02-01-01. Basic Types & Interfaces Cheat Sheet](02-01-01-Basic-Types-And-Interfaces-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-02-01. Advanced Types & Utility Cheat Sheet](02-02-01-Advanced-Types-And-Guards-Syntax.md)

© 2026 Dev-Library / 02-02-00. Advanced Types: Обединения, Type Guards, Трансформации & Utility Types
