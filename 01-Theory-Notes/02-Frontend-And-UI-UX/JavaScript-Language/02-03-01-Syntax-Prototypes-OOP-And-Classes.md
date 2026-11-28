[⬅️ 02-03-00. Prototypes, Prototype Chain & Modern ES6+ ...](02-03-00-Prototypes-OOP-And-Classes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-04-00. Event Loop, Microtasks, Promises & `async/...](02-04-00-Event-Loop-And-Asynchronous-JS.md)

---

# 🟨 02-03-01. Classes, Inheritance & Prototypes — Syntax & Cheat Sheet

## 📑 Съдържание
* [1. Дефиниране на Клас, `#private` полета и Getters/Setters](#1-дефиниране-на-клас-private-полета-и-getterssetters)
* [2. Наследяване с `extends` и `super`](#2-наследяване-с-extends-и-super)
* [3. Статични методи, полета и `static {}` инициализационни блокове](#3-статични-методи-полета-и-static--инициализационни-блокове)
* [4. Директна работа с прототипи (`Object.create`, `Object.hasOwn`)](#4-директна-работа-с-прототипи-objectcreate-objecthasown)
* [5. Factory Functions & Composition (Алтернатива на класовете)](#5-factory-functions--composition-алтернатива-на-класовете)
* [6. Синтактичен Cheat Sheet (JS OOP срещу C#)](#6-синтактичен-cheat-sheet-js-oop-срещу-c)

---

## 1. Дефиниране на Клас, `#private` полета и Getters/Setters

```javascript
class FormulaEngine {
  // Публично поле с начална стойност (Class Field)
  precision = 2;

  // Истинско частно поле (ES2022+) — недостъпно извън класа
  #history = [];

  constructor(name, precision = 2) {
    this.name = name;
    this.precision = precision;
  }

  // Getter (чете се като свойство: engine.historyCount)
  get historyCount() {
    return this.#history.length;
  }

  // Setter с валидация (пише се като свойство: engine.decimals = 4)
  set decimals(value) {
    if (value < 0 || value > 10) {
      throw new RangeError("Точността трябва да е между 0 и 10 знака.");
    }
    this.precision = value;
  }

  // Публичен метод (записан в FormulaEngine.prototype)
  record(expression, result) {
    const rounded = this.#round(result);
    this.#history.push({ expression, result: rounded, date: new Date() });
    return rounded;
  }

  // Частен помощен метод
  #round(num) {
    return Number(num.toFixed(this.precision));
  }
}

const engine = new FormulaEngine("Core", 3);
engine.record("10 / 3", 10 / 3); // 3.333
console.log(engine.historyCount); // 1
```

---

## 2. Наследяване с `extends` и `super`

```javascript
class BaseValidator {
  constructor(ruleName) {
    // Имитация на абстрактен клас в чист JS
    if (new.target === BaseValidator) {
      throw new Error("BaseValidator е абстрактен и не може да се инстанцира директно.");
    }
    this.ruleName = ruleName;
  }

  validate(value) {
    return value !== null && value !== undefined;
  }
}

class RangeValidator extends BaseValidator {
  constructor(min, max) {
    super("RangeRule"); // Задължително преди всяко използване на 'this'!
    this.min = min;
    this.max = max;
  }

  // Overriding на метод от родителския клас
  validate(value) {
    const isPresent = super.validate(value); // Извикване на родителския метод
    return isPresent && typeof value === "number" && value >= this.min && value <= this.max;
  }
}

const ageCheck = new RangeValidator(18, 99);
ageCheck instanceof RangeValidator; // true
ageCheck instanceof BaseValidator;  // true
```

---

## 3. Статични методи, полета и `static {}` инициализационни блокове

```javascript
class MathRegistry {
  static #constants = new Map();

  // Static Initialization Block (ES2022) — изпълнява се веднъж при зареждане на класа
  static {
    this.#constants.set("PI", Math.PI);
    this.#constants.set("E", Math.E);
  }

  static getConstant(key) {
    return this.#constants.get(key.toUpperCase());
  }
}

MathRegistry.getConstant("pi"); // 3.141592653589793
```

---

## 4. Директна работа с прототипи (`Object.create`, `Object.hasOwn`)

```javascript
// 1. Създаване на обект с изрично зададен прототип
const baseOperations = {
  add(a, b) { return a + b; },
  sub(a, b) { return a - b; }
};

const scientificOps = Object.create(baseOperations);
scientificOps.pow = (a, b) => a ** b;

scientificOps.add(2, 3); // 5 (намерено по прототипната верига!)

// 2. Проверка за собствено свойство (модерният заместител на hasOwnProperty)
Object.hasOwn(scientificOps, "pow"); // true
Object.hasOwn(scientificOps, "add"); // false (идва от прототипа)

// 3. Проверка на прототипа на инстанция
Object.getPrototypeOf(scientificOps) === baseOperations; // true
```

---

## 5. Factory Functions & Composition (Алтернатива на класовете)

В съвременния JavaScript често се предпочита **композиция чрез Factory функции**, защото избягва проблемите с `this` при подаване на методи като callbacks:

```javascript
function createLogger(prefix) {
  const logs = []; // Защитено в Closure

  return {
    info(msg) {
      logs.push(`[${prefix}] ${msg}`);
    },
    getLogs() {
      return [...logs]; // Връща копие, за да защити оригиналния масив
    }
  };
}

const logger = createLogger("CALC");
const { info, getLogs } = logger; // Работи без грешка дори след деструктуриране!
info("Готово");
```

---

## 6. Синтактичен Cheat Sheet (JS OOP срещу C#)

| ООП Конструкция | JavaScript (ES2025+) | C# (.NET) |
| :--- | :--- | :--- |
| **Дефиниция на клас** | `class User { ... }` | `public class User { ... }` |
| **Конструктор** | `constructor(name) { this.name = name; }` | `public User(string name) { Name = name; }` |
| **Частно поле** | `#passwordHash;` | `private string _passwordHash;` |
| **Наследяване** | `class Admin extends User` | `public class Admin : User` |
| **Родителски конструктор** | `super(name);` | `: base(name)` |
| **Статичен метод** | `static create() { ... }` | `public static User Create() { ... }` |
| **Проверка за инстанция** | `obj instanceof User` | `obj is User` |
| **Проверка на собствено свойство** | `Object.hasOwn(obj, "key")` | Reflection (`GetProperty`) |







---

[⬅️ 02-03-00. Prototypes, Prototype Chain & Modern ES6+ ...](02-03-00-Prototypes-OOP-And-Classes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [➡️ 02-04-00. Event Loop, Microtasks, Promises & `async/...](02-04-00-Event-Loop-And-Asynchronous-JS.md)

© 2026 Dev-Library / 02-03-01. Classes, Inheritance & Prototypes — Syntax & Cheat Sheet
