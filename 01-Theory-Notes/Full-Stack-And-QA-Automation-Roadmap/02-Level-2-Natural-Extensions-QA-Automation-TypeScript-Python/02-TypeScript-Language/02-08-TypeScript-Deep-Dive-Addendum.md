[⬅️ 02-05. TypeScript Декоратори](02-05-TypeScript-Decorators.md) | 📂 Папка: 2-TypeScript-Language/Lectures | [🏠 Начало](../../../README.md) | [➡️ 02-06. Инженерен Справочник](../Interview-Handbook/02-06-TypeScript-Traps-And-Snippets.md)

---

# 02-08. TypeScript Deep Dive: Инсталация, Настройка и Напреднали Концепции

**Модул 02:** TypeScript | **Цел:** Пълна настройка на работната среда (`tsc`, `ts-node`, `tsconfig.json`, VS Code Debugging) и овладяване на напредналите оператори, типови трансформации и метапрограмиране от лекциите.

## 🧭 Съдържание на темата
* [1. Инсталация, компилация и настройка на средата (VS Code & Node.js)](#1-инсталация-компилация-и-настройка-на-средата-vs-code--nodejs)
* [2. Допълнения към Основни типове и Type Safety (`never`, Type Predicates)](#2-допълнения-към-основни-типове-и-type-safety-never-type-predicates)
* [3. Напреднали оператори и Типови трансформации (`satisfies`, `as const`, `keyof`, Mapped Types)](#3-напреднали-оператори-и-типови-трансформации-satisfies-as-const-keyof-mapped-types)
* [4. ООП разширения (Method Overloads, `override` и `!` Assertion)](#4-ооп-разширения-method-overloads-override-и--assertion)
* [5. Напреднали Дженерици (Conditional Types & Филтриране с `never`)](#5-напреднали-дженерици-conditional-types--филтриране-с-never)
* [6. Декоратори в дълбочина (Композиция и `reflect-metadata`)](#6-декоратори-в-дълбочина-композиция-и-reflect-metadata)

---

## 1. ИНСТАЛАЦИЯ, КОМПИЛАЦИЯ И НАСТРОЙКА НА СРЕДАТА (VS CODE & NODE.JS)

За да работиш с TypeScript локално, първо трябва да имаш инсталиран **Node.js**.

### А. Инсталация и инициализация (Терминал)
```bash
# 1. Глобална инсталация на TypeScript компилатора
npm install -g typescript

# 2. Проверка на версията (трябва да върне Version 5.x.x)
tsc --version

# 3. Създаване на конфигурационен файл tsconfig.json в папката на проекта
tsc --init
```

### Б. Изпълнение на кода (Два начина)
1. **Чрез транспилация (стандартен начин):**
   ```bash
   npx tsc               # Превежда всички .ts файлове в .js (в папка dist/ или out/)
   node out/index.js     # Изпълнява генерирания JavaScript файл през Node.js
   ```
2. **Директно изпълнение без ръчен билд (`ts-node`):**
   ```bash
   npm install -D ts-node    # Инсталира се като dev зависимост
   npx ts-node index.ts      # Стартира .ts файла директно в конзолата
   ```

### В. Настройка на `tsconfig.json` (за дебъгване и декоратори)
*Забележка: В реалния `tsconfig.json` изтрий коментарите, тъй като стандартният JSON не ги поддържа.*
```json
{
  "compilerOptions": {
    "target": "esnext",
    "module": "esnext",
    "sourceMap": true,                   // Задължително за Breakpoints във VS Code!
    "strict": true,                      // Включва всички строги проверки
    "noImplicitOverride": true,          // Изисква думата 'override' при пренаписване
    "experimentalDecorators": true,      // Активира декораторите (@)
    "emitDecoratorMetadata": true,       // Активира метаданните за reflect-metadata
    "outDir": "out"                      // Папката, в която влизат .js файловете
  }
}
```

### Г. Настройка на VS Code Debugger (`launch.json`)
Отвори таба **Run and Debug** във VS Code ➔ натисни **create a launch.json file** ➔ избери **Node.js** и постави следната конфигурация, за да можеш да слагаш червени точки (Breakpoints) директно в `.ts` файловете:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "node",
      "request": "launch",
      "name": "Launch TypeScript File",
      "program": "${workspaceFolder}/${fileBasename}",
      "preLaunchTask": "tsc: build - tsconfig.json",
      "outFiles": ["${workspaceFolder}/out/**/*.js"]
    }
  ]
}
```

---

## 2. ДОПЪЛНЕНИЯ КЪМ ОСНОВНИ ТИПОВЕ И TYPE SAFETY (`NEVER`, TYPE PREDICATES)

### А. Типът `never`
Използва се за функции, които **никога не връщат стойност нормално** (винаги хвърлят грешка или въртят безкраен цикъл), както и за недостижим код.
```typescript
function throwCriticalError(message: string): never {
    throw new Error(message);
}

function infiniteServerLoop(): never {
    while (true) {
        // Сървърът слуша постоянно...
    }
}
```

### Б. Потребителски Type Predicates (`val is string`)
Когато имаш променлива от тип `unknown`, можеш да напишеш специална функция за проверка, която убеждава компилатора какъв е точният тип вътре в `if` блока.
```typescript
function isString(val: unknown): val is string {
    return typeof val === "string";
}

let incomingData: unknown = "SoftUni TypeScript";

if (isString(incomingData)) {
    // Тук TypeScript вече знае на 100%, че incomingData е string!
    console.log(incomingData.toUpperCase());
}
```

---

## 3. НАПРЕДНАЛИ ОПЕРАТОРИ И ТИПОВИ ТРАНСФОРМАЦИИ (`SATISFIES`, `AS CONST`, `KEYOF`, MAPPED TYPES)

### А. `as const` и `satisfies`
* **`as const`:** Заключва всички вложени стойности в обект или масив като неизменяеми (`readonly`) точни литерали.
* **`satisfies`:** Проверява дали даден обект отговаря на определен тип, **без да губи** конкретната информация за точните стойности вътре.
```typescript
// 1. as const
const config = {
    env: "production",
    port: 8080
} as const;
// config.port = 3000; -> ❌ Грешка: Cannot assign to 'port' because it is a read-only property

// 2. satisfies
type UserContract = { id: number; name: string };

const userObj = {
    id: 1,
    name: "Елизабет",
    role: "Admin" // Допълнително свойство
} satisfies UserContract; // ✅ Проверява, че има id и name, но пази и достъпа до userObj.role!
```

### Б. Операторите `keyof`, `in` и `typeof` на ниво типове
```typescript
type Point = { x: number; y: number };

// keyof извлича ключовете като обединение: "x" | "y"
type PointKeys = keyof Point; 

// typeof извлича типа от вече създадена променлива
let samplePoint = { x: 10, y: 20 };
type ExtractedPointType = typeof samplePoint; // { x: number; y: number }

// 'in' като Type Guard за проверка на свойство
type Cat = { meow: () => void };
type Dog = { bark: () => void };

function makeNoise(pet: Cat | Dog) {
    if ("bark" in pet) {
        pet.bark(); // TypeScript знае, че pet е Dog
    }
}
```

### В. Mapped Types, Indexed Access Types и Recursive Types
```typescript
// 1. Indexed Access Type (взима типа на конкретно поле)
type Person = { name: string; age: number };
type PersonNameType = Person["name"]; // string

// 2. Mapped Type (създава нов тип, обхождайки ключовете на друг)
type MakeReadonly<T> = {
    readonly [K in keyof T]: T[K];
};

// 3. Recursive Type (за дървовидни структури)
type TreeNode = {
    value: number;
    left?: TreeNode;
    right?: TreeNode;
};
```

### Г. Интерфейси за функции и наследяване на класове
```typescript
// 1. Интерфейс, описващ функция
interface MathOperation {
    (a: number, b: number): number;
}
const add: MathOperation = (x, y) => x + y;

// 2. Интерфейс, наследяващ клас
class Hardware {
    public ram: number = 16;
}
interface LaptopSpecs extends Hardware {
    cpu: string;
}
```

---

## 4. ООП РАЗШИРЕНИЯ (METHOD OVERLOADS, `OVERRIDE` И `!` ASSERTION)

* **Definite Assignment Assertion (`!`):** Слага се след името на поле в клас (`private _name!: string;`), за да каже на компилатора: *„Сигурен съм, че това поле ще получи стойност преди да се използва, не хвърляй грешка“*.
* **Ключовата дума `override`:** Изрично маркира, че методът пренаписва метод от родителския клас (предпазва от правописни грешки, ако в `tsconfig.json` е включено `"noImplicitOverride": true`).
* **Method Overloads:** Позволява дефиниране на няколко валидни комбинации от аргументи над една обща имплементация.

```typescript
class Greeter {
    // Декларации на допустимите начини за викане (Overloads)
    greet(age: number): string;
    greet(firstName: string, lastName: string): string;
    
    // Обща имплементация (покрива всички горни случаи)
    greet(a: number | string, b?: string): string {
        if (typeof a === "number") {
            return `Ти си на ${a} години.`;
        }
        return `Здравей, ${a} ${b}!`;
    }
}

const g = new Greeter();
console.log(g.greet(25));               // ✅ Валидно
console.log(g.greet("Иван", "Петров")); // ✅ Валидно
// g.greet("Иван");                     // ❌ Грешка при компилация: няма такава сигнатура!
```

---

## 5. НАПРЕДНАЛИ ДЖЕНЕРИЦИ (CONDITIONAL TYPES & ФИЛТРИРАНЕ С `NEVER`)

Можеш да пишеш логика на ниво типове чрез тернарния оператор `extends ? :` и да филтрираш полета чрез `never` (типовете с `never` автоматично изчезват от крайния резултат).

```typescript
// 1. Условен тип (Conditional Type)
type IsString<T> = T extends string ? "Да" : "Не";
type Check1 = IsString<string>; // "Да"
type Check2 = IsString<number>; // "Не"

// 2. Извличане САМО на имената на числовите свойства от даден клас/обект
type Employee = {
    name: string;
    age: number;
    salary: number;
};

type NumberKeysOnly<T> = {
    [K in keyof T]: T[K] extends number ? K : never;
}[keyof T];

type EmployeeNumericFields = NumberKeysOnly<Employee>; // Резултат: "age" | "salary"
```

---

## 6. ДЕКОРАТОРИ В ДЪЛБОЧИНА (КОМПОЗИЦИЯ И `REFLECT-METADATA`)

### А. Ред на изпълнение и композиция
Когато сложиш няколко декоратора един над друг (`@dec1()` най-отгоре, `@dec2()` отдолу):
1. **Фабриките** се оценяват **отгоре надолу** (`dec1` ➔ `dec2`).
2. **Самите декоратори** се прилагат **отдолу нагоре** (`dec1(dec2(target))`).
*Важно: Декораторите **не се наследяват** автоматично от дъщерните класове — всеки наследник трябва да бъде декориран поотделно.*

```typescript
function log1() {
    console.log("1. Фабрика log1 оценена");
    return function (target: any, key: string, descriptor: PropertyDescriptor) {
        console.log("4. Декоратор log1 приложен");
    };
}

function log2() {
    console.log("2. Фабрика log2 оценена");
    return function (target: any, key: string, descriptor: PropertyDescriptor) {
        console.log("3. Декоратор log2 приложен");
    };
}

class Demo {
    @log1()
    @log2()
    run() {}
}
```

### Б. Разширяване на клас чрез Class Decorator
Class Decorator може да върне нов анонимен клас, който наследява оригиналния и добавя нови свойства или променя поведението на конструктора му:

```typescript
function AddTimestamp<T extends new (...args: any[]) => {}>(constructor: T) {
    return class extends constructor {
        createdAt = new Date().toISOString();
    };
}

@AddTimestamp
class Order {
    constructor(public id: number) {}
}

// За да знае TypeScript за новото свойство, разширяваме интерфейса на класа
interface Order {
    createdAt: string;
}

const myOrder = new Order(101);
console.log(`Поръчка #${myOrder.id} създадена на: ${myOrder.createdAt}`);
```

### В. Проверка на типове в Runtime чрез `reflect-metadata`
Тъй като типовете в TypeScript изчезват след компилация, библиотеката `reflect-metadata` (заедно с `"emitDecoratorMetadata": true` в `tsconfig.json`) записва метаданни за типовете, които могат да се четат по време на изпълнение (използва се в Angular и NestJS за Dependency Injection и валидации):

```bash
npm install reflect-metadata
```

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
[⬅️ 02-05. TypeScript Декоратори](02-05-TypeScript-Decorators.md) | 📂 Папка: 2-TypeScript-Language| [🏠 Начало](../../../README.md) | [➡️ 02-06. Инженерен Справочник](02-06-TypeScript-Traps-And-Snippets.md)
