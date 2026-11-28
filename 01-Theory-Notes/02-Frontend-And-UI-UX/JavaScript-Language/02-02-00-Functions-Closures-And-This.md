[⬅️ 02-01-01. Variables, Types & Operators — Syntax](02-01-00-Variables-Types-And-Coercion.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-02-01. Functions, Closures & `this` — Syntax](02-02-01-Syntax-Functions-Closures-And-This.md)

---

# 🟨 02-02-00. Functions, Lexical Scope, Closures & `this` Binding

## 📑 Съдържание
* [1. First-Class Functions & Higher-Order Functions (HOF)](#1-first-class-functions--higher-order-functions-hof)
* [2. Execution Context, Lexical Scope и Scope Chain](#2-execution-context-lexical-scope-и-scope-chain)
* [3. Closures (Затваряния) — Как работят в паметта](#3-closures-затваряния--как-работят-в-паметта)
* [4. Правилата за обвързване на `this` (4-те правила)](#4-правилата-за-обвързване-на-this-4-те-правила)
* [5. Function Declaration vs Function Expression vs Arrow Function](#5-function-declaration-vs-function-expression-vs-arrow-function)
* [6. Скрити капани и Edge Cases](#6-скрити-капани-и-edge-cases)

---

## 1. First-Class Functions & Higher-Order Functions (HOF)

В JavaScript функциите са **First-Class Citizens (първокласни обекти)** — те са специален вид callable обекти, което означава, че могат да бъдат:
* Присвоявани на променливи и константи
* Подавани като аргументи на други функции (**Callbacks**)
* Връщани като резултат от други функции (**Factory / Curried Functions**)
* Съхранявани в масиви и обекти.

**Higher-Order Function (Функция от по-висок ред)** е всяка функция, която приема друга функция като аргумент (напр. `map`, `filter`, `addEventListener`) или връща нова функция.

---

## 2. Execution Context, Lexical Scope и Scope Chain

* **Lexical Scope (Лексикален / Статичен обхват):** Видимостта на променливите се определя от **мястото, където функцията е написана в кода**, а не от мястото, откъдето е извикана.
* **Scope Chain (Верига на обхватите):** Когато V8 търси променлива, първо проверява локалния `Execution Context`. Ако не я намери, се изкачва нагоре към родителския лексикален обхват и така чак до `Global Scope`. Ако и там липсва ➔ хвърля `ReferenceError`.

```text
Global Scope (const taxRate = 0.20)
 └── Outer Function: createInvoice(client)
      └── Inner Function: calculateTotal(amount)
           └─ Търси 'amount' (локално ✅) ➔ 'client' (родителски ✅) ➔ 'taxRate' (глобален ✅)
```

---

## 3. Closures (Затваряния) — Как работят в паметта

**Closure (Затваряне)** е комбинацията от функция и нейния лексикален обхват. Closure възниква, когато **вътрешна функция запомня и продължава да има достъп до променливите на външната си функция, дори след като външната функция вече е приключила изпълнението си** и е премахната от Call Stack!

* **Как V8 го съхранява:** Вместо да унищожи локалните променливи на външната функция след `return`, V8 ги премества в **Memory Heap** в специален вътрешен обект (`[[Environment]]` / `Context`), докато съществува референция към вътрешната функция.
* **Практически приложения:** Истинска капсулация на състояние (private state без класове), Memoization (кеширане на скъпи изчисления), Debounce/Throttle и Event Handlers.

> **🧠 Мнемоника:** **Closure е раницата на функцията** — когато вътрешната функция излезе от външната, тя носи със себе си „раница“ с всички променливи, които са били около нея при създаването ѝ.

---

## 4. Правилата за обвързване на `this` (4-те правила)

За разлика от C# и Java, където `this` винаги сочи към текущата инстанция на класа, в JavaScript стойността на **`this` се определя динамично в момента на извикване на функцията** (с изключение на Arrow функциите), по следните 4 правила (подредени по приоритет от най-висок към най-нисък):

1. **`new` Binding (Най-висок приоритет):** При извикване с `new User()`, се създава нов празен обект и `this` сочи към него.
2. **Explicit Binding (`call`, `apply`, `bind`):** Програмистът изрично подава кой обект да бъде `this` (`fn.call(user)`).
3. **Implicit Binding (Извикване като метод на обект):** `user.getName()` ➔ `this` е обектът **преди точката** (`user`).
4. **Default Binding (Свободно извикване):** `getName()` ➔ в `"use strict"` (и ES Modules) `this` е **`undefined`**; без strict mode сочи към глобалния обект (`window` / `globalThis`).

---

## 5. Function Declaration vs Function Expression vs Arrow Function

| Характеристика | Function Declaration (`function fn() {}`) | Function Expression (`const fn = function() {}`) | Arrow Function (`const fn = () => {}`) |
| :--- | :--- | :--- | :--- |
| **Hoisting** | ✅ Изцяло (може да се вика преди реда) | ❌ Не (подлежи на TDZ при `const`/`let`) | ❌ Не (подлежи на TDZ при `const`/`let`) |
| **Собствен `this`** | Да (динамичен при извикване) | Да (динамичен при извикване) | ❌ **Няма!** Наследява `this` лексикално от обграждащия scope |
| **Обект `arguments`** | Да | Да | ❌ Няма (използвайте `...args`) |
| **Конструктор (`new`)** | ✅ Може (`new fn()`) | ✅ Може (`new fn()`) | ❌ Хвърля `TypeError: fn is not a constructor` |

---

## 6. Скрити капани и Edge Cases

1. **Загуба на `this` при подаване на метод като callback:**
   * При `button.addEventListener("click", calculator.reset)` методът `reset` се предава като референция без обекта преди точката ➔ при клик `this` става `button` (или `undefined`), а не `calculator`!
   * **Решение:** Използвайте `.bind(calculator)` или обвиваща Arrow функция `() => calculator.reset()`.
2. **Arrow функция като метод на обект:**
   * `const obj = { count: 0, inc: () => { this.count++; } }` ➔ `this` **не е** `obj`, а външният обхват (`window`/`undefined`), защото обектният литерал `{}` **не създава** лексикален обхват! Използвайте съкратен метод `inc() { this.count++; }`.
3. **Класическият Closure капан с `var` в цикъл:**
   * `for (var i = 0; i < 3; i++) { setTimeout(() => console.log(i), 0); }` отпечатва `3, 3, 3`, защото `var` има само една споделена инстанция за цялата функция. С `let` на всяка итерация се създава нов лексикален обхват и отпечатва `0, 1, 2`.

---

[⬅️ 02-01-01. Variables, Types & Operators — Syntax](02-01-00-Variables-Types-And-Coercion.md) | [📂 Назад към JavaScript Language Хъба](README.md) | [📦 Предишен модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [🌐 Домейн: 02. Frontend & UI/UX](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language](README.md) | [🌐 Следващ домейн: 03. Backend Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: TypeScript-Language](../TypeScript-Language/README.md) | [➡️ 02-02-01. Functions, Closures & `this` — Syntax](02-02-01-Syntax-Functions-Closures-And-This.md)

© 2026 Dev-Library / 02-02-00. Functions, Lexical Scope, Closures & `this` Binding
