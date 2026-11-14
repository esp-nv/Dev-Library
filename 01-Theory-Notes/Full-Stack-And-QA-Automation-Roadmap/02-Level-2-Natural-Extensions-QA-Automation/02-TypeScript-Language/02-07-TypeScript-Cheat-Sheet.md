[⬅️ 02-06. Инженерен Справочник](../Interview-Handbook/02-06-TypeScript-Traps-And-Snippets.md) | [📂 Папка: 2-TypeScript-Language](README.md) | [🏠 Начало](../../../README.md)

---

# 02-07. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 02:** TypeScript | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за вградени Utility типове, разширени конфигурации, Type Guards и готов за производство асинхронен TypeScript софтуерен компонент.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Справочник на Вградените Манипулационни Типове (Utility Types)](#1-global-reference-guide-справочник-на-вградените-манипулационни-типове-utility-types)
* [2. TYPESCRIPT CORE CHEAT SHEET (Бърз синтактичен пищов)](#2-typescript-core-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI & BACKEND SNIPPET: "Асинхронен Generic Хъб за Стрийминг на Данни"](#3-готов-ui--backend-snippet-асинхронен-generic-хъб-за-стрийминг-на-данни)

---

## 1. GLOBAL REFERENCE GUIDE: Справочник на Вградените Манипулационни Типове (Utility Types)

TypeScript ни предоставя мощни вградени глобални макроси за трансформиране на съществуващи интерфейси в паметта, спестявайки време за ръчно пренаписване на нови DTO структури:
*   **`Partial<T>`** ➔ Превръща автоматично абсолютно всички свойства на обекта `T` в **опционални** (добавя им `?` на ниско ниво). Идеално за `PATCH` HTTP заявки.
*   **`Readonly<T>`** ➔ Налага маркер `readonly` над абсолютно всички полета, правейки обекта напълно имунизиран срещу мутации и пренаписване.
*   **`Pick<T, Keys>`** ➔ Изрязва и конструира изцяло нов олекотен тип, извличайки само посочените ключове (`Keys`) от оригиналната структура.
*   **`Omit<T, Keys>`** ➔ Обратното на Pick — генерира нов тип, като изтрива изрично дефинираните полета от обхвата на обекта.

---

## 2. TYPESCRIPT CORE CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `let input: unknown;` ➔ **Най-сигурният избор за несигурни данни** — изисква твърд `Type Guard` одит преди употреба на методите.
*   `type CustomPredicate = (param: any) => param is string;` ➔ Синтаксис за ръчно стесняване на типове в Runtime през `is` ключевата дума.
*   `as` (Type Assertion) ➔ Насилствено пренаписване на преценката на компилатора (напр. `const canvas = document.getElementById("canvas") as HTMLCanvasElement;`).
*   `readonly string[]` или `ReadonlyArray<string>` ➔ Заковава масива в паметта и блокира употребата на мутиращи методи като `.push()` или `.pop()`.

---

## 3. ГОТОВ UI & BACKEND SNIPPET: "АСИНХРОНЕН GENERIC ХЪБ ЗА СТРИЙМИНГ НА ДАННИ"

Копирай този готов, напълно типизиран, асинхронен Generic софтуерен компонент. Той имплементира сигурни асинхронни нишки през `Promise`, налага `extends` ограничения на дженерик слоевете, борави с `Pick` утилити трансформации и е застрахован против тихи runtime крашове:

```typescript
// 1. Дефиниране на базов корпоративен интерфейс
interface IBaseEntity {
    id: number | string;
    createdAt: Date;
}

// 2. Разширена бизнес структура за Full-Stack одит
interface IStudentEvaluation extends IBaseEntity {
    studentName: string;
    score: number;
    notes?: string;
}

// Извличане на олекотен тип за лог визитки чрез Omit
type EvaluationLogToken = Omit<IStudentEvaluation, "notes" | "createdAt">;

// 3. Изграждане на високопроизводителен Асинхронен Дженерик Стриймър (Generic Data Streamer)
class AsyncDataStreamerHub<T extends IBaseEntity> {
    private activeBuffer: T[] = [];

    constructor(initialData: T[]) {
        this.activeBuffer = initialData;
    }

    // Връща Promise обещание — перфектна съвместимост с C# Task асинхронност
    public async fetchRecordByIdAsync(targetId: number | string): Promise<T> {
        // Симулиране на мрежова латентност (Network Latency) в паметта
        await new Promise(resolve => setTimeout(resolve, 150));

        const locatedNode = this.activeBuffer.find(item => item.id === targetId);

        if (!locatedNode) {
            throw new Error(`[🚨 RUNTIME CRASH] Елементът с ID '${targetId}' липсва в NoSQL масива на RAM паметта.`);
        }

        return locatedNode;
    }
}

// 4. Тестване, задействане и одит на типизираната среда
async function runProductionDiagnosticPipeline() {
    const rawMockData: IStudentEvaluation[] = [
        { id: "TX_101", studentName: "Елизабет", score: 96, createdAt: new Date() },
        { id: "TX_102", studentName: "Георги", score: 89, notes: "Перфектна ООП архитектура", createdAt: new Date() }
    ];

    // Инициализираме хъба строго за обекти, наследяващи IBaseEntity
    const streamProcessor = new AsyncDataStreamerHub<IStudentEvaluation>(rawMockData);

    try {
        console.log("[⏳ МРЕЖОВ ТРАФИК] Извикване на данни от Redis RAM...");
        
        const extractedCandidate = await streamProcessor.fetchRecordByIdAsync("TX_101");
        console.log(`[✅ DATA HIT] Прочетен запис: ${extractedCandidate.studentName} | Оценка: ${extractedCandidate.score}%`);

        // Къстъм трансформация през Пик тип в реално време
        const logToken: EvaluationLogToken = {
            id: extractedCandidate.id,
            studentName: extractedCandidate.studentName,
            score: extractedCandidate.score
        };
        console.log(`[📊 LOG COMPONENT] Генериран лек одит токен: ${JSON.stringify(logToken)}`);

    } catch (runtimeException: any) {
        console.error(runtimeException.message);
    }
}

// Задействане на асинхронния конвейер
runProductionDiagnosticPipeline();
```
*   **Коментар към кода:** Шаблонът `T extends IBaseEntity` гарантира, че дженерик хъбът може да стриймва и филтрира единствено обекти, които притежават свойствата `id` и `createdAt`, застраховайки метода `.find(item => item.id === targetId)` от тихи компилационни сривове.
*   **Очакван компилационен изход в конзолата на браузъра:**
    ```text
    [⏳ МРЕЖОВ ТРАФИК] Извикване на данни от Redis RAM...
    [✅ DATA HIT] Прочетен запис: Елизабет | Оценка: 96%
    [📊 LOG COMPONENT] Генериран лек одит токен: {"id":"TX_101","studentName":"Елизабет","score":96}
    ```

---
[⬅️ 02-06. Инженерен Справочник](02-06-TypeScript-Traps-And-Snippets.md) | [📂 Папка: 2-TypeScript-Language](README.md) | [🏠 Начало](../../../README.md)
