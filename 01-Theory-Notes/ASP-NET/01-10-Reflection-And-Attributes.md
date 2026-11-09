[⬅️ 01-09. Справочник по конфигурация](01-09-AutoMapper-Identity-Configuration-Guide.md) | 📂 Глава 01. ASP.NET Core Fundamentals | [🏠 Начало (ASP.NET)](README.md)

---

# 01-10. Reflection и Attributes (Дълбок Архитектурен Справочник)

**Цел:** Пълно разглобяване на концепциите за Метапрограмиране, инспекция на метаданни по време на изпълнение (Reflection API), динамично извикване на конструктори и методи и изграждане на Къстъм Атрибути.

## 🧭 Съдържание на темата
* [1. Какво е Метапрограмиране и Reflection (Кога и защо се използва)](#1-какво-е-метапрограмиране-и-reflection-кога-и-защо-се-използва)
* [2. Работа с класа Type и Динамично заделяне на памет (Activator)](#2-работа-с-класа-type-и-динамично-заделяне-на-памет-activator)
* [3. Инспекция и модификация на Полета, Конструктори и Методи](#3-инспекция-и-модификация-на-полета-конструктори-и-методи)
* [4. Създаване на Къстъм Атрибути (Custom Attributes Requirements)](#4-създаване-на-къстъм-атрибути-custom-attributes-requirements)
* [5. Системни капани: Грешки при достъп до Private памет и Корекция](#5-системни-капани-грешки-при-достъп-до-private-памет-и-корекция)

---

## 1. КАКВО Е МЕТАПРОГРАМИРАНЕ И REFLECTION (КОГА И ЗАЩО СЕ ИЗПОЛЗВА)

*   **Метапрограмиране (Metaprogramming):** Програмна техника, при която компютърните програми имат способността да разглеждат и третират други програми (или себе си) като свои собствени данни. Това позволява на софтуера да чете, анализира, трансформира или модифицира своята структура в движение, докато работи.
*   **Reflection (Рефлексия):** Конкретната имплементация на метапрограмирането в .NET средите. Това е способността на езика C# да изследва собствените си метаданни и типове по време на изпълнение (Runtime).

### ⚖️ Плюсове и Минуси (Trade-offs) на Рефлексията:
*   **Кога СЕ използва:** При изграждане на плъгин архитектури, автоматични уеб мапери (като AutoMapper), Dependency Injection IoC контейнери (които трябва да откриват конструктори автоматично) и системи за Unit Testing.
*   **Кога НЕ се използва:** Ако дадена операция може да се извърши по класически синхронен път, рефлексията трябва да се избягва. Тя носи сериозно забавяне на производителността (`Performance overhead`), заобикаля операционната сигурност (`Security restrictions`) и излага на показ скрити вътрешни компоненти на софтуера (`Exposure of internals`).

---

## 2. РАБОТА С КЛАСА TYPE И ДИНАМИЧНО ЗАДЕЛЯНЕ НА ПАМЕТ (ACTIVATOR)

Класът **`Type`** е централният входен пункт за достъп до метаданните на всеки обект. Можем да го извлечем по три начина:

```csharp
Type t1 = typeof(Student); // 1. По време на компилация (ако знаем името)
Type t2 = Type.GetType("SchoolSystem.Data.Entities.Student"); // 2. По време на изпълнение чрез стринг
Type t3 = instance.GetType(); // 3. Чрез жива инстанция в паметта
```

Чрез статичния двигател **`Activator.CreateInstance`** можем динамично да създадем празен обект в Heap паметта на компютъра само по неговия уловен `Type` метаданък:

### 📄 C# Код за динамична инстанция:
```csharp
using System;
using System.Text;

public class ActivationDemo
{
    public static void Run()
    {
        // Взимаме метаданните за класа StringBuilder през уеб стринг
        Type sbType = Type.GetType("System.Text.StringBuilder")!; // 0.1.220

        // Динамично активираме празен конструктор и заделяме памет
        StringBuilder sbInstance = (StringBuilder)Activator.CreateInstance(sbType)!; // 0.1.220
        
        sbInstance.Append("Успешно активиран!");
        Console.WriteLine(sbInstance.ToString());
    }
}
```

#### 🎯 Очакван изход (Expected Output в конзолата):
```text
Успешно активиран!
```

---

## 3. ИНСПЕКЦИЯ И МОДИФИКАЦИЯ НА ПОЛЕТА, КОНСТРУКТОРИ И МЕТОДИ

Чрез Reflection API можем да пробием капсулацията на всеки клас и да прочетем или променим неговите елементи през флагове за достъп (**`BindingFlags`**).

### А. Модифициране на скрити Полета (Fields)
```csharp
using System;
using System.Reflection;

public class Account
{
    private int secretBalance = 100; // 🔒 Частно капсулирано поле
}

public class FieldReflection
{
    public static void BreakCapsulation()
    {
        Account myAccount = new Account();
        Type accountType = typeof(Account);

        // Налагаме BindingFlags филтри, за да уловим частното NonPublic поле
        FieldInfo field = accountType.GetField("secretBalance", BindingFlags.Instance | BindingFlags.NonPublic)!; // 0.1.221

        // Ръчно пренаписваме стойността на диска директно в обекта!
        field.SetValue(myAccount, 50000); // 0.1.224

        // Извличаме новата стойност
        int updatedValue = (int)field.GetValue(myAccount)!; // 0.1.224
        Console.WriteLine($"Нов баланс по сметката: {updatedValue}");
    }
}
```

#### 🎯 Очакван изход (Expected Output в конзолата):
```text
Нов баланс по сметката: 50000
```

### Б. Извикване на специфични Конструктори и Методи
```csharp
using System;
using System.Reflection;

public class Calculator
{
    private int Multiply(int a, int b) => a * b;
}

public class MethodReflection
{
    public static void RunPrivateMethod()
    {
        Calculator calc = new Calculator();
        Type calcType = typeof(Calculator);

        // Улавяме private метода през име и филтри
        MethodInfo method = calcType.GetMethod("Multiply", BindingFlags.Instance | BindingFlags.NonPublic)!; // 0.1.228

        // Извикваме метода насила, подавайки обекта и масив от аргументи
        object result = method.Invoke(calc, new object[] { 5, 6 })!; // 0.1.229

        Console.WriteLine($"Резултат от скрития метод: {result}");
    }
}
```

#### 🎯 Очакван изход (Expected Output в конзолата):
```text
Резултат от скрития метод: 30
```

---

## 4. СЪЗДАВАНЕ НА КЪСТЪМ АТРИБУТИ (CUSTOM ATTRIBUTES REQUIREMENTS)

**Атрибутите** са специални класове за съхранение на допълнителни метаданни, които описват нашия C# код (класове, методи, полета) и могат да бъдат четени по време на изпълнение. Всеки къстъм атрибут трябва:
1. Да наследява базовия клас `System.Attribute`.
2. Името на класа му задължително да завършва с думичката **`Attribute`**.
3. Да дефинира обхвата си чрез атрибута **`[AttributeUsage]`**.

### 📄 1. Софтуерен код на Атрибута и Тракера (По SoftUni изпитен модел):
```csharp
using System;
using System.Reflection;
using System.Linq;

// Стъпка 1: Дефинираме атрибута
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)] // 0.1.237
public class AuthorAttribute : Attribute // 0.1.237
{
    public string Name { get; set; }
    public AuthorAttribute(string name) { this.Name = name; } // 0.1.237
}

// Стъпка 2: Прилагаме го над кодови структури
[Author("Victor")] // 0.1.236
public class StartUp
{
    [Author("Georgi")] // 0.1.236
    public static void CoreExecution() { }
}

// Стъпка 3: Сканираме метаданните чрез Рефлексия
public class CodingTracker
{
    public static void PrintMethodsByAuthor()
    {
        Type targetType = typeof(StartUp); // 0.1.239
        var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance); // 0.1.239

        foreach (var method in methods)
        {
            // Проверяваме дали методът притежава нашия атрибут
            if (method.CustomAttributes.Any(a => a.AttributeType == typeof(AuthorAttribute))) // 0.1.239
            {
                var attributes = method.GetCustomAttributes(false); // 0.1.239
                foreach (AuthorAttribute attr in attributes) // 0.1.239
                {
                    Console.WriteLine($"Методът '{method.Name}' е разписан от автора: {attr.Name}"); // 0.1.239
                }
            }
        }
    }
}
```

#### 🎯 Очакван изход (Expected Output в конзолата):
```text
Методът 'CoreExecution' е разписан от автора: Georgi
```

---

## 5. СИСТЕМНИ КАПАНИ: ГРЕШКИ ПРИ ДОСТЪП ДО PRIVATE ПАМЕТ И КОРЕКЦИЯ

Неправилното конфигуриране на флаговете за търсене води до фатални сривове в Runtime паметта.

### 🚨 Капан №1: Получаване на NullReferenceException при грешен BindingFlag
*   **Какво правиш:** Опитваш се да извлечеш частно поле през метода `GetField("име")`, но в скобите не подаваш никакви флагове, мислейки че .NET ще го намери сам:
    ```csharp
    // ❌ АНТИ-ПАТЕРН (Ще върне NULL и ще срине следващия ред!):
    FieldInfo field = type.GetField("secretBalance"); 
    field.SetValue(instance, 200); 
    ```
*   **Точен софтуерен Crash (Exception Message) в конзолата:**
    ```text
    NullReferenceException: Object reference not set to an instance of an object.
    ```
*   **Защо се случва:** По подразбиране, ако не подадеш флагове, методът `GetField()` сканира **ЕДИНСТВЕНО публични полета (Public)**. Когато се опитва да намери `private` променлива, той се проваля и връща стойност `null`. Опитът за ползване на `null` обект срива виртуалната машина.
*   **Как да го поправиш:** Винаги, когато търсиш скрита памет, комбинираш филтрите чрез побитов ИЛИ (`|`) оператор: `BindingFlags.Instance | BindingFlags.NonPublic`, за да укажеш точния достъп.

---
[⬅️ 01-09. Справочник по конфигурация](01-09-AutoMapper-Identity-Configuration-Guide.md) | 📂 Глава 01. ASP.NET Core Fundamentals | [🏠 Начало (ASP.NET)](README.md)
---
© 2026 Dev-Library / Справочник: Reflection & Attributes Пищов