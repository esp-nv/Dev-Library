[⬅️ 02-05-00. Decorators, Modules & Async TypeScript](02-05-00-Decorators-Modules-And-Async.md) | [📂 Текуща папка: 02. TypeScript Language](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 03. Python Scripting](../03-Python-Scripting/README.md) | [➡️ 02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md)

---

# 02-05-01. Decorators, Modules & Async TypeScript: Синтаксис от Нулата & Cheat Sheet

**Раздел 03:** Decorators, Modules & Async TypeScript | **Цел:** Практически шаблони от нулата за **Class Decorator (`@AddTimestamp`)**, **Композиция на декоратори**, проверка на типове в Runtime чрез **`reflect-metadata` (`@ValidateType`)**, модулен синтаксис (`import type` / `export`), типизиран `async/await` (`Promise<T>`) и 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Class Decorator (`@AddTimestamp`) и Композиция на декоратори](#1-class-decorator-addtimestamp-и-композиция-на-декоратори)
* [2. Runtime проверка на типове чрез `reflect-metadata` (`@ValidateType`)](#2-runtime-проверка-на-типове-чрез-reflect-metadata-validatetype)
* [3. ES Modules (`import type`), `.d.ts` тапи и Типизиран `async / await`](#3-es-modules-import-type-dts-тапи-и-типизиран-async--await)
* [4. Обобщение / Бърз Cheat Sheet](#4-обобщение--бърз-cheat-sheet)

---

## 1. CLASS DECORATOR (`@ADDTIMESTAMP`) И КОМПОЗИЦИЯ НА ДЕКОРАТОРИ

```typescript
// 1. Class Decorator, който разширява конструктора и добавя ново свойство createdAt
function AddTimestamp<T extends new (...args: any[]) => {}>(constructor: T) {
    return class extends constructor {
        createdAt = new Date().toISOString();
    };
}

@AddTimestamp
class Order {
    constructor(public id: number) {}
}

// Declaration Merging: уведомяваме TypeScript компилатора за добавеното поле
interface Order {
    createdAt: string;
}

const myOrder = new Order(101);
console.log(`Поръчка #${myOrder.id} създадена на: ${myOrder.createdAt}`);

// 2. Демонстрация на реда при композиция на множество декоратори
function dec1() {
    console.log("1. Фабрика dec1 оценена (отгоре надолу)");
    return function (target: any, key: string, descriptor: PropertyDescriptor) {
        console.log("4. Декоратор dec1 приложен (отдолу нагоре)");
    };
}

function dec2() {
    console.log("2. Фабрика dec2 оценена (отгоре надолу)");
    return function (target: any, key: string, descriptor: PropertyDescriptor) {
        console.log("3. Декоратор dec2 приложен (отдолу нагоре)");
    };
}

class PipelineDemo {
    @dec1()
    @dec2()
    execute() {}
}
```

---

## 2. RUNTIME ПРОВЕРКА НА ТИПОВЕ ЧРЕЗ `REFLECT-METADATA` (`@VALIDATETYPE`)

Изисква `npm install reflect-metadata` и `"emitDecoratorMetadata": true` в `tsconfig.json`:

```typescript
import "reflect-metadata";

class Address {
    constructor(public city: string) {}
}

function ValidateType(target: any, propertyKey: string, descriptor: PropertyDescriptor) {
    const originalSet = descriptor.set!;

    descriptor.set = function (value: any) {
        // Извлича очаквания тип от метаданните на компилатора в Runtime
        const expectedType = Reflect.getMetadata("design:type", target, propertyKey);

        if (!(value instanceof expectedType)) {
            throw new TypeError(`Грешка: Очакван тип ${expectedType.name}!`);
        }

        originalSet.call(this, value);
    };
}

class Person {
    private _address!: Address;

    constructor(address: Address) {
        this._address = address;
    }

    @ValidateType
    set address(value: Address) {
        this._address = value;
    }

    get address(): Address {
        return this._address;
    }
}

const person = new Person(new Address("Видин"));
person.address = new Address("София"); // ✅ Валидно
// person.address = { city: "Варна" } as any; // ❌ Хвърля Runtime TypeError: Очакван тип Address!
```

---

## 3. ES MODULES (`IMPORT TYPE`), `.D.TS` ТАПИ И ТИПИЗИРАН `ASYNC / AWAIT`

```typescript
// 1. Импортиране само на типове (изчезва на 100% в генерирания JS файл)
import type { IProduct } from './models/product.model';

// 2. Декларационен файл (declarations.d.ts) за външна нетипизирана JS библиотека
// declare module 'legacy-analytics-lib';

// 3. Типизирана асинхронна заявка с Promise<T> и безопасен catch (error: unknown)
async function fetchProductById(id: number): Promise<IProduct | null> {
    try {
        const response = await fetch(`https://devlib.local{id}`);
        if (!response.ok) {
            throw new Error(`HTTP грешка! Статус: ${response.status}`);
        }
        const data = (await response.json()) as IProduct;
        return data;
    } catch (error: unknown) {
        if (error instanceof Error) {
            console.error(`[API Error]: ${error.message}`);
        }
        return null;
    }
}
```

---

## 4. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Инструмент | Синтаксис в TypeScript | Предназначение и Бележка |
| :--- | :--- | :--- |
| **Активиране на декоратори** | `"experimentalDecorators": true` | Задава се в `compilerOptions` на `tsconfig.json`. |
| **Metadata в Runtime** | `"emitDecoratorMetadata": true` | Заедно с `import "reflect-metadata";` позволява `Reflect.getMetadata(...)`. |
| **Method Decorator** | `(target, key, descriptor) => { ... }` | Обвива и модифицира `descriptor.value` (AOP прихващане). |
| **Type-Only Import** | `import type { UserDto } from './dto';` | Гарантира, че импортът ще се изтрие изцяло при компилация. |
| **Ambient Declaration** | `declare module 'old-js-pkg';` | Премахва грешката за липсващи `.d.ts` типове при стари npm пакети. |
| **Типизирана Async функция** | `async function load(): Promise<User[]>` | Всяка `async` функция задължително връща `Promise<T>`. |
| **Безопасен `catch` блок** | `catch (err: unknown) { if (err instanceof Error) ... }` | Стандарт в TypeScript 4.4+ при включен `strict: true`. |

---

[⬅️ 02-05-00. Decorators, Modules & Async TypeScript](02-05-00-Decorators-Modules-And-Async.md) | [📂 Текуща папка: 02. TypeScript Language](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 03. Python Scripting](../03-Python-Scripting/README.md) | [➡️ 02-96. Performance & Optimizations](02-96-Performance-And-Optimizations.md)

© 2026 Dev-Library / 02-05-01. Decorators, Modules & Async TypeScript: Синтаксис от Нулата & Cheat Sheet
