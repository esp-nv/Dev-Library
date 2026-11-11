[⬅️ 02-03. OOP в TypeScript](02-03-OOP-in-TypeScript.md) | [📂 Папка: 2-TypeScript-Language](README.md)  | [🏠 Начало](../../../README.md) | [➡️ 02-05. TypeScript Декоратори](02-05-TypeScript-Decorators.md)

---

# 02-04. Овладяване на Дженерици: Гъвкави и преизползваеми структури

**Модул 02:** TypeScript | **Лекционна тема 04:** Дженерици (Generics), обобщени класове и функции, и софтуерни ограничения (Constraints) чрез extends.

---

## 1. АРХИТЕКТУРЕН АНАЛИЗ & ПРЕДИМСТВА И НЕДОСТАТЪЦИ

Дженериците (**Generics**) във фреймуърк средите на TypeScript ни дават възможност да дефинираме софтуерни компоненти (класове, интерфейси или функции), които работят с голямо разнообразие от видове данни, без предварително да заковават конкретен примитив. Те служат като шаблони, които приемат типовете като динамични променливи, силно наподобявайки мощните `Generic <T>` структури в C# екосистемата.

### 🟢 Предимства (Pros):
*   **Максимална преизползваемост на кода:** Един и същ логически компонент (напр. структура за HTTP отговори или мрежови хранилища) може да обслужва различни бизнес обекти, запазвайки 100% от сигурността на типа.
*   **Елиминиране на компромисния тип `any`:** Потребителят запазва пълна инспекция и IntelliSense върху обектите, които извлича от дженерик структурите.

### 🔴 Недостатъци (Cons):
*   **Повишена синтактична сложност:** Натрупването на множество типови променливи (напр. `<T, K, V>`) и вложени ограничения прави кода труден за бързо четене от младши програмисти.
*   **Компилационни рестрикции:** Вътре в дженерик функцията нямаш право да извършваш специфични операции над променливите, освен ако те не са изрично застраховани чрез налагане на софтуерни ограничения (`Constraints`).

---

## 2. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Анти-патърн с липсващи софтуерни ограничения):
```typescript
function getLengthProperty<T>(input: T): number {
    // 🚨 КРАХ: Компилаторът хвърля грешка на секундата!
    // Той не знае какъв е типът 'T' и дали притежава свойството .length.
    return input.length; 
}
```
*   **Грешка при компилация:** *Property 'length' does not exist on type 'T'.*

### 🟢 ПРАВИЛЕН КОД (Строг TypeScript стандарт за Generic Шаблони):
```typescript
// 1. Дефиниране на интерфейс за налагане на твърдо софтуерно ограничение
interface ILengthwise {
    length: number;
}

// 2. Дженерик функция с изрично застрахован constraint чрез 'extends'
function getLengthSecure<T extends ILengthwise>(input: T): number {
    return input.length; // Напълно защитено — компилаторът знае, че всичко тук има .length
}

// 3. Изграждане на напреднал Дженерик склад (Generic Storage Class) за .NET съвместимост
class DataRepositoryHub<T> {
    private entityStore: T[] = [];

    public pushToMemory(item: T): void {
        this.entityStore.push(item);
    }

    public extractFromMemory(index: number): T {
        if (index < 0 || index >= this.entityStore.length) {
            throw new Error("Софтуерен срив: Индексът е извън обхвата на RAM масива.");
        }
        return this.entityStore[index];
    }
}

// 4. Тестване и одит на дженерик средите
type StudentModuleRecord = { name: string; score: number };

// Инициализираме склада строго за обекти от тип StudentModuleRecord
const repo = new DataRepositoryHub<StudentModuleRecord>();

repo.pushToMemory({ name: "Елизабет", score: 95 });
repo.pushToMemory({ name: "Георги", score: 88 });

// Извличането е 100% типизирано — браузърът знае, че обектът съдържа .score
const evaluatedCandidate = repo.extractFromMemory(0);
console.log(`Курсист: ${evaluatedCandidate.name} | Успех: ${evaluatedCandidate.score}%`);

const textLength = getLengthSecure("Успешен тест на дженерик филтър");
console.log(`Дължина на низа: ${textLength} символа.`);
```
*   **Коментар към кода:** Методът `extends ILengthwise` принуждава функцията да приема единствено стойности, които имат свойство `length` (като стрингове, масиви или обекти с такова поле), блокирайки подаването на чисти числа или булеви променливи още по време на писане.
*   **Очакван компилационен изход в конзолата:**
    ```text
    Курсист: Елизабет | Успех: 95%
    Дължина на низа: 31 символа.
    ```

---
[⬅️ 02-03. OOP в TypeScript](02-03-OOP-in-TypeScript.md) | [📂 Папка: 2-TypeScript-Language](README.md)   | [🏠 Начало](../../../README.md) | [➡️ 02-05. TypeScript Декоратори](02-05-TypeScript-Decorators.md)
