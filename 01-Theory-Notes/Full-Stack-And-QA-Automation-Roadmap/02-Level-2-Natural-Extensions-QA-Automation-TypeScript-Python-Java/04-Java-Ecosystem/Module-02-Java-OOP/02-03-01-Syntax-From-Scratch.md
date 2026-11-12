[⬅️ 02-03-00. Теория & Капани](02-03-00-Inheritance.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-04-00. Interfaces & Abstract Classes](02-04-00-Interfaces-And-Abstract-Classes.md)

---

# 02-03-01. Inheritance (`extends`, `super` & `final`): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за наследяване на класове, верижно извикване с `super(...)`, пренаписване с `@Override` и използване на `final` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Базов клас и наследник (`extends` и `super(...)`)](#1-базов-клас-и-наследник-extends-и-super)
* [2. Многостепенно наследяване (Animal ➔ Mammal ➔ Dog)](#2-многостепенно-наследяване-animal--mammal--dog)
* [3. Използване на `final` класове и `final` методи](#3-използване-на-final-класове-и-final-методи)
* [4. Правила за пренаписване на методи (`@Override` и Covariant Return Types)](#4-правила-за-пренаписване-на-методи-override-и-covariant-return-types)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. БАЗОВ КЛАС И НАСЛЕДНИК (`EXTENDS` И `SUPER(...)`)

```java
// Базов клас (Superclass / Parent)
public class Employee {
    private String name;
    private double baseSalary;

    public Employee(String name, double baseSalary) {
        this.name = name;
        this.baseSalary = baseSalary;
    }

    public String getName() { return name; }
    public double calculateSalary() { return baseSalary; }
}

// Наследник (Subclass / Child)
public class Manager extends Employee {
    private double bonus;

    public Manager(String name, double baseSalary, double bonus) {
        super(name, baseSalary); // Предава данните към конструктора на Employee
        this.bonus = bonus;
    }

    @Override
    public double calculateSalary() {
        return super.calculateSalary() + this.bonus; // Извиква базовия метод + добавя бонуса
    }
}
```

---

## 2. МНОГОСТЕПЕННО НАСЛЕДЯВАНЕ (ANIMAL ➔ MAMMAL ➔ DOG)

```java
class Animal {
    public void eat() { System.out.println("eating..."); }
}

class Dog extends Animal {
    public void bark() { System.out.println("barking..."); }
}

class Puppy extends Dog {
    public void weep() { System.out.println("weeping..."); }
}

// Puppy разполага с eat(), bark() и weep()!
```

---

## 3. ИЗПОЛЗВАНЕ НА `FINAL` КЛАСОВЕ И `FINAL` МЕТОДИ

```java
// 1. Забрана за наследяване на целия клас (напр. за помощни класове или сигурност)
public final class SecurityTokenGenerator {
    public static String generate() {
        return java.util.UUID.randomUUID().toString();
    }
}

// 2. Забрана за пренаписване на критичен метод в наследниците
class BasePayment {
    public final void validateTransaction() {
        System.out.println("Криптирана проверка, която никой наследник не може да промени!");
    }
}
```

---

## 4. ПРАВИЛА ЗА ПРЕНАПИСВАНЕ НА МЕТОДИ (`@OVERRIDE` И COVARIANT RETURN TYPES)

При `@Override` трябва да спазваш 3 правила:
1.  **Еднакво име и параметри** с метода в базовия клас.
2.  **Същото или по-широко ниво на достъп** (можеш да промениш `protected` на `public`, но **не можеш** да стесниш `public` до `private`!).
3.  **Covariant Return Type:** Връщаният тип може да е същият или по-конкретен подтип (напр. родителят връща `Animal`, а наследникът при `@Override` може да връща `Dog`).

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция | Синтаксис в Java | Какво прави |
| :--- | :--- | :--- |
| **Наследяване на клас** | `class Child extends Parent` | Наследява всички не-`private` членове |
| **Родителски конструктор** | `super(arg1, arg2);` | Задължително е първият ред в конструктора |
| **Родителски метод / поле** | `super.methodName()` | Достъпва оригиналната версия от базовия клас |
| **Анотация за пренаписване** | `@Override` | Накара компилатора да провери сигнатурата |
| **Забрана за наследяване** | `final class A` | Спира `extends A` |
| **Забрана за пренаписване** | `final void run()` | Спира `@Override` на метода в наследниците |

---
[⬅️ 02-03-00. Теория & Капани](02-03-00-Inheritance.md) | 📂 Папка: Module-02-Java-OOP | [🏠 Начало](../../README.md) | [➡️ 02-04-00. Interfaces & Abstract Classes](02-04-00-Interfaces-And-Abstract-Classes.md)
