[⬅️ 02-10. TypeScript Category Cheat Sheet](02-10-Category-Cheat-Sheet.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-12. Interview QA, Traps & Debugging](02-12-Interview-QA-Traps-And-Debugging.md)

---

# 02-11. TypeScript Common Errors, Edge Cases & Fixes (Каталог на грешките)

**Цел:** Практически наръчник за бързо диагностициране и фиксиране на най-честите компилационни грешки в TypeScript (`TS2xxx`), Runtime изненади заради Type Erasure и гранични случаи (**Edge Cases** при масиви, индекси, `null`/`undefined` и числа).

## 🧭 Съдържание на темата
* [1. Компилационни грешки при `null` и `undefined` (`TS18048` / `TS2532` / `TS2322`)](#1-компилационни-грешки-при-null-и-undefined-ts18048--ts2532--ts2322)
* [2. Грешки при индексиране на обекти и речници (`TS7053` / `TS2536`)](#2-грешки-при-индексиране-на-обекти-и-речници-ts7053--ts2536)
* [3. Грешки при несъвместимост на типове и Type Widening (`TS2345` / `TS2339`)](#3-грешки-при-несъвместимост-на-типове-и-type-widening-ts2345--ts2339)
* [4. Гранични случаи (Edge Cases): Начало и край на колекции, NaN и Type Erasure при JSON](#4-гранични-случаи-edge-cases-начало-и-край-на-колекции-nan-и-type-erasure-при-json)
* [5. Бърза диагностична таблица (Error Code ➔ Причина ➔ Фикс)](#5-бърза-диагностична-таблица-error-code--причина--фикс)
* [6. ЙЕРАРХИЯ НА ИЗКЛЮЧЕНИЯТА, CUSTOM EXCEPTIONS & STACK TRACE ПРАВИЛА](#6-йерархия-на-изключенията-custom-exceptions--stack-trace-правила)

---

## 1. КОМПИЛАЦИОННИ ГРЕШКИ ПРИ `NULL` И `UNDEFINED` (`TS18048` / `TS2532` / `TS2322`)

### Грешка: `TS18048: 'user' is possibly 'undefined'.`
*   **Причина:** При включен `strictNullChecks: true` методи като `Array.find()` или `Map.get()` връщат `T | undefined`, а кодът се опитва директно да достъпи свойство `user.email`.
*   **Грешно срещу Правилно:**
```typescript
interface User { id: number; email: string; }
const users: User[] = [{ id: 1, email: "ana@test.bg" }];

// ❌ ГРЕШНО: TS18048 ('found' може да е undefined!)
// const found = users.find(u => u.id === 2);
// console.log(found.email);

// ❌ ОПАСЕН ФИКС: Non-null assertion (!) маха грешката при компилация, но гърми в Runtime!
// console.log(found!.email); // TypeError: Cannot read properties of undefined!

// ✅ ПРАВИЛЕН ФИКС 1: Optional Chaining (?.) + Nullish Coalescing (??)
const found = users.find(u => u.id === 2);
const email = found?.email ?? "guest@test.bg";

// ✅ ПРАВИЛЕН ФИКС 2: Early Return / Guard Clause (стеснява типа до User)
if (!found) {
    throw new Error("Потребителят не е намерен!");
}
console.log(found.email); // Тук TypeScript знае на 100%, че found е User!
```

---

## 2. ГРЕШКИ ПРИ ИНДЕКСИРАНЕ НА ОБЕКТИ И РЕЧНИЦИ (`TS7053` / `TS2536`)

### Грешка: `TS7053: Element implicitly has an 'any' type because expression of type 'string' can't be used to index type ...`
*   **Причина:** Опитваш да достъпиш свойство на обект чрез динамична променлива `obj[key]`, където `key` е обикновен `string`, а обектът няма Index Signature.
*   **Грешно срещу Правилно:**
```typescript
const statusMessages = {
    200: "OK",
    404: "Not Found",
    500: "Server Error"
} as const;

// ❌ ГРЕШНО: TS7053 (code е обикновен number, не 200 | 404 | 500)
function getMessageBad(code: number): string {
    // return statusMessages[code]; // Compile Error!
    return "";
}

// ✅ ПРАВИЛЕН ФИКС: Използване на keyof typeof + Type Guard
type StatusCode = keyof typeof statusMessages; // 200 | 404 | 500

function isValidStatus(code: number): code is StatusCode {
    return code in statusMessages;
}

function getMessageSafe(code: number): string {
    return isValidStatus(code) ? statusMessages[code] : "Unknown Status";
}
```
---

## 3. ГРЕШКИ ПРИ НЕСЪВМЕСТИМОСТ НА ТИПОВЕ И TYPE WIDENING (`TS2345` / `TS2339`)

### Грешка: `TS2345: Argument of type 'string' is not assignable to parameter of type '"GET" | "POST"'.`
*   **Причина:** Когато декларираш обект с `const req = { method: "GET" }`, TypeScript разширява (*Type Widening*) типа на `method` до общ `string`, защото теоретично свойството може да бъде презаписано по-късно.
*   **Грешно срещу Правилно:**
```typescript
type HttpMethod = "GET" | "POST" | "DELETE";

function sendRequest(url: string, method: HttpMethod) {
    console.log(method, url);
}

// ❌ ГРЕШНО: req.method има тип 'string', а не '"GET"' -> TS2345!
// const req = { url: "/api/users", method: "GET" };
// sendRequest(req.url, req.method);

// ✅ ПРАВИЛЕН ФИКС: Добавяне на `as const` или изрично типизиране
const req = { url: "/api/users", method: "GET" } as const;
sendRequest(req.url, req.method); // Работи без грешка!
```

---

## 4. ГРАНИЧНИ СЛУЧАИ (EDGE CASES): НАЧАЛО И КРАЙ НА КОЛЕКЦИИ, NAN И TYPE ERASURE ПРИ JSON

| Граничен случай (Edge Case) | Какво се чупи в Runtime | Как се фиксира правилно |
| :--- | :--- | :--- |
| **Празен масив или излизане извън граници (`arr[0]`, `arr[arr.length]`)** | В JS/TS `arr[999]` не хвърля `IndexOutOfBoundsException`, а връща тихо **`undefined`**! | Включи `"noUncheckedIndexedAccess": true` в `tsconfig.json` и проверявай `if (arr.length === 0)` или ползвай `arr.at(-1)` с проверка. |
| **Деление на нула (`10 / 0`) и `Number("abc")`** | Не хвърля грешка, а връща **`Infinity`** или **`NaN`** (а `typeof NaN === "number"`!). | Проверявай изрично `Number.isFinite(val)` и `!Number.isNaN(val)` преди математически операции. |
| **Числова стойност `0` или празен низ `""` при `\|\|`** | `0 \|\| 10` връща `10`, защото `0` е *falsy*! | Използвай **Nullish Coalescing (`??`)**: `0 ?? 10` връща коректно `0`. |
| **Външен JSON от `response.json() as User`** | `as User` се изтрива при компилация (**Type Erasure**). Ако бекендът върне `null` вместо масив, приложението гърми в Runtime! | Валидирай входящия JSON на границата на системата с **Zod** (`UserSchema.parse(rawJson)`). |

---

## 5. БЪРЗА ДИАГНОСТИЧНА ТАБЛИЦА (ERROR CODE ➔ ПРИЧИНА ➔ ФИКС)

| Код на грешка | Съобщение | Причина | Бърз Фикс |
| :--- | :--- | :--- | :--- |
| **`TS2322`** | `Type 'X' is not assignable to type 'Y'` | Несъвместими типове при присвояване | Провери дали не липсва поле или дали няма `null`/`undefined` |
| **`TS2339`** | `Property 'x' does not exist on type 'Y'` | Достъп до несъществуващо поле или върху Union тип без стесняване | Използвай Type Guard (`"x" in obj` или `typeof`) |
| **`TS7006`** | `Parameter 'x' implicitly has an 'any' type` | Липсва тип на параметър във функция при `strict: true` | Добави изричен тип: `(x: string) => ...` |
| **`TS2769`** | `No overload matches this call` | Подадени са грешни аргументи на функция с няколко сигнатури | Провери типовете и броя на аргументите отзад-напред в грешката |
| **`TS18046`** | `'err' is of type 'unknown'` | В `catch (err)` по подразбиране грешката е `unknown` | Провери `if (err instanceof Error) console.error(err.message)` |

---

---

## 6. ЙЕРАРХИЯ НА ИЗКЛЮЧЕНИЯТА, CUSTOM EXCEPTIONS & STACK TRACE ПРАВИЛА

По модела на системните runtime среди (.NET CLR / JVM / V8), грешките в TypeScript / JavaScript се делят на **вградени системни грешки** и **потребителски бизнес изключения (Custom Errors)**:

### 1. Вградени системни грешки срещу Потребителски (Application) изключения

| Категория | Вградени класове (V8 / Runtime) | Потребителски класове (Custom `Error`) |
| :--- | :--- | :--- |
| **Примери** | `TypeError`, `RangeError`, `SyntaxError`, `ReferenceError`, `URIError` | `ValidationError`, `NotFoundError`, `UnauthorizedError` |
| **Възстановимост** | Обикновено сочат към бъг в кода (напр. достъп до `undefined`) | Очаквани бизнес състояния, които се хващат и превръщат в HTTP статус (`400`, `404`) |
| **Критични сривове** | `RangeError: Maximum call stack size exceeded` (аналог на `StackOverflowError`) или `Out Of Memory` — не се хващат с `try-catch` | Не се прилага |

#### Подробна справка: Кога възниква всяка грешка и в коя фаза?

| Клас / Тип грешка | Фаза на възникване | При какво възниква (Точна причина) | Пример и как се предотвратява |
| :--- | :--- | :--- | :--- |
| **`TSxxxx` (`TS2322`, `TS18048`)** | **Компилационна** (`Compile-time` при `tsc`) | Несъвместими типове, липсващо поле, `undefined` достъп или грешен брой аргументи още преди кодът да тръгне. | `const x: number = "abc";` ➔ Оправя се преди билд чрез коректни типове и Type Guards. |
| **`SyntaxError`** | **Компилационна / Парсване** (`Parse-time`) | Нарушен синтаксис на езика (незатворена скоба) или подаване на невалиден низ към `JSON.parse("{bad}")`. | Винаги обвивайте `JSON.parse()` в `try-catch` при външни данни. |
| **`TypeError`** | **При изпълнение** (`Runtime` в V8) | Опит за операция върху несъвместим тип в паметта — най-често четене на свойство от `undefined`/`null` или викане на нещо, което не е функция. | `undefined.toUpperCase()` или `(123 as any)()` ➔ Използвайте `?.`, `??` и Zod валидация на входа. |
| **`ReferenceError`** | **При изпълнение** (`Runtime` в V8) | Достъп до променлива, която изобщо не е декларирана или се чете преди инициализация в *Temporal Dead Zone* (`let`/`const`). | `console.log(user); let user = "Ana";` ➔ Декларирайте променливите в началото на обхвата. |
| **`RangeError`** | **При изпълнение** (`Runtime` в V8) | Число или дължина извън допустимите граници — напр. `new Array(-1)`, `num.toFixed(200)` или безкрайна рекурсия (`Maximum call stack size exceeded`). | Проверявайте границите на индексите и слагайте базово дъно при рекурсия. |
| **`URIError`** | **При изпълнение** (`Runtime` в V8) | Подаване на невалидна URL последователност към `decodeURIComponent("%E0%A4%A")`. | Валидирайте или обвийте `decodeURIComponent()` в `try-catch`. |
| **`AggregateError`** | **При изпълнение** (`Runtime` в V8) | Когато няколко паралелни операции се провалят едновременно — напр. всички промиси в `Promise.any([...])` бъдат отхвърлени. | Проверете масива `err.errors`, за да видите всяка отделна причина за отказ. |
| **Custom `Error` (`NotFoundError`)** | **При изпълнение** (`Runtime` — бизнес логика) | Когато приложението умишлено хвърли бизнес грешка (`throw new NotFoundError(...)`), за да прекъсне операцията контролирано. | Хваща се в глобален Error Handler (напр. Express Middleware) и се връща като HTTP `404` / `400`. |


### 2. Анатомия на обекта `Error` и запазване на веригата (`cause` = `InnerException`)
*   **`err.name`:** Името на класа на грешката (напр. `"ValidationError"`).
*   **`err.message`:** Четимо описание на причината за проблема.
*   **`err.stack`:** Пълният **Stack Trace** (последователността от извикани функции и номерата на редовете).
*   **`err.cause` (ES2022+):** Еквивалент на `InnerException` в C# — пази оригиналната нисконивова грешка, когато я обвиеш в по-високонивово бизнес изключение!

### 3. Златни правила при `try-catch-finally` и прехвърляне (`throw`)
```typescript
class DatabaseQueryError extends Error {
    constructor(message: string, options?: { cause?: unknown }) {
        super(message, options);
        this.name = "DatabaseQueryError";
    }
}

async function fetchOrderSafe(id: number) {
    try {
        // Симулация на нисконивова мрежова грешка
        throw new TypeError("Connection reset by peer");
    } catch (err) {
        // ✅ ПРАВИЛНО: Обвиваме грешката и подаваме { cause: err }, за да НЕ загубим оригиналния Stack Trace!
        throw new DatabaseQueryError(`Неуспешно зареждане на поръчка #${id}`, { cause: err });
    } finally {
        // Изпълнява се ВИНАГИ (за почистване на ресурси/таймери), освен при рязък срив на процеса (process.exit)
    }
}
```
*   **Производителност:** Хвърлянето и хващането на изключения създава тежък обект със Stack Trace и забавя V8 двигателя. **Никога** не използвай `throw` за нормален контрол на потока (Flow Control) — при очаквани липси връщай `undefined`, `null` или Result обект `{ ok: false, error: "..." }`.


[⬅️ 02-10. TypeScript Category Cheat Sheet](02-10-Category-Cheat-Sheet.md) | 📂 Папка: 02-TypeScript-Language | [🏠 Начало](../README.md) | [➡️ 02-12. Interview QA, Traps & Debugging](02-12-Interview-QA-Traps-And-Debugging.md)
