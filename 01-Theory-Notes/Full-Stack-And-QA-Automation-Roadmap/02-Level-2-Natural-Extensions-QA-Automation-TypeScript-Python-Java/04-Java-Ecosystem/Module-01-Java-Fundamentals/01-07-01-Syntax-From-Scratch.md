[⬅️ 01-07-00. Теория & Капани](01-07-00-Maps-And-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-08-00. Stream API & Lambdas](01-08-00-Stream-API.md)

---

# 01-07-01. Associative Arrays (`Map`): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване, обхождане (`entrySet`, `keySet`, `forEach`), вложени речници и сортиране на `Map<K, V>` по ключ и по стойност в Java от нулата.

## 🧭 Съдържание на темата
* [1. Основни операции с `HashMap`, `LinkedHashMap` и `TreeMap`](#1-основни-операции-с-hashmap-linkedhashmap-и-treemap)
* [2. Четирите начина за обхождане на `Map<K, V>`](#2-четирите-начина-за-обхождане-на-mapk-v)
* [3. Вложени речници (`Map<String, List<Integer>>` и `Map<String, Map<String, Integer>>`)](#3-вложени-речници-mapstring-listinteger-и-mapstring-mapstring-integer)
* [4. Сортиране на `Map<K, V>` по стойност (Value) и по ключ (Key)](#4-сортиране-на-mapk-v-по-стойност-value-и-по-ключ-key)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ОСНОВНИ ОПЕРАЦИИ С `HASHMAP`, `LINKEDHASHMAP` И `TREEMAP`

```java
import java.util.*;

public class MapBasics {
    public static void main(String[] args) {
        // 1. Създаване (HashMap = без ред, LinkedHashMap = ред на добавяне, TreeMap = сортиран)
        Map<String, Double> products = new LinkedHashMap<>();

        // 2. Добавяне и обновяване
        products.put("Laptop", 2200.00);
        products.put("Mouse", 45.50);
        products.putIfAbsent("Mouse", 99.00); // Няма да презапише 45.50, защото "Mouse" вече съществува!

        // 3. Четене и проверки
        double price = products.get("Laptop");                  // 2200.0
        double safePrice = products.getOrDefault("Keyboard", 0.0); // 0.0 (не гърми!)
        boolean hasKey = products.containsKey("Mouse");         // true
        boolean hasVal = products.containsValue(45.50);         // true

        // 4. Изтриване
        products.remove("Mouse");
    }
}
```

---

## 2. ЧЕТИРИТЕ НАЧИНА ЗА ОБХОЖДАНЕ НА `MAP<K, V>`

```java
Map<String, Integer> ages = Map.of("Анна", 25, "Борис", 30, "Виктор", 28);

// 1. Най-ефективен начин (Чете едновременно ключ и стойност чрез entrySet):
for (Map.Entry<String, Integer> entry : ages.entrySet()) {
    System.out.printf("%s -> %d год.%n", entry.getKey(), entry.getValue());
}

// 2. Модерен Lambda синтаксис (Java 8+):
ages.forEach((name, age) -> System.out.println(name + ": " + age));

// 3. Само по ключовете (keySet):
for (String name : ages.keySet()) {
    System.out.println("Име: " + name);
}

// 4. Само по стойностите (values):
for (int age : ages.values()) {
    System.out.println("Възраст: " + age);
}
```

---

## 3. ВЛОЖЕНИ РЕЧНИЦИ (`MAP<STRING, LIST<INTEGER>>` И `MAP<STRING, MAP<STRING, INTEGER>>`)

```java
Map<String, List<Double>> studentGrades = new LinkedHashMap<>();

// Добавяне на оценка към студент (създава списъка автоматично, ако студентът е нов!)
studentGrades.putIfAbsent("Елизабет", new ArrayList<>());
studentGrades.get("Елизабет").add(6.00);

// По-кратък запис с computeIfAbsent:
studentGrades.computeIfAbsent("Елизабет", k -> new ArrayList<>()).add(5.75);
```

---

## 4. СОРТИРАНЕ НА `MAP<K, V>` ПО СТОЙНОСТ (VALUE) И ПО КЛЮЧ (KEY)

```java
Map<String, Integer> scores = Map.of("Ivan", 80, "Maria", 95, "Georgi", 95);

// Сортиране по точки НИЗХОДЯЩО, а при равни точки по име ВЪЗХОДЯЩО:
scores.entrySet().stream()
        .sorted(
            Map.Entry.<String, Integer>comparingByValue().reversed()
                     .thenComparing(Map.Entry.comparingByKey())
        )
        .forEach(e -> System.out.println(e.getKey() + " -> " + e.getValue()));
// Изход:
// Georgi -> 95
// Maria -> 95
// Ivan -> 80
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Метод на `Map<K, V>` | Какво прави | Бележка |
| :--- | :--- | :--- |
| **`map.put(k, v)`** | Добавя или презаписва стойността за ключ `k` | Връща старата стойност или `null` |
| **`map.putIfAbsent(k, v)`** | Записва стойността **само ако ключът липсва** | Идеално за инициализация |
| **`map.getOrDefault(k, def)`** | Връща стойността или `def`, ако ключът липсва | Предпазва от `NullPointerException` |
| **`map.merge(k, 1, Integer::sum)`** | Увеличава брояч с 1 (или го създава със стойност 1) | Най-краткият брояч в Java |
| **`map.computeIfAbsent(k, fn)`** | Изчислява и записва нова стойност, ако ключът липсва | Идеално за `Map<K, List<V>>` |
| **`map.entrySet()`** | Връща множество от двойки `Map.Entry<K, V>` | Най-бързият начин за обхождане |
| **`Map.of("k1", v1, "k2", v2)`** | Създава бърз непроменяем речник (до 10 двойки) | Не позволява `null` и `.put()` |

---
[⬅️ 01-07-00. Теория & Капани](01-07-00-Maps-And-Stream-API.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-08-00. Stream API & Lambdas](01-08-00-Stream-API.md)
