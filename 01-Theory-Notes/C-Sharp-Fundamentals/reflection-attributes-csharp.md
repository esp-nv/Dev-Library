// ============================================================
// C# REFLECTION & ATTRIBUTES — АРХИТЕКТУРЕН УЧЕБНИК
// ============================================================
// 🧠 ИДЕЯТА: Не гледаме на кода само като текст. Знаем как
// .NET зарежда метаданните в RAM и как да ги изследваме в движение.
// ============================================================

// ============================================================
// 1. КАКВО Е МЕТАДАННИ (METADATA) И РЕФЛЕКСИЯ?
// ============================================================
// Когато компилираш проект (напр. StudentManagement), C# генерира 
// джаджа, наречена Assembly (.dll или .exe файл).
// Този файл съдържа:
//   1. Intermediate Language (IL) Код -> Инструкции за процесора.
//   2. Metadata Tables -> Списък с абсолютно всяко име на клас,
//      свойство, метод, модификатор за достъп и забит атрибут.
//
// Рефлексията (Reflection) е софтуерният скенер, който ти позволява
// по време на изпълнение (Runtime) да прочетеш тези метаданни.
//
// ЗАПОМНИ:
// Compile-time -> Знаеш името на класа и го пишеш твърдо в кода.
// Runtime -> Програмата сама открива типа и неговата структура в паметта.
// ============================================================

// ============================================================
// 2. АНАТОМИЯ НА КЛАСА TYPE (Паспортът на обекта)
// ============================================================
// КОГА СЕ ПОЛЗВА?
// -> Когато искаш да вземеш пълната схема на даден софтуерен модел.

using System;
using System.Reflection;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    private string _secretToken = "STUDENT_2026_SECRET";

    public void DisplayInfo() => Console.WriteLine($"{Id}: {Name}");
    private void UpdateToken(string newToken) => _secretToken = newToken;
}

public class TypeExploration
{
    public void Run()
    {
        // Начин А: Чрезtypeof() - работи директно с името на типа (Бързо и Compile-time защитено)
        Type t1 = typeof(Student);

        // Начин Б: Чрез .GetType() - работи върху конкретна инстанция в Heap паметта
        Student instance = new Student { Id = 1, Name = "Мария" };
        Type t2 = instance.GetType();

        // И двата начина сочат към ЕДИН И СЪЩ Type обект в системната памет
        Console.WriteLine($"Име на класа: {t1.Name}");        // Резултат: Student
        Console.WriteLine($"Пълно име: {t1.FullName}");      // Резултат: Namespace.Student
        Console.WriteLine($"Референтен тип ли е?: {t1.IsClass}"); // Резултат: True
    }
}
// ЗАПОМНИ:
// Type обектът съдържа софтуерната матрица. Той не съдържа самите данни на студента.
// ============================================================

// ============================================================
// 3. ИЗВЛИЧАНЕ НА ЧЛЕНОВЕ (Properties, Methods, Fields)
// ============================================================
// Рефлексията позволява да разглобиш структурата на парчета.

public class StructuralInspector
{
    public void Inspect()
    {
        Type studentType = typeof(Student);

        // 1. Извличане на всички свойства (Properties)
        PropertyInfo[] properties = studentType.GetProperties();
        foreach (PropertyInfo prop in properties)
        {
            Console.WriteLine($"Свойство: {prop.Name} от тип {prop.PropertyType.Name}");
        }

        // 2. Извличане на методи (Methods)
        MethodInfo[] methods = studentType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            Console.WriteLine($"Публичен метод: {method.Name} с върнат тип {method.ReturnType.Name}");
        }
    }
}
// ПЛЮСОВЕ:
// -> Позволява изграждане на динамични автоматизации (мапинги, сериализации).
// МИНУСИ:
// -> GetProperties() извършва тежко претърсване на низове в паметта.
// ============================================================

// ============================================================
// 4. ДИНАМИЧНО ИНСТАНЦИРАНЕ (Activator.CreateInstance)
// ============================================================
// КОГА?
// -> Имаш името на класа като текст от конфигурационен файл и искаш да направиш обект от него.

public class DynamicFactory
{
    public object CreateObjectDynamically(string fullClassName)
    {
        // 1. Намираме типа в текущо зареденото Assembly
        Type targetType = Type.GetType(fullClassName);
        
        if (targetType == null)
        {
            throw new TypeLoadException($"Класът {fullClassName} не е намерен в метаданните!");
        }

        // 2. Създаваме обекта без ключовата дума 'new'
        // Под капака .NET намира конструктора по подразбиране и заделя памет в Heap-а
        object newInstance = Activator.CreateInstance(targetType);

        return newInstance;
    }
}
// НАЙ-ЧЕСТА ГРЕШКА:
// Подаване на клас, който няма безпараметричен конструктор -> Гърми с MissingMethodException.
// ЗАПОМНИ:
// Activator.CreateInstance винаги връща базов 'object'. Трябва да го кастнеш или да работиш динамично.
// ============================================================
// ============================================================
// 5. ПРОБИВАНЕ НА КАПСУЛАЦИЯТА (Breaking Encapsulation)
// ============================================================
// ⚠️ ВНИМАНИЕ: Това показва мощността на Рефлексията. Тя може да прескочи 
// модификаторите 'private' и да чете/пише скрито състояние в RAM паметта.

public class EncapsulationBreaker
{
    public void BreakIntoPrivateFields()
    {
        // 1. Създаваме нормален обект Student
        Student targetStudent = new Student { Id = 42, Name = "Алекс" };

        Type type = targetStudent.GetType();

        // 2. Намираме частното поле _secretToken чрез BindingFlags
        FieldInfo privateField = type.GetField("_secretToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        // 3. Извличаме скритата стойност от конкретната инстанция в Heap-а
        string oldValue = (string)privateField.GetValue(targetStudent);
        Console.WriteLine($"Стойност под капака: {oldValue}"); // Резултат: STUDENT_2026_SECRET

        // 4. Променяме private променливата СИЛОВО директно в паметта!
        privateField.SetValue(targetStudent, "HACKED_TOKEN_2026");
        
        // 5. Намираме и извикваме private метод по неговото string име
        MethodInfo privateMethod = type.GetMethod("UpdateToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
            
        privateMethod.Invoke(targetStudent, new object[] { "NEW_REFLECTED_TOKEN" });
    }
}
// ЗАПОМНИ:
// Рефлексията може да чете всичко в рамките на твоя процес. Модификаторите 
// private/protected са компилаторна защита, но Рефлексията ги заобикаля в Runtime.
// ============================================================

// ============================================================
// 6. CUSTOM ATTRIBUTES (Декоративни етикети)
// ============================================================
// КОГА СЕ ПОЛЗВА?
// -> Когато искаш да забиеш допълнителни метаданни за даден метод/клас,
//    които системата да анализира по-късно автоматично.

// 1. Дефиниране на Атрибут (Винаги наследява System.Attribute)
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)] 
public class ValidateRangeAttribute : Attribute
{
    public double Min { get; }
    public double Max { get; }

    public ValidateRangeAttribute(double min, double max)
    {
        Min = min;
        Max = max;
    }
}

// 2. Реален модел в StudentManagement, маркиран с етикета
public class Course
{
    public string Title { get; set; }

    [ValidateRange(2.0, 6.0)] // Маркираме, че оценката по курса има граници
    public double FinalGrade { get; set; }
}

// 3. СКЕНЕРЪТ (Енджинът), който чете етикета в Runtime и налага бизнес логика
public class AttributeValidationEngine
{
    public bool ValidateObject(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo prop in properties)
        {
            // Проверяваме дали свойството притежава нашия етикет
            ValidateRangeAttribute attribute = prop.GetCustomAttribute<ValidateRangeAttribute>();

            if (attribute != null)
            {
                double value = (double)prop.GetValue(obj);
                
                // Извличаме лимитите директно от етикета (метаданните)
                if (value < attribute.Min || value > attribute.Max)
                {
                    Console.WriteLine($"❌ Грешка: Свойството {prop.Name} е извън лимита!");
                    return false;
                }
            }
        }
        return true;
    }
}
// ЗАПОМНИ:
// Сам по себе си атрибутът е просто пасивен стикер/маркер. Той се превръща в код
// едва когато друг компонент го извика чрез методи от типа на .GetCustomAttribute().
// ============================================================

// ============================================================
// 7. СРАВНЕНИЕ И ТИПОВЕ РЕЗУЛТАТ — КРАТКО
// ============================================================
/* 
Операция                Връщан тип       Цел
typeof(T)               Type             Паспорт на типа по име (Compile-time)
obj.GetType()           Type             Паспорт на типа по инстанция (Runtime)
GetProperties()         PropertyInfo[]   Списък от свойства (вътрешни кутии)
GetMethods()            MethodInfo[]     Списък от методи (вътрешни действия)
GetCustomAttribute<T>() T (Attribute)    Извличане на прикачен етикет
*/
// ============================================================

// ============================================================
// 8. НАЙ-ЧЕСТИ ГРЕШКИ И JUNIOR КАПАНИ
// ============================================================
// ❌ 1. Сляпо използване на "Magic Strings" в търсенето
// Type t = typeof(Student);
// MethodInfo m = t.GetMethod("Displayinfo"); // ❌ Грешка! Малка буква 'i'. Връща null.
// m.Invoke(obj, null); // ❌ БУУМ! NullReferenceException в Runtime.
// 
// -> РЕШЕНИЕ: Използвай оператора nameof(), за да имаш компилаторна проверка:
// MethodInfo m = t.GetMethod(nameof(Student.DisplayInfo));

// ❌ 2. Поставяне на Рефлексия в критични цикли (Performance Crash)
// 
// foreach (var student in largeStudentList)
// {
//     // ❌ УЖАСЕН ПОДХОД: Кодът отива милиони пъти до RAM таблиците с метаданни!
//     var props = typeof(Student).GetProperties(); 
// }
// 
// -> РЕШЕНИЕ: Извлечи свойствата само веднъж извън цикъла и ги кеширай в масив или списък.

// ❌ 3. Промяна на readonly / backing полета без нужда
// Заобикалянето на капсулацията прави кода ти напълно непредсказуем 
// и чупи стълбовете на ООП дизайните ти. Използвай го само за тестови рамки.
// ============================================================

// ============================================================
// 9. ВРЪЗКА С ENTITY FRAMEWORK CORE
// ============================================================
// Когато в Entity Framework Core дефинираш твоя контекст:
// 
// public DbSet<Student> Students { get; set; }
// 
// Под капака EF Core стартира масивен рефлексивен процес:
//   1. Отваря класа Student чрез typeof(Student).
//   2. Обхожда всички Properties чрез .GetProperties().
//   3. Сканира за атрибути като [Key] или [MaxLength].
//   4. Генерира SQL миграционния скрипт автоматично.
// 
// Ето защо разбирането на Рефлексията ти дава ясна представа какво точно се случва 
// зад сцената на големите C# софтуерни рамки!
// ============================================================

// ============================================================
// 10. ФИНАЛНО АРХИТЕКТУРНО ПРАВИЛО
// ============================================================
// ПЪРВО МИСЛЯ: "Може ли този проблем да се реши със статичен C#, интерфейси 
// или класически полиморфизъм?"
// 
// АКО ДА -> Използвам класическия начин.
// АКО НЕ (Пиша DI Container, ORM, Универсален Mapper или Специфична Тестова Рамка) -> 
// чак тогава се обръщам към силата на Рефлексията и Атрибутите.
// 
// Винаги балансирай между производителност (Performance) и гъвкавост (Flexibility).
// ============================================================
// © 2026 Dev-Library / Архитектурно ядро
