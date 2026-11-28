[⬅️ 02-98. Common Errors, Edge Cases & Fixes](02-98-Common-Errors-And-Fixes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [📦 Към следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md)

---

# 🟨 02-99. Interview Q&A, Скрити Капани & Debugging Techniques

## 📑 Съдържание
* [1. Топ 10 Интервю Въпроси с Архитектурни Отговори](#1-топ-10-интервю-въпроси-с-архитектурни-отговори)
* [2. „Познай изхода“ — 5 класически скрити капана](#2-познай-изхода--5-класически-скрити-капана)
* [3. Професионални техники за Debugging в Chrome DevTools & VS Code](#3-професионални-техники-за-debugging-в-chrome-devtools--vs-code)

---

## 1. Топ 10 Интервю Въпроси с Архитектурни Отговори

1. **Каква е разликата между `var`, `let` и `const`?**
   * **`var`** има *Function Scope*, инициализира се с `undefined` при Hoisting и позволява повторна декларация.
   * **`let` и `const`** имат *Block Scope* (`{}`), стоят в *Temporal Dead Zone (TDZ)* до реда на декларация и не позволяват повторна декларация в същия обхват. `const` забранява презаписване на референцията (но не прави обектите/масивите immutable).
2. **Обяснете какво е Closure и дайте реален пример от практиката.**
   * Closure е комбинацията от функция и нейния лексикален обхват, която позволява на вътрешна функция да достъпва променливите на външната функция дори след като външната вече е приключила изпълнението си. Използва се за *private state* (инкапсулация), *Memoization*, *Debounce/Throttle* и *Currying*.
3. **Какъв е редът на изпълнение в Event Loop между синхронен код, `Promise.then` и `setTimeout(..., 0)`?**
   * Първо се изпълнява целият **синхронен код** в Call Stack ➔ след това **всички Microtasks** (`Promise.then`, `queueMicrotask`) до пълно изпразване на опашката ➔ чак след това се взема **един Macrotask** (`setTimeout`).
4. **Как се определя стойността на `this` в JavaScript?**
   * При обикновените функции `this` се определя **динамично в момента на извикване** по 4 правила (по приоритет): `new` binding ➔ explicit (`call`/`apply`/`bind`) ➔ implicit (обектът преди точката) ➔ default (`undefined` в strict mode / `window` без strict mode). При **Arrow функциите** няма собствен `this` — те го наследяват лексикално от обграждащия обхват.
5. **Каква е разликата между `==` и `===`?**
   * `==` извършва неявно преобразуване на типовете (*Type Coercion*) по сложни правила, докато `===` проверява едновременно **стойност И тип** без преобразуване. Изключение за еднакви стойности прави само `Object.is(NaN, NaN) === true` и `Object.is(+0, -0) === false`.
6. **Как работи прототипното наследяване (`Prototype Chain`)?**
   * Всеки обект има вътрешна връзка `[[Prototype]]` към родителски прототип. При търсене на свойство или метод, V8 първо проверява собствените свойства на обекта, после се изкачва по веригата на прототипите до `Object.prototype` и накрая стига до `null`.
7. **Каква е разликата между `Promise.all` и `Promise.allSettled`?**
   * `Promise.all` следва принципа *Fail-Fast* — отхвърля се веднага щом **който и да е** от подадените Promises се провали. `Promise.allSettled` изчаква **всички** да приключат и връща масив от обекти `{ status: "fulfilled" | "rejected", value | reason }`.
8. **Какво е Event Delegation и защо се използва?**
   * Техника, при която вместо да закачаме отделни Event Listeners на множество дъщерни елементи, закачаме **един слушател на общия родител** и използваме Bubbling фазата + `event.target.closest(...)`. Пести памет и работи автоматично за динамично добавени елементи.
9. **Каква е разликата между `localStorage`, `sessionStorage` и `HttpOnly` Cookies?**
   * `localStorage` (~5MB) пази данните постоянно в браузъра; `sessionStorage` (~5MB) ги изтрива при затваряне на таба; нито едно от двете не се праща автоматично към сървъра. `Cookies` (~4KB) се изпращат с всяка HTTP заявка, а флагът `HttpOnly` ги прави напълно недостъпни за JavaScript (защита от XSS кражба на сесия).
10. **Как V8 оптимизира изпълнението на JavaScript (JIT & Hidden Classes)?**
    * V8 парсва кода в AST, компилира го в байткод чрез **Ignition** и профилира горещите функции. Често изпълняваният код се компилира в оптимизиран машинен код от **TurboFan**, разчитайки на стабилни **Hidden Classes (Shapes)** на обектите.

---

## 2. „Познай изхода“ — 5 класически скрити капана

### Капан 1: Event Loop (Sync vs Microtask vs Macrotask)
```javascript
console.log("1");
setTimeout(() => console.log("2"), 0);
Promise.resolve().then(() => console.log("3"));
console.log("4");
// Изход: 1, 4, 3, 2
```

### Капан 2: `var` в цикъл срещу `let`
```javascript
for (var i = 0; i < 3; i++) {
  setTimeout(() => console.log(i), 0);
}
// Изход: 3, 3, 3 (една споделена променлива 'i' във function scope)
```

### Капан 3: Автоматично вмъкване на точка и запетая (ASI) след `return`
```javascript
function getUser() {
  return
  {
    name: "Ivan"
  };
}
console.log(getUser()); // undefined! (JS слага ';' веднага след return)
```

### Капан 4: `typeof null` и `Array.isArray`
```javascript
console.log(typeof null);        // "object" (исторически бъг от 1995 г.)
console.log(typeof []);          // "object"
console.log(Array.isArray([]));  // true
```

### Капан 5: Сортиране на числа по подразбиране
```javascript
console.log([1, 20, 3, 100].sort());
// Изход: [1, 100, 20, 3] (лексикографско сравнение като стрингове!)
```

---

## 3. Професионални техники за Debugging в Chrome DevTools & VS Code

* **Ключовата дума `debugger;`:** Поставена в кода, автоматично спира изпълнението на точно този ред, когато DevTools са отворени.
* **Conditional Breakpoints & Logpoints:** В Chrome DevTools / VS Code кликнете с десен бутон върху номера на реда ➔ *Add conditional breakpoint* (спира само при условие, напр. `i === 50`) или *Add logpoint* (печата в конзолата без да променяте изходния код!).
* **Структурирано логване в конзолата:**
  * `console.table(arrayOfObjects)` — визуализира масив от обекти като интерактивна сортируема таблица.
  * `console.time("parse") ... console.timeEnd("parse")` — измерва точното време за изпълнение в милисекунди.
  * `console.trace()` — отпечатва пълния Call Stack до текущия ред.
* **Memory & Performance Profiling:** Използвайте таб **Memory ➔ Heap Snapshot** в Chrome DevTools, за да откриете *Detached DOM nodes* и Memory Leaks.







---

[⬅️ 02-98. Common Errors, Edge Cases & Fixes](02-98-Common-Errors-And-Fixes.md) | [📂 Назад към JavaScript Language & Architecture Хъба](README.md) | [📦 Предишен модул: HTML5-CSS3-And-SASS](../HTML5-CSS3-And-SASS/README.md) | [🌐 Домейн: 02. Фронтенд и UI/UX Дизайн](../README.md) | [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏛️ Начало Dev-Library](../../../README.md) | [📂 Текуща папка: JavaScript Language & Architecture](README.md) | [🌐 Следващ домейн: 03-Backend-Ecosystems](../../03-Backend-Ecosystems/README.md) | [📦 Следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [📦 Към следващ модул: Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md)

© 2026 Dev-Library / 02-99. Interview Q&A, Скрити Капани & Debugging Techniques
