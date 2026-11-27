[⬅️ 02-07-00. Теория & Капани](02-07-00-Reflection-And-Annotations.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-08-00. Exceptions & Error Handling](02-08-00-Exceptions-Handling.md)

---

# 02-07-01. Reflection and Annotations: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за инспекция на класове, създаване на обекти по време на изпълнение, промяна на `private` полета, извикване на `private` методи и създаване/четене на собствени анотации в Java от нулата.

## 🧭 Съдържание на темата
* [1. Получаване на `Class<?>` обект и инспекция на метаданни](#1-получаване-на-class-обект-и-инспекция-на-метаданни)
* [2. Динамично създаване на инстанция и промяна на `private` поле](#2-динамично-създаване-на-инстанция-и-промяна-на-private-поле)
* [3. Динамично извикване на `private` метод (`Method.invoke`)](#3-динамично-извикване-на-private-метод-methodinvoke)
* [4. Създаване и четене на собствена Runtime анотация (`@interface`)](#4-създаване-и-четене-на-собствена-runtime-анотация-interface)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПОЛУЧАВАНЕ НА `CLASS<?>` ОБЕКТ И ИНСПЕКЦИЯ НА МЕТАДАННИ

Има **3 начина** да вземеш `Class<?>` обекта на един клас в Java:

```java
import java.lang.reflect.Modifier;

public class ClassInspectionDemo {
    public static void main(String[] args) throws ClassNotFoundException {
        // Начин 1: Директно от името на класа (Най-често)
        Class<String> c1 = String.class;

        // Начин 2: От съществуващ обект
        String text = "Hello";
        Class<?> c2 = text.getClass();

        // Начин 3: По пълно име на класа като String (използва се от JDBC / Spring)
        Class<?> c3 = Class.forName("java.util.ArrayList");

        System.out.println("Име: " + c3.getSimpleName());        // ArrayList
        System.out.println("Пакет: " + c3.getPackageName());     // java.util
        System.out.println("Public ли е: " + Modifier.isPublic(c3.getModifiers())); // true
    }
}
```

---

## 2. ДИНАМИЧНО СЪЗДАВАНЕ НА ИНСТАНЦИЯ И ПРОМЯНА НА `PRIVATE` ПОЛЕ

```java
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;

class SecretVault {
    private String pinCode = "0000";
    private SecretVault() {} // Частен конструктор!
    public String getPinCode() { return pinCode; }
}

public class ReflectionHackDemo {
    public static void main(String[] args) throws Exception {
        Class<SecretVault> clazz = SecretVault.class;

        // 1. Инстанциране на клас с PRIVATE конструктор!
        Constructor<SecretVault> ctor = clazz.getDeclaredConstructor();
        ctor.setAccessible(true);
        SecretVault vault = ctor.newInstance();

        // 2. Промяна на PRIVATE поле по време на изпълнение!
        Field pinField = clazz.getDeclaredField("pinCode");
        pinField.setAccessible(true);
        pinField.set(vault, "9876");

        System.out.println("Нов ПИН: " + vault.getPinCode()); // Изход: Нов ПИН: 9876
    }
}
```

---

## 3. ДИНАМИЧНО ИЗВИКВАНЕ НА `PRIVATE` МЕТОД (`METHOD.INVOKE`)

```java
import java.lang.reflect.Method;

class Calculator {
    private int secretMultiply(int a, int b) {
        return a * b;
    }
}

public class MethodInvokeDemo {
    public static void main(String[] args) throws Exception {
        Calculator calc = new Calculator();
        Class<?> clazz = calc.getClass();

        // Намираме метода по име и типове на параметрите
        Method method = clazz.getDeclaredMethod("secretMultiply", int.class, int.class);
        method.setAccessible(true);

        // Извикваме метода върху инстанцията 'calc' с аргументи (6, 7)
        int result = (int) method.invoke(calc, 6, 7);
        System.out.println("Резултат: " + result); // Изход: Резултат: 42
    }
}
```

---

## 4. СЪЗДАВАНЕ И ЧЕТЕНЕ НА СОБСТВЕНА RUNTIME АНОТАЦИЯ (`@INTERFACE`)

```java
import java.lang.annotation.*;
import java.lang.reflect.Method;

// 1. Дефиниция на анотацията
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.METHOD)
@interface RunAutomatically {
    int times() default 1;
}

// 2. Клас, който използва анотацията
class Worker {
    @RunAutomatically(times = 3)
    public void sayHello() {
        System.out.println("Здравей от анотиран метод!");
    }
}

// 3. Мини-фреймуърк, който сканира класа и изпълнява анотираните методи
public class AnnotationRunner {
    public static void main(String[] args) throws Exception {
        Worker worker = new Worker();
        for (Method m : worker.getClass().getDeclaredMethods()) {
            if (m.isAnnotationPresent(RunAutomatically.class)) {
                RunAutomatically info = m.getAnnotation(RunAutomatically.class);
                for (int i = 0; i < info.times(); i++) {
                    m.invoke(worker);
                }
            }
        }
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод в Reflection API | Какво връща | Забележка |
| :--- | :--- | :--- |
| **`clazz.getDeclaredFields()`** | `Field[]` (всички полета на текущия клас) | Включва `private` и `protected` |
| **`clazz.getFields()`** | `Field[]` (само `public` полета + наследените) | Не вижда `private` полета! |
| **`clazz.getDeclaredConstructor(args...)`** | `Constructor<T>` | Използвай `int.class`, а не `Integer.class` за примитиви |
| **`member.setAccessible(true)`** | `void` | Изключва проверката за достъп (`private` ➔ достъпен) |
| **`method.invoke(instance, args...)`** | `Object` | За `static` метод подай `null` като първи аргумент |
| **`clazz.isAnnotationPresent(Ann.class)`** | `boolean` | Анотацията трябва да има `@Retention(RUNTIME)` |
| **`Modifier.isStatic(f.getModifiers())`** | `boolean` | Проверява модификатори (`static`, `final`, `abstract`) |

---
[⬅️ 02-07-00. Теория & Капани](02-07-00-Reflection-And-Annotations.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-08-00. Exceptions & Error Handling](02-08-00-Exceptions-Handling.md)
