[⬅️ 02-02-01. Functions, Closures & Array Methods — Syntax](02-02-01-Syntax-Functions-Closures-And-This.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-03-01. Prototypes, OOP & Classes — Syntax](02-03-01-Syntax-Prototypes-OOP-And-Classes.md)

---

# 🟨 02-03-00. Prototypes, Prototype Chain & Modern ES6+ Classes

## 📑 Съдържание
* [1. Прототипно наследяване (Prototypal Inheritance) vs Класово ООП](#1-прототипно-наследяване-prototypal-inheritance-vs-класово-ооп)
* [2. Веригата на прототипите (Prototype Chain) и `[[Prototype]]`](#2-веригата-на-прототипите-prototype-chain-и-prototype)
* [3. Какво точно прави операторът `new` под капака?](#3-какво-точно-прави-операторът-new-под-капака)
* [4. Модерни ES6+ Класове и истински `#private` полета](#4-модерни-es6-класове-и-истински-private-полета)
* [5. Сравнение на ООП в JavaScript срещу C# (.NET)](#5-сравнение-на-ооп-в-javascript-срещу-c-net)
* [6. Скрити капани и Edge Cases](#6-скрити-капани-и-edge-cases)

---

## 1. Прототипно наследяване (Prototypal Inheritance) vs Класово ООП

За разлика от **C#** и **Java**, където класовете са статични чертежи (*Blueprints*), от които се инстанцират обекти, **JavaScript няма истински класове на ниво runtime** — той използва **Прототипно наследяване (Prototypal Inheritance)**.

* В JavaScript **обектите наследяват директно от други обекти** чрез скрита вътрешна връзка `[[Prototype]]`.
* Синтаксисът `class` (въведен в ES6 / ES2015) е **„синтактична захар“ (Syntactic Sugar)** върху прототипния модел, но с няколко важни подобрения за безопасност (задължителен `strict mode`, невъзможност да се извика без `new` и истински `#private` полета).

> **🧠 Мнемоника:** В **C#** обектът е *къща, построена по архитектурен чертеж (клас)*. В **JavaScript** обектът е *къща, която има телефонна линия директно към къщата-родител (прототип)* — ако в твоята къща няма чук, звъниш на родителя да го използваш.

---

## 2. Веригата на прототипите (Prototype Chain) и `[[Prototype]]`

Всеки обект в JavaScript има скрито вътрешно свойство `[[Prototype]]` (достъпно стандартизирано чрез `Object.getPrototypeOf(obj)` или исторически чрез `__proto__`).

Когато достъпите свойство или метод (`user.toString()`), V8 изпълнява следното търсене:
1. Проверява дали `user` притежава свойството като **собствено (Own Property)** (`Object.hasOwn(user, "toString")`).
2. Ако не го намери, преминава по `[[Prototype]]` към `User.prototype`.
3. Ако и там липсва, продължава нагоре към `Object.prototype`.
4. На върха на веригата стои **`null`** (`Object.getPrototypeOf(Object.prototype) === null`). Ако свойството не е намерено до `null`, се връща **`undefined`** (или `TypeError`, ако се опитвате да го извикате като функция).

```text
[ Инстанция: calc ] ──[[Prototype]]──► [ Calculator.prototype ] ──[[Prototype]]──► [ Object.prototype ] ──► null
  • history: []                          • evaluate(expr)                            • toString()
  • precision: 2                         • clearHistory()                            • hasOwnProperty()
```

### Важна разлика: `prototype` срещу `[[Prototype]]` (`__proto__`)
* **`Fn.prototype`:** Свойство, което притежават **само функциите-конструктори и класовете**. То определя какъв ще бъде `[[Prototype]]` на бъдещите инстанции, създадени с `new Fn()`.
* **`obj.[[Prototype]]` (`Object.getPrototypeOf(obj)`):** Скритата връзка на **всяка инстанция** към нейния родителски прототип.

---

## 3. Какво точно прави операторът `new` под капака?

Когато напишете `const calc = new Calculator(2)`, JavaScript енджинът автоматично изпълнява **4 стъпки**:

1. **Създава нов празен обект** в Memory Heap: `const instance = {};`
2. **Свързва прототипа:** Задава `Object.setPrototypeOf(instance, Calculator.prototype)`.
3. **Изпълнява конструктора с обвързан `this`:** Извиква `Calculator.call(instance, 2)`, така че всички `this.precision = 2` се записват върху новия обект.
4. **Връща обекта:** Връща автоматично `instance` (освен ако конструкторът изрично не върне друг референтен обект).

---

## 4. Модерни ES6+ Класове и истински `#private` полета

Съвременният ES2022+ синтаксис предоставя чиста структура за ООП:

* **`constructor(...)`:** Специален метод за инициализация на инстанцията (всеки клас може да има само един конструктор).
* **Методи в тялото на класа:** Автоматично се закачат върху `ClassName.prototype` (не се дублират в паметта за всяка инстанция!).
* **Истински частни полета и методи (`#field`, `#method()`):** Въведени в ES2022. За разлика от конвенцията с долна черта `_field` (която е само визуална), `#field` се **налага твърдо от V8 енджина на ниво синтаксис и runtime** — опит за достъп отвън хвърля `SyntaxError`.
* **Статични членове (`static`):** Принадлежат на самия клас (`MathHelper.round()`), а не на инстанциите.

---

## 5. Сравнение на ООП в JavaScript срещу C# (.NET)

| Концепция | JavaScript (ES2025+) | C# (.NET) |
| :--- | :--- | :--- |
| **Модел на наследяване** | Прототипно (`class` + `extends` е захар върху прототипи) | Класово (статични типове и V-Table) |
| **Модификатори за достъп** | Public по подразбиране; истински `#private` (няма `protected`) | `public`, `private`, `protected`, `internal` |
| **Множествено наследяване** | Единично (`extends` приема 1 родител; използват се Mixins) | Единично наследяване на класове + множество `interface` |
| **Абстрактни класове / Интерфейси** | Няма в чист JS (имитират се с проверка `new.target === Base`) | Вградени `abstract class` и `interface` (и в TypeScript!) |
| **Извикване на родителски конструктор** | Задължително `super()` **преди** всяко използване на `this` | `: base(...)` в сигнатурата на конструктора |

---

## 6. Скрити капани и Edge Cases

1. **Използване на `this` преди `super()` в наследник:** В клас с `extends`, опитът да достъпите `this` преди да извикате `super()` хвърля `ReferenceError: Must call super constructor in derived class before accessing 'this'`.
2. **Класовете НЕ се hoist-ват като функции:** За разлика от `function Foo() {}`, декларацията `class Foo {}` остава в **Temporal Dead Zone (TDZ)** до реда на дефиниция.
3. **Стрелкови функции като методи в клас (`method = () => {}`):** Създават отделно копие на функцията за **всяка една инстанция** (хабят повече RAM), вместо да споделят един метод в `prototype`, но гарантират заключен `this` при подаване като Event Handler.

---

[⬅️ 02-02-01. Functions, Closures & Array Methods — Syntax](02-02-01-Syntax-Functions-Closures-And-This.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-03-01. Prototypes, OOP & Classes — Syntax](02-03-01-Syntax-Prototypes-OOP-And-Classes.md)

© 2026 Dev-Library / 02-03-00. Prototypes, Prototype Chain & Modern ES6+ Classes
