[⬅️ 02-04-01. Generics & Conditional Types Cheat Sheet](02-04-01-Mastering-Generics-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-05-01. Decorators, Modules & Async Cheat Sheet](02-05-01-Decorators-Modules-And-Async-Syntax.md)

---

# 02-05-00. Decorators, ES Modules & Async TypeScript (AOP, Metadata & Капани)

**Раздел 03:** Decorators, Modules & Async TypeScript | **Цел:** Овладяване на метапрограмирането и Аспектно-ориентираното програмиране (**AOP**) в TypeScript чрез декоратори на класове, методи и аксесори, реда на композиция на множество декоратори, четенето на типове в Runtime чрез **`reflect-metadata`**, модулната система (`import`/`export`, `.d.ts` декларационни файлове) и типизирания асинхронен поток (`Promise<T>`, `async/await`).

## 🧭 Съдържание на темата
* [1. Как да се запомни по-лесно (Мнемоника: AOP, Ред на композиция & .d.ts Преводач)](#1-как-да-се-запомни-по-лесно-мнемоника-aop-ред-на-композиция--dts-преводач)
* [2. Архитектура на Method Decorator и Блок-схема (Прихващане на нишката)](#2-архитектура-на-method-decorator-и-блок-схема-прихващане-на-нишката)
* [3. Сравнителни матрици: Видове Декоратори & TypeScript Decorators срещу C# Attributes](#3-сравнителни-матрици-видове-декоратори--typescript-decorators-срещу-c-attributes)
* [4. Плюсове и минуси на Декораторите и `reflect-metadata`](#4-плюсове-и-минуси-на-декораторите-и-reflect-metadata)
* [5. Кодов одит: Грешен (Дублиран лог спам в бизнес метода) срещу Правилен (`@LogTransaction` AOP)](#5-кодов-одит-грешен-дублиран-лог-спам-в-бизнес-метода-срещу-правилен-logtransaction-aop)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ ПО-ЛЕСНО (МНЕМОНИКА: AOP, РЕД НА КОМПОЗИЦИЯ & .D.TS ПРЕВОДАЧ)

* **Декораторът е като прозрачна обвивка за подарък (AOP Wrapper):** Не отваряш кутията и не променяш кода вътре в метода `executeTransfer()`, а увиваш метода в декоратор `@LogTransaction()`, който автоматично записва лог преди и след изпълнението му.
* **Редът на изпълнение на множество декоратори (Асансьор надолу ➔ Асансьор нагоре):**
  ```typescript
  @dec1() // 1. Фабриката dec1 се оценява ПЪРВА (отгоре надолу)
  @dec2() // 2. Фабриката dec2 се оценява ВТОРА (отгоре надолу)
  run() {} // 3. Прилага се dec2(run), и накрая dec1(dec2(run)) (отдолу нагоре!)
  ```
* **Файлът `.d.ts` (Двуезичен речник за стари JS библиотеки):** Когато инсталираш стара npm библиотека, написана на суров JavaScript, TypeScript не знае какви типове очакват функциите ѝ. Декларационният файл `index.d.ts` съдържа **само типови сигнатури без имплементация** и служи като преводач между JS библиотеката и TS компилатора.

---

## 2. АРХИТЕКТУРА НА METHOD DECORATOR И БЛОК-СХЕМА (ПРИХВАЩАНЕ НА НИШКАТА)

```text
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  ИЗВИКВАНЕ: coreProcessor.executeTransfer("BG_AUTH_8912", 350)          │
 └────────────────────────────────────┬────────────────────────────────────┘
                                      │
                                      ▼
 ┌─────────────────────────────────────────────────────────────────────────┐
 │  @LogTransaction() WRAPPER (PropertyDescriptor.value)                   │
 │  1. [Преди метода]: console.log("[🚀 AUDIT] Извикване с аргументи...")   │
 │  2. [Оригинал]:     originalMethod.apply(this, args) ──► Бизнес логика  │
 │  3. [След метода]:  console.log("[✅ AUDIT] Успешен край...")            │
 └─────────────────────────────────────────────────────────────────────────┘
```

---

## 3. СРАВНИТЕЛНИ МАТРИЦИ: ВИДОВЕ ДЕКОРАТОРИ & TYPESCRIPT DECORATORS СРЕЩУ C# ATTRIBUTES

### 📊 А. Видове декоратори в TypeScript

| Вид декоратор | Къде се поставя? | Какви параметри получава функцията? | Типично приложение (Angular / NestJS) |
| :--- | :--- | :--- | :--- |
| **Class Decorator** | Над `class User { ... }` | `(constructor: Function)` | `@Component`, `@Injectable`, `@Controller`, добавяне на полета. |
| **Method Decorator** | Над метод в клас | `(target, propertyKey, descriptor)` | `@Get('/users')`, `@LogTransaction()`, кеширане, измерване на време. |
| **Accessor Decorator** | Над `get` или `set` | `(target, propertyKey, descriptor)` | `@ValidateType` за проверка на типове в Runtime. |
| **Property Decorator** | Над поле в клас | `(target, propertyKey)` | `@Input()`, `@Column()`, декларативна валидация (`@IsEmail()`). |
| **Parameter Decorator** | Пред параметър в метод | `(target, propertyKey, parameterIndex)` | `@Body()`, `@Param('id')`, `@Inject()` в NestJS. |

### 📊 Б. TypeScript Decorators срещу C# Attributes (`[Attribute]`)

| Критерий | 🔷 TypeScript Decorators (`@Dec`) | 🟣 C# Attributes (`[Attr]`) |
| :--- | :--- | :--- |
| **Какво представляват?** | **Реални JavaScript функции**, които се изпълняват при зареждане на класа. | **Пасивни метаданни**, записани в компилирания `.dll` (IL код). |
| **Могат ли да променят кода?** | ✅ **Да** — директно обвиват или подменят методите и конструктора. | ❌ **Не** — четат се отвън чрез Reflection (`GetCustomAttribute`). |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ДЕКОРАТОРИТЕ И `REFLECT-METADATA`

### 🟢 Предимства (Pros):
* **Чисто разделение на отговорностите (Separation of Concerns):** Бизнес логиката остава освободена от логване, одит, трансакции и проверки за права.
* **Основа на Enterprise рамките:** Цялата архитектура на **Angular**, **NestJS** и **TypeORM** стъпва върху декоратори и `reflect-metadata` за Dependency Injection.

### 🔴 Недостатъци (Cons):
* **Не се наследяват автоматично:** Ако декорираш базов клас `BaseController`, неговите наследници **не** получават декоратора автоматично — всеки дъщерен клас трябва да бъде декориран изрично.
* **Специфични настройки в `tsconfig.json`:** За класическите експериментални декоратори (използвани от NestJS и Angular) се изискват `"experimentalDecorators": true` и `"emitDecoratorMetadata": true`.

---

## 5. КОДОВ ОДИТ: ГРЕШЕН (ДУБЛИРАН ЛОГ СПАМ) СРЕЩУ ПРАВИЛЕН (`@LOGTRANSACTION` AOP)

### ❌ ГРЕШЕН КОД (Ръчно набиване на лог нишки във всеки бизнес метод):
```typescript
class AccountService {
    public updateBalance(amount: number) {
        // 🚨 ДЕФЕКТ: Силно дублиране на код и размиване на бизнес логиката с логове!
        console.log(`[ОДИТ ТРАФИК] Стартиране на метод updateBalance с параметър ${amount}`);

        // Реална бизнес операция...
        console.log("[ОДИТ ТРАФИК] КРАЙ: Успешно изпълнение на операцията.");
    }
}
```

### 🟢 ПРАВИЛЕН КОД (Аспектен Method Decorator `@LogTransaction()`):
```typescript
function LogTransaction() {
    return function (
        target: any,
        propertyKey: string,
        descriptor: PropertyDescriptor
    ) {
        const originalMethod = descriptor.value;

        descriptor.value = function (...args: any[]) {
            console.log(`[🚀 AUDIT LOG] Извикване на метод: '${propertyKey}' с аргументи: ${JSON.stringify(args)}`);
            const result = originalMethod.apply(this, args);
            console.log(`[✅ AUDIT LOG] Успешен край на трансакцията за '${propertyKey}'.`);
            return result;
        };

        return descriptor;
    };
}

class FinancialController {
    private cachedBalance: number = 1500;

    @LogTransaction()
    public executeTransfer(accountDto: string, amount: number): void {
        this.cachedBalance -= amount;
        console.log(`[Бизнес Логика] Трансферът от ${amount} лв. към ${accountDto} е обработен.`);
    }
}

const coreProcessor = new FinancialController();
coreProcessor.executeTransfer("BG_AUTH_8912739", 350);
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

* **Въпрос 1:** *„Как да оправим грешката `Could not find a declaration file for module 'old-js-lib'` при инсталиране на стара JavaScript библиотека в TypeScript проект?“*
  * **Отговор:** Има два инженерни подхода:
    1. Проверяваме в репозиторито *DefinitelyTyped* и инсталираме типовите дефиниции чрез `npm install --save-dev @types/old-js-lib`.
    2. Ако пакетът няма публикувани типове, създаваме файл `declarations.d.ts` в папката `src/` и добавяме декларация `declare module 'old-js-lib';` (или описваме точните сигнатури на функциите му).
* **Въпрос 2:** *„Какъв е типът на променливата `error` в блока `try { ... } catch (error) { ... }` в модерен TypeScript (`strict: true`) и как се обработва безопасно?“*
  * **Отговор:** От TypeScript 4.4+ типът по подразбиране в `catch` блока е **`unknown`** (а не `any`), защото в JavaScript може да бъде хвърлено всичко (`throw new Error(...)`, `throw "грешка"` или `throw 404`). За да достъпим `error.message` безопасно, задължително проверяваме `if (error instanceof Error) { console.error(error.message); }`.

---

[⬅️ 02-04-01. Generics & Conditional Types Cheat Sheet](02-04-01-Mastering-Generics-Syntax.md) | [⬅️ Назад към  модул Responsive-Forms-And-Frameworks](../Responsive-Forms-And-Frameworks/README.md) | [⬅️ Назад към  02-Frontend-And-UI-UX](../README.md) [📚 Инженерна Библиотека 01-Theory-Notes](../../README.md) | [🏠 Начало Dev-Library](../../../README.md) | [📂 Текуща папка: TypeScript Language](README.md) | [📂 Следващ домейн: 03-Backend-Ecosystems](../03-Backend-Ecosystems/README.md) | [📂 Следващ модул: C-Sharp-And-ASP-NET/C-Sharp-Fundamentals](../../03-Backend-Ecosystems/C-Sharp-And-ASP-NET/C-Sharp-Fundamentals/README.md)  | [➡️ 02-05-01. Decorators, Modules & Async Cheat Sheet](02-05-01-Decorators-Modules-And-Async-Syntax.md)

© 2026 Dev-Library / 02-05-00. Decorators, ES Modules & Async TypeScript (AOP, Metadata & Капани)
