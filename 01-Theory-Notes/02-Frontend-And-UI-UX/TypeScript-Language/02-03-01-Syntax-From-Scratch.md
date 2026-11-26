[⬅️ 02-03-00. OOP in TypeScript](02-03-00-OOP-In-TypeScript.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-04-00. Mastering Generics & Constraints](02-04-00-Mastering-Generics.md)

---

# 02-03-01. OOP in TypeScript: Синтаксис от Нулата & Cheat Sheet

**Раздел 02:** Object-Oriented TypeScript & Generics | **Цел:** Практически шаблони от нулата за класове, `get` / `set` аксесори, статични членове (`static`), **Полиморфизъм (Strategy Pattern)**, **Singleton Pattern**, **Method Overloads**, наследяване с `override`, интерфейси наследяващи класове и 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Класове, Getters/Setters, Static членове и `#private` полета](#1-класове-getterssetters-static-членове-и-private-полета)
* [2. Наследяване с `extends`, `super` и `override`](#2-наследяване-с-extends-super-и-override)
* [3. Полиморфизъм в действие (Strategy Pattern) & Singleton Pattern](#3-полиморфизъм-в-действие-strategy-pattern--singleton-pattern)
* [4. Method Overloads и Интерфейс, наследяващ клас](#4-method-overloads-и-интерфейс-наследяващ-клас)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. КЛАСОВЕ, GETTERS/SETTERS, STATIC ЧЛЕНОВЕ И `#PRIVATE` ПОЛЕТА

```typescript
class BankAccount {
    public static readonly currency: string = "EUR";
    #balance: number; // Истинско Runtime частно поле (заключено в V8)

    constructor(
        public readonly iban: string,
        initialBalance: number
    ) {
        this.#balance = initialBalance;
    }

    // Getter за контролирано четене
    public get balance(): number {
        return this.#balance;
    }

    // Setter с бизнес валидация
    public set balance(value: number) {
        if (value < 0) {
            throw new Error("Балансът не може да бъде отрицателен!");
        }
        this.#balance = value;
    }

    public deposit(amount: number): void {
        if (amount > 0) this.#balance += amount;
    }
}

const acc = new BankAccount("BG80BNBG96611020345678", 500);
acc.deposit(250);
console.log(`IBAN: ${acc.iban} | Наличност: ${acc.balance} ${BankAccount.currency}`);
```

---

## 2. НАСЛЕДЯВАНЕ С `EXTENDS`, `SUPER` И `OVERRIDE`

При включен флаг `"noImplicitOverride": true` в `tsconfig.json`, TypeScript изисква ключовата дума **`override`** при пренаписване на базов метод (точно като в C#), предпазвайки от правописни грешки в името на метода:

```typescript
class BaseLogger {
    public log(message: string): void {
        console.log(`[BASE]: ${message}`);
    }
}

class FileLogger extends BaseLogger {
    public override log(message: string): void {
        super.log(message); // Извиква оригиналния метод на родителя
        console.log(`[FILE WRITE]: Записано във файл.`);
    }
}
```

---

## 3. ПОЛИМОРФИЗЪМ В ДЕЙСТВИЕ (STRATEGY PATTERN) & SINGLETON PATTERN

### 🔹 А. Полиморфизъм чрез интерфейси (Strategy Pattern)
```typescript
interface IPaymentProcessor {
    pay(amount: number): string;
}

class CardPayment implements IPaymentProcessor {
    public pay(amount: number): string {
        return `Платени ${amount} EUR с кредитна карта.`;
    }
}

class CryptoPayment implements IPaymentProcessor {
    public pay(amount: number): string {
        return `Платени ${amount} EUR чрез USDC портфейл.`;
    }
}

// Една и съща функция работи полиморфно с всяка бъдеща имплементация на IPaymentProcessor
function executeCheckout(processor: IPaymentProcessor, total: number): void {
    console.log(processor.pay(total));
}

executeCheckout(new CardPayment(), 120);
executeCheckout(new CryptoPayment(), 250);
```

### 🔹 Б. Singleton Pattern (Единствена инстанция в паметта)
```typescript
class AppConfiguration {
    private static instance: AppConfiguration;
    public readonly apiUrl: string = "https://devlib.local";

    // Частен конструктор — забранява директно извикване на 'new AppConfiguration()' отвън
    private constructor() {}

    public static getInstance(): AppConfiguration {
        if (!AppConfiguration.instance) {
            AppConfiguration.instance = new AppConfiguration();
        }
        return AppConfiguration.instance;
    }
}

const config1 = AppConfiguration.getInstance();
const config2 = AppConfiguration.getInstance();
console.log(config1 === config2); // Изход: true (една и съща референция в Heap паметта)
```

---

## 4. METHOD OVERLOADS И ИНТЕРФЕЙС, НАСЛЕДЯВАЩ КЛАС

```typescript
// 1. Method Overloads (Множество сигнатури + 1 обща имплементация)
class Greeter {
    greet(age: number): string;
    greet(firstName: string, lastName: string): string;

    greet(a: number | string, b?: string): string {
        if (typeof a === "number") {
            return `Ти си на ${a} години.`;
        }
        return `Здравей, ${a} ${b}!`;
    }
}

// 2. Интерфейс, наследяващ клас
class Hardware {
    public ram: number = 16;
}

interface LaptopSpecs extends Hardware {
    cpu: string;
}

const devMachine: LaptopSpecs = { ram: 32, cpu: "Intel Core Ultra 7" };
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| ООП Конструкция | Синтаксис в TypeScript | Бележка и Паралел с C# |
| :--- | :--- | :--- |
| **Parameter Property** | `constructor(public readonly id: number)` | Автоматично създава и попълва полето в класа. |
| **Истинско частно поле** | `#secret: string;` | Остава заключено и в Runtime (за разлика от `private`). |
| **Имплементация на интерфейс** | `class UserRepo implements IRepo { ... }` | Компилаторът проверява всички методи и свойства. |
| **Абстрактен клас / метод** | `abstract class Base { abstract run(): void; }` | Не може да се инстанцира директно с `new`. |
| **Пренаписване на метод** | `public override toString(): string` | Изисква `"noImplicitOverride": true` в `tsconfig.json`. |
| **Definite Assignment** | `private _service!: ApiService;` | Убеждава компилатора, че полето ще получи стойност. |
| **Method Overloads** | Множество сигнатури + 1 общо тяло | Външният код вижда само конкретните Overload сигнатури. |
| **Заключване на `this`** | `handler = () => { ... }` | Запазва контекста на инстанцията при подаване като Callback. |

---

[⬅️ 02-03-00. OOP in TypeScript](02-03-00-OOP-In-TypeScript.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-04-00. Mastering Generics & Constraints](02-04-00-Mastering-Generics.md)

© 2026 Dev-Library / 02-03-01. OOP in TypeScript: Синтаксис от Нулата & Cheat Sheet