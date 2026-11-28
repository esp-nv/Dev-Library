[⬅️ 02-01-00. Basic Types, Interfaces & Type Aliases](02-01-00-Basic-Types-And-Interfaces.md) | [📂 Назад към TypeScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [➡️ 02-02-00. Advanced Types: Обединения, Type Guards, Т...](02-02-00-Advanced-Types-And-Guards.md)

---

# 02-01-01. Basic Types & Interfaces: Синтаксис от Нулата & Cheat Sheet

**Раздел 01:** TypeScript Core & Type System | **Цел:** Точни синтактични шаблони от нулата за примитивни типове, масиви, функции, `interface` (за обекти, функции и наследяване), работа с `unknown` и `never`, plus обобщена 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Примитивни типове, Масиви и Type Inference](#1-примитивни-типове-масиви-и-type-inference)
* [2. Типизиране на Функции, Optional & Default параметри и `never`](#2-типизиране-на-функции-optional--default-параметри-и-never)
* [3. Интерфейси (Обекти, Наследяване, Функции и Index Signatures)](#3-интерфейси-обекти-наследяване-функции-и-index-signatures)
* [4. Безопасна работа с `unknown` и `JSON.parse()` (Вход / Изход)](#4-безопасна-работа-с-unknown-и-jsonparse-вход--изход)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПРИМИТИВНИ ТИПОВЕ, МАСИВИ И TYPE INFERENCE

```typescript
// 1. Основни примитиви (винаги с малки букви: string, number, boolean!)
let courseTitle: string = "TypeScript Fundamentals";
let maxPoints: number = 100;
let price: number = 49.99;          // В TS няма int/double — всички числа са 'number'
let isCompleted: boolean = true;

// 2. Type Inference (компилаторът сам определя типа 'string')
let inferredCity = "Видин";
// inferredCity = 123;              // ❌ Грешка: Type 'number' is not assignable to type 'string'

// 3. Типизирани масиви (Два еквивалентни синтаксиса)
let scores: number[] =;
let tags: Array<string> = ["TS", "QA", "FullStack"];

// 4. Неизменяем масив (блокира .push() и .pop())
const lockedRoles: readonly string[] = ["Admin", "User"];
```

---

## 2. ТИПИЗИРАНЕ НА ФУНКЦИИ, OPTIONAL & DEFAULT ПАРАМЕТРИ И `NEVER`

```typescript
// 1. Стандартна функция с изрични типове на входа и изхода
function calculateTax(amount: number, taxRate: number = 0.20): number {
    return amount * taxRate;
}

// 2. Опционален параметър (?) — задължително стои след задължителните параметри
function formatGreeting(name: string, title?: string): string {
    return title ? `Здравей, ${title} ${name}!` : `Здравей, ${name}!`;
}

// 3. Функция без върната стойност (void)
function logInfo(message: string): void {
    console.log(`[INFO]: ${message}`);
}

// 4. Функция, която никога не връща нормално (never)
function throwCriticalError(message: string): never {
    throw new Error(message);
}
```

---

## 3. ИНТЕРФЕЙСИ (ОБТЕКТИ, НАСЛЕДЯВАНЕ, ФУНКЦИИ И INDEX SIGNATURES)

```typescript
// 1. Базов интерфейс с readonly и опционално (?) свойство
interface IBaseEntity {
    readonly id: number;
    createdAt: Date;
}

// 2. Наследяване на интерфейс чрез 'extends'
interface IUser extends IBaseEntity {
    username: string;
    email: string;
    phone?: string;
}

// 3. Интерфейс, описващ сигнатура на функция
interface MathOperation {
    (a: number, b: number): number;
}
const add: MathOperation = (x, y) => x + y;

// 4. Index Signature (за обекти с динамични ключове)
interface StringDictionary {
    [key: string]: string;
}
const headers: StringDictionary = {
    "Content-Type": "application/json",
    "Authorization": "Bearer token123"
};
```

---

## 4. БЕЗОПАСНА РАБОТА С `UNKNOWN` И `JSON.PARSE()` (ВХОД / ИЗХОД)

* **Вход (Суров JSON от мрежата):**
  ```typescript
  const rawJson: string = '{"username": "elena_dev", "level": 2}';
  const parsedData: unknown = JSON.parse(rawJson);

  if (
      typeof parsedData === "object" &&
      parsedData !== null &&
      "username" in parsedData &&
      typeof (parsedData as { username: unknown }).username === "string"
  ) {
      console.log(`Потребител: ${(parsedData as { username: string }).username.toUpperCase()}`);
  }
  ```
* **Изход в конзолата:**
  ```text
  Потребител: ELENA_DEV
  ```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Тип | Синтаксис в TypeScript | Пример и Бележка |
| :--- | :--- | :--- |
| **Примитивни типове** | `string` \| `number` \| `boolean` | Винаги с малки букви (`String` с главна е JS wrapper обект — избягвай!). |
| **Масив / Readonly масив** | `T[]` \| `readonly T[]` | `const nums: number[] = [1, 2, 3];` |
| **Опционално / Readonly поле** | `prop?: string` \| `readonly id: number` | `?` позволява `undefined`; `readonly` забранява презаписване. |
| **Сигурен неизвестен тип** | `let data: unknown;` | Изисква проверка с `typeof` / `instanceof` преди употреба. |
| **Функция без резултат / Срив** | `: void` \| `: never` | `void` = завършва без стойност; `never` = хвърля грешка или безкраен цикъл. |
| **Интерфейс и наследяване** | `interface B extends A { ... }` | Описва формата на обект или клас (изтрива се в Runtime). |
| **Компилация и Стартиране** | `npx tsc` \| `npx ts-node app.ts` | `tsc` генерира `.js` файлове; `ts-node` изпълнява директно в конзолата. |







---

[⬅️ 02-01-00. Basic Types, Interfaces & Type Aliases](02-01-00-Basic-Types-And-Interfaces.md) | [📂 Назад към TypeScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [➡️ 02-02-00. Advanced Types: Обединения, Type Guards, Т...](02-02-00-Advanced-Types-And-Guards.md)

© 2026 Dev-Library / 02-01-01. Basic Types & Interfaces: Синтаксис от Нулата & Cheat Sheet
