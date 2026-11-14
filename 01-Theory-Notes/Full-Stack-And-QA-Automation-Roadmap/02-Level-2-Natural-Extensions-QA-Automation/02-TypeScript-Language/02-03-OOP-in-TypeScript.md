[⬅️ 02-02. Разширени видове данни](02-02-Advanced-Data-Types.md) | [📂 Папка: 2-TypeScript-Language](README.md)   | [🏠 Начало](../../../README.md) | [➡️ 02-04. Овладяване на Дженерици](02-04-Mastering-Generics.md)

---

# 02-03. Обектно-ориентирано програмагане (OOP) в TypeScript среди

**Модул 02:** TypeScript | **Лекционна тема 03:** Класове, интерфейси (Interfaces), модификатори за достъп, капсулация и абстрактни класове.

---

## 1. АРХИТЕКТУРЕН АНАЛИЗ & ПРЕДИМСТВА И НЕДОСТАТЪЦИ

TypeScript превръща прототипното наследяване на JavaScript в истинска, класическа обектно-ориентирана система, силно напомняща на C# средите. Чрез вграждането на твърди интерфейси и модификатори, езикът позволява на Full-Stack инженерите да прилагат чисти архитектурни патерни (като SOLID принципите) директно във фронтенд слоя.

### 🟢 Предимства (Pros):
*   **Истинска компилационна капсулация:** Модификаторите `private`, `protected` и `public` заковават достъпа до методите още в редактора, спирайки нерегламентирани външни намеси.
*   **Документиране чрез интерфейси (Design by Contract):** Интерфейсите налагат твърди структурни форми (Shape Contracts) над обектите без никакво софтуерно раздуване на крайния JS файл.

### 🔴 Недостатъци (Cons):
*   **Модификаторите изчезват в Runtime (Type Erasure):** Тъй като JavaScript няма native поддръжка за `private`, компилаторът премахва тези думи. Кодът става публичен по време на изпълнение, освен ако не се използват новите нативни JS частни свойства предхождани от знака `#`.
*   **Повишена абстрактност на кодовата база:** Изграждането на дълбока йерархия от интерфейси изисква повече архитектурно планиране и време за разработка.

---

## 2. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД

### ❌ ГРЕШЕН КОД (Анти-патърн със счупена капсулация и липсваща имплементация):
```typescript
interface IRepository {
    saveData(): void;
}

// 🚨 КРАХ: Класът лъже интерфейса — не имплементира метода saveData()!
class BadStudentRepository implements IRepository {
    private connectionString: string = "Server=localhost;";
}

const badRepo = new BadStudentRepository();
// 🚨 КРАХ: Модификаторът private се игнорира в JS и позволява външен прочит на пароли!
console.log(badRepo.connectionString); 
```
*   **Грешка при компилация:** *Class 'BadStudentRepository' incorrectly implements interface 'IRepository'. Property 'saveData' is missing in type 'BadStudentRepository'.*

### 🟢 ПРАВИЛЕН КОД (Строг TypeScript стандарт за ООП Дизайн):
```typescript
// 1. Дефиниране на твърд архитектурен договор (Interface)
interface IEvaluator {
    calculateFinalStatus(): string;
}

// 2. Абстрактен базов клас за капсулация на общи свойства
abstract class TeamMember {
    // Parameter Properties синтакзис — автоматично дефинира и запълва променливите в паметта
    constructor(
        public readonly id: number,
        protected readonly name: string
    ) {}

    abstract printCredentials(): void;
}

// 3. Имплементация на наследяването и интерфейса едновременно
class SeniorStudent extends TeamMember implements IEvaluator {
    private scoreMetrics: number[];

    constructor(id: number, name: string, scores: number[]) {
        super(id, name); // Задължително извикване на базовия конструктор
        this.scoreMetrics = scores;
    }

    // Имплементация на абстрактния метод от родителя
    public printCredentials(): void {
        console.log(`[Инженерна Справка] ID: ${this.id} | Курсист: ${this.name}`);
    }

    // Имплементация на метода от интерфейса IEvaluator
    public calculateFinalStatus(): string {
        const sum = this.scoreMetrics.reduce((acc, curr) => acc + curr, 0);
        const average = sum / this.scoreMetrics.length;
        
        return average >= 80 
            ? "Успешно преминаване към Ниво 2." 
            : "Необходима е допълнителна рефакторизация.";
    }
}

// 4. Извикване и одит на ООП обектите
const candidate: SeniorStudent = new SeniorStudent(105, "Елизабет", [85, 90, 78]);
candidate.printCredentials();

const finalDecision: string = candidate.calculateFinalStatus();
console.log(`Краен Резултат: ${finalDecision}`);
```
*   **Коментар към кода:** Свойството `name` е маркирано като `protected`, което означава, че достъпът до него е разрешен само вътре в наследника `SeniorStudent`, но е напълно забранен за директен външен прочит. Употребата на `readonly` гарантира, че веднъж заложено в конструктора, `id`-то не може да бъде пренаписано.
*   **Очакван компилационен изход в конзолата:**
    ```text
    [Инженерна Справка] ID: 105 | Курсист: Елизабет
    Краен Резултат: Успешно преминаване към Ниво 2.
    ```

---
[⬅️ 02-02. Разширени видове данни](02-02-Advanced-Data-Types.md) | [📂 Папка: 2-TypeScript-Language](README.md)   | [🏠 Начало](../../../README.md) | [➡️ 02-04. Овладяване на Дженерици](02-04-Mastering-Generics.md)
