[⬅️ 02-04-00. Mastering Generics & Constraints](02-04-00-Mastering-Generics.md) | [📂 Текуща папка: 02. TypeScript Language](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 03. Python Scripting](../03-Python-Scripting/README.md) | [➡️ 02-05-00. Decorators, Modules & Async TypeScript](02-05-00-Decorators-Modules-And-Async.md)

---

# 02-04-01. Generics & Conditional Types: Синтаксис от Нулата & Cheat Sheet

**Раздел 02:** Object-Oriented TypeScript & Generics | **Цел:** Практически шаблони от нулата за генерични функции с `keyof` ограничения, генерични интерфейси (`ApiResponse<T>`), **Conditional Types & филтриране с `never`**, готовия за продукция **`AsyncDataStreamerHub<T extends IBaseEntity>`** и 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Генерични функции с `keyof` ограничение и Генерични интерфейси](#1-генерични-функции-с-keyof-ограничение-и-генерични-интерфейси)
* [2. Conditional Types и Филтриране на полета с `never` (Вход / Изход)](#2-conditional-types-и-филтриране-на-полета-с-never-вход--изход)
* [3. Готов за продукция шаблон: `AsyncDataStreamerHub<T extends IBaseEntity>`](#3-готов-за-продукция-шаблон-asyncdatastreamerhubt-extends-ibaseentity)
* [4. Обобщение / Бърз Cheat Sheet](#4-обобщение--бърз-cheat-sheet)

---

## 1. ГЕНЕРИЧНИ ФУНКЦИИ С `KEYOF` ОГРАНИЧЕНИЕ И ГЕНЕРИЧНИ ИНТЕРФЕЙСИ

```typescript
// 1. Безопасно извличане на свойство от произволен обект чрез <T, K extends keyof T>
function getProperty<T, K extends keyof T>(obj: T, key: K): T[K] {
    return obj[key];
}

const user = { id: 101, username: "elena_ts", isActive: true };
const nameValue = getProperty(user, "username"); // Типът автоматично е 'string'!
// getProperty(user, "nonExistent");             // ❌ Грешка при компилация!

// 2. Генеричен интерфейс за стандартизирани REST API отговори
interface ApiResponse<TData, TMeta = { timestamp: string }> {
    success: boolean;
    data: TData;
    meta?: TMeta;
}

const userResponse: ApiResponse<{ id: number; email: string }> = {
    success: true,
    data: { id: 101, email: "elena@devlib.bg" }
};
```

---

## 2. CONDITIONAL TYPES И ФИЛТРИРАНЕ НА ПОЛЕТА С `NEVER` (ВХОД / ИЗХОД)

* **Вход (Дефиниция на условни типове и филтър):**
  ```typescript
  // 1. Базов условен тип (Conditional Type)
  type IsString<T> = T extends string ? "Да" : "Не";
  type Check1 = IsString<string>; // "Да"
  type Check2 = IsString<number>; // "Не"

  // 2. Извличане САМО на имената на числовите свойства от даден тип
  type Employee = {
      name: string;
      age: number;
      salary: number;
  };

  type NumberKeysOnly<T> = {
      [K in keyof T]: T[K] extends number ? K : never;
  }[keyof T];

  type EmployeeNumericFields = NumberKeysOnly<Employee>;
  ```
* **Изход (Изчислен тип в компилатора):**
  ```typescript
  // Резултат: "age" | "salary" (полето "name" е премахнато чрез 'never')
  ```

---

## 3. ГОТОВ ЗА ПРОДУКЦИЯ ШАБЛОН: `ASYNCDATASTREAMERHUB<T EXTENDS IBASEENTITY>`

```typescript
interface IBaseEntity {
    id: number | string;
    createdAt: Date;
}

interface IStudentEvaluation extends IBaseEntity {
    studentName: string;
    score: number;
    notes?: string;
}

type EvaluationLogToken = Omit<IStudentEvaluation, "notes" | "createdAt">;

class AsyncDataStreamerHub<T extends IBaseEntity> {
    private activeBuffer: T[] = [];

    constructor(initialData: T[]) {
        this.activeBuffer = initialData;
    }

    public async fetchRecordByIdAsync(targetId: number | string): Promise<T> {
        await new Promise(resolve => setTimeout(resolve, 150));

        const locatedNode = this.activeBuffer.find(item => item.id === targetId);
        if (!locatedNode) {
            throw new Error(`[🚨 RUNTIME CRASH] Елементът с ID '${targetId}' липсва в паметта.`);
        }
        return locatedNode;
    }
}

async function runProductionDiagnosticPipeline() {
    const rawMockData: IStudentEvaluation[] = [
        { id: "TX_101", studentName: "Елизабет", score: 96, createdAt: new Date() },
        { id: "TX_102", studentName: "Георги", score: 89, notes: "Перфектна ООП архитектура", createdAt: new Date() }
    ];

    const streamProcessor = new AsyncDataStreamerHub<IStudentEvaluation>(rawMockData);

    try {
        const extractedCandidate = await streamProcessor.fetchRecordByIdAsync("TX_101");
        console.log(`[✅ DATA HIT] Прочетен запис: ${extractedCandidate.studentName} | Оценка: ${extractedCandidate.score}%`);

        const logToken: EvaluationLogToken = {
            id: extractedCandidate.id,
            studentName: extractedCandidate.studentName,
            score: extractedCandidate.score
        };
        console.log(`[📊 LOG COMPONENT] Генериран лек одит токен: ${JSON.stringify(logToken)}`);
    } catch (runtimeException: unknown) {
        if (runtimeException instanceof Error) {
            console.error(runtimeException.message);
        }
    }
}

runProductionDiagnosticPipeline();
```

---

## 4. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Шаблон | Синтаксис в TypeScript | Предназначение |
| :--- | :--- | :--- |
| **Генерична функция** | `function wrap<T>(val: T): T[] { return [val]; }` | Създава функция, която запазва типа на подадения аргумент. |
| **Генеричен клас** | `class Cache<T> { private items: T[] = []; }` | Типизирана структура от данни (опашка, стек, хранилище). |
| **Ограничение (`extends`)** | `<T extends { id: number }>` | Изисква типът `T` да притежава поне посочените свойства. |
| **Ограничение по ключ** | `<T, K extends keyof T>(obj: T, key: K): T[K]` | Гарантира, че `key` е валидно поле на обекта `obj`. |
| **Default Generic Type** | `interface Page<T = unknown> { items: T[]; }` | Задава тип по подразбиране, ако не се подаде изрично. |
| **Conditional Type** | `T extends U ? TrueType : FalseType` | Условна логика на ниво типове при компилация. |
| **Филтриране с `never`** | `{ [K in keyof T]: T[K] extends V ? K : never }[keyof T]` | Извлича само ключовете, чиито стойности отговарят на тип `V`. |

---

[⬅️ 02-04-00. Mastering Generics & Constraints](02-04-00-Mastering-Generics.md) | [📂 Текуща папка: 02. TypeScript Language](README.md) | [🏠 Начало](../../../README.md) | [📂 Следваща папка: 03. Python Scripting](../03-Python-Scripting/README.md) | [➡️ 02-05-00. Decorators, Modules & Async TypeScript](02-05-00-Decorators-Modules-And-Async.md)

© 2026 Dev-Library / 02-04-01. Generics & Conditional Types: Синтаксис от Нулата & Cheat Sheet
