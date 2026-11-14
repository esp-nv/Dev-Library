[⬅️ 02-01. Въведение и Среди](02-01-Introduction-to-TypeScript.md) | [📂 Папка: 2-TypeScript-Language](README.md) | [🏠 Начало](../../../README.md) | [➡️ 02-03. OOP в TypeScript](02-03-OOP-in-TypeScript.md)

---

# 02-02. Разширени видове данни: Обединения, Алиаси и Кортежи

**Модул 02:** TypeScript | **Лекционна тема 02:** Union Types, Type Aliases, Tuples, Enums и опционални свойства.

---

## 1. АРХИТЕКТУРЕН АНАЛИЗ & ПРЕДИМСТВА И НЕДОСТАТЪЦИ

В реални среди данните често не са просто единични примитиви. TypeScript разширява възможностите ни чрез гъвкави комбинации от типове и твърди масиви с фиксирана дължина, доближавайки се до изразителността на функционалните и типизираните езици.

### 🟢 Предимства (Pros):
*   **Гъвкаво моделиране на данни (Union Flexibility):** Позволява на променлива да приеме повече от един валиден софтуерен тип безопасно.
*   **Преизползваемост през Type Aliases:** Елиминира дублирането на дълги дефинирания за обекти, като ги капсулира в чисти custom имена.

### 🔴 Недостатъци (Cons):
*   **Изисква Type Narrowing (Свиване на типове):** При използване на обединения (`Union`), браузърът изисква изрична проверка с ключеви думи като `typeof`, преди да ти разреши да извикаш специфични за даден тип методи.
*   **Enums генерират излишен изходен код:** За разлика от чистите типове, стандартният `enum` инжектира реален обект с двупосочно мапиране в крайния JavaScript файл.

---

## 2. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Анти-патърн без Type Guards):
```typescript
type IdType = number | string;

function processRecord(id: IdType) {
    // 🚨 КРАХ: Компилаторът хвърля грешка! 
    // Свойството .toUpperCase() не съществува над типа 'number'.
    return id.toUpperCase(); 
}
```
*   **Грешка при компилация:** *Property 'toUpperCase' does not exist on type 'IdType'. Property 'toUpperCase' does not exist on type 'number'.*

### 🟢 ПРАВИЛЕН КОД (Строг TypeScript стандарт с Навигация на типовете):
```typescript
// 1. Дефиниране на Type Alias за комплексна уеб структура
type UserRole = "Admin" | "Teacher" | "Student"; // Literal Union Type

type UserProfile = {
    readonly id: number | string;               // Union Type предпазен от пренаписване
    username: string;
    role: UserRole;
    phoneNumber?: string;                       // Опционално уеб свойство
};

// 2. Дефиниране на твърд Кортеж (Tuple) за API отговори
type DatabaseResponse = [boolean, string]; 

function authenticateUser(profile: UserProfile): DatabaseResponse {
    // Пример за Type Guard / Narrowing на ниско ниво
    if (typeof profile.id === "string") {
        console.log(`Проверка на низово ID: ${profile.id.trim()}`); // Безрисков стрингов метод
    }

    if (profile.role === "Admin") {
        return [true, "Достъпът е разрешен за администраторски одит."];
    }
    
    return [false, "Грешка: Недостатъчни софтуерни права."];
}

// Извикване на валидираната структура
const activeUser: UserProfile = {
    id: "  AUTH_8971  ",
    username: "esp_nv",
    role: "Admin"
};

const [isAuthorized, message] = authenticateUser(activeUser);
console.log(`Статус: ${isAuthorized} | Съобщение: ${message}`);
```
*   **Коментар към кода:** Използването на `typeof profile.id === "string"` свива типа и софтуерът знае, че вътре в тези скоби `id` е строго `string`. Кортежът (`Tuple`) гарантира, че масивът винаги ще съдържа точно два елемента — първо `boolean`, после `string`.
*   **Очакван компилационен изход в конзолата:**
    ```text
    Проверка на низово ID: AUTH_8971
    Статус: true | Съобщение: Достъпът е разрешен за администраторски одит.
    ```

---
[⬅️ 02-01. Въведение и Среди](02-01-Introduction-to-TypeScript.md) | [📂 Папка: 2-TypeScript-Language](README.md)   | [🏠 Начало](../../../README.md) | [➡️ 02-03. OOP в TypeScript](02-03-OOP-in-TypeScript.md)
