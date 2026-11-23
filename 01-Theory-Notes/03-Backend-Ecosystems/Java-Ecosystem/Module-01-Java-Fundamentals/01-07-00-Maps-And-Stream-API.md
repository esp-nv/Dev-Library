[⬅️ 01-06. Objects, Classes & Records](01-06-00-Objects-And-Classes.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-07-01. Syntax From Scratch](01-07-01-Syntax-From-Scratch.md)

---

# 01-07-00. Associative Arrays (`HashMap`, `LinkedHashMap`, `TreeMap` — Теория & Капани)

**Модул 01:** Java Fundamentals | **Лекционна тема 07:** Асоциативни масиви (Речници) в Java (`Map<K, V>`), вътрешно устройство на `HashMap` (Buckets, Hash Collisions и Red-Black Tree в Java 8+), разлика между `HashMap`, `LinkedHashMap` и `TreeMap`, и сравнение с C#, Python и JavaScript.

## 🧭 Съдържание на темата
* [1. Как да се запомни и чете по-лесно (Мнемоника)](#1-как-да-се-запомни-и-чете-по-лесно-мнемоника)
* [2. Архитектура на `HashMap` в паметта (Buckets & Treeify Блок-схема)](#2-архитектура-на-hashmap-в-паметта-buckets--treeify-блок-схема)
* [3. Сравнителна таблица: Java Maps срещу C#, Python и JavaScript](#3-сравнителна-таблица-java-maps-срещу-c-python-и-javascript)
* [4. Плюсове и минуси на трите вида `Map` в Java](#4-плюсове-и-минуси-на-трите-вида-map-в-java)
* [5. Кодов одит: Правилен срещу Грешен код (`NullPointerException` при авто-разопаковане и изменяеми ключове)](#5-кодов-одит-правилен-срещу-грешен-код-nullpointerexception-при-авто-разопаковане-и-изменяеми-ключове)
* [6. Интервю капани, въпроси и уловки](#6-интервю-капани-въпроси-и-уловки)

---

## 1. КАК ДА СЕ ЗАПОМНИ И ЧЕТЕ ПО-ЛЕСНО (МНЕМОНИКА)

*   **Трите шкафа за ключ-стойност в Java:**
    1.  **`HashMap<K, V>` (Разбърканият шкаф за светкавична скорост):** Не пази никакъв ред на ключовете, но намира, добавя и трие записи за **O(1)** време.
    2.  **`LinkedHashMap<K, V>` (Шкафът с хронология):** Пази **точния ред на добавяне** (Insertion Order) чрез скрит двойносвързан списък — 100% еквивалент на модерния `dict` в Python 3.7+ и `Map` в JavaScript!
    3.  **`TreeMap<K, V>` (Подреденият азбучен указател):** Държи ключовете **винаги автоматично сортирани** по възходящ ред (напр. от A до Z или от 1 до 100) чрез балансирано Червено-черно дърво (**Red-Black Tree**) за **O(log n)** време.

---

## 2. АРХИТЕКТУРА НА `HASHMAP` В ПАМЕТТА (BUCKETS & TREEIFY БЛОК-СХЕМА)

Когато извикаш `map.put("Ivan", 25)`, Java прекарва ключа през следната верига (начален капацитет: **16 кофи**, Load Factor: **0.75**):

```text
Ключ: "Ivan"
     │
     ▼  1. Изчислява хеш кода
"Ivan".hashCode() ➔ 2290583
     │
     ▼  2. Намира индекса на кофата (Bucket Index = hash & (n - 1))
Кофа [7] в масива Node<K, V>[] table
     │
     ├── Ако кофата е празна ➔ Записва новия възел директно (O(1))
     └── Ако има сблъсък (Hash Collision):
           ├── До 7 елемента в кофата ➔ Свързан списък (Linked List: O(n))
           └── Над 8 елемента в кофата (и таблица >= 64) ➔ Превръща се в
               Червено-черно дърво (Red-Black Tree: O(log n))!
```

---

## 3. СРАВНИТЕЛНА ТАБЛИЦА: JAVA MAPS СРЕЩУ C#, PYTHON И JAVASCRIPT

| Операция / Структура | ☕ Java 21+ (`Map<K, V>`) | 🟣 C# 12+ (`Dictionary<K, V>`) | 🐍 Python 3.x (`dict`) | 🟡 JavaScript (`Map`) |
| :--- | :--- | :--- | :--- | :--- |
| **Стандартен речник (O(1))** | `new HashMap<>()` (без ред) | `new Dictionary<K, V>()` | `{}` (пази реда!) | `new Map()` (пази реда!) |
| **Речник с ред на вмъкване** | `new LinkedHashMap<>()` | Няма вграден (OrderedDictionary) | `{}` (по подразбиране) | `new Map()` (по подразбиране) |
| **Автоматично сортиран речник** | `new TreeMap<>()` | `new SortedDictionary<K, V>()` | `SortedDict` (външен пакет) | Няма вграден |
| **Добавяне / Презаписване** | `map.put("A", 1);` | `dict["A"] = 1;` | `d["A"] = 1` | `map.set("A", 1);` |
| **Добавяне само ако липсва** | `map.putIfAbsent("A", 1);` | `dict.TryAdd("A", 1);` | `d.setdefault("A", 1)` | `if (!map.has("A")) map.set(...)` |
| **Четене на стойност** | `map.get("A")` (`null` ако липсва) | `dict["A"]` (гърми с KeyNotFound!) | `d.get("A")` (`None` ако липсва) | `map.get("A")` (`undefined`) |
| **Безопасно четене с Default** | `map.getOrDefault("A", 0)` | `dict.GetValueOrDefault("A", 0)` | `d.get("A", 0)` | `map.get("A") ?? 0` |
| **Проверка за ключ / стойност** | `containsKey(k)` / `containsValue(v)` | `ContainsKey(k)` / `ContainsValue(v)` | `k in d` / `v in d.values()` | `map.has(k)` |

---

## 4. ПЛЮСОВЕ И МИНУСИ НА ТРИТЕ ВИДА `MAP` В JAVA

### 🟢 `HashMap` и `LinkedHashMap` — Предимства и Недостатъци:
*   **Предимства:** Константно време за търсене, добавяне и триене ($O(1)$); позволяват **един `null` ключ** и неограничен брой `null` стойности.
*   **Недостатъци:** Не са безопасни при многонишкова работа (за паралелни нишки се ползва **`ConcurrentHashMap`**).

### 🔵 `TreeMap` — Предимства и Недостатъци:
*   **Предимства:** Ключовете са винаги сортирани; предлага мощни методи за диапазони (`firstKey()`, `lastKey()`, `subMap(from, to)`).
*   **Недостатъци:** По-бавен от `HashMap` ($O(\log n)$ вместо $O(1)$) и **не позволява `null` ключове** (хвърля `NullPointerException`, защото трябва да сравнява ключовете с `compareTo()`).

---

## 5. КОДОВ ОДИТ: ПРАВИЛЕН СРЕЩУ ГРЕШЕН КОД (`NULLPOINTEREXCEPTION` ПРИ АВТО-РАЗОПАКОВАНЕ И ИЗМЕНЯЕМИ КЛЮЧОВЕ)

### ❌ ГРЕШЕН КОД (Скрит `NullPointerException` при `map.get()` и мутиране на ключ):
```java
Map<String, Integer> scores = new HashMap<>();

// 🚨 ГРЕШКА 1: map.get("Ivan") връща null (обект Integer), защото ключът още не съществува!
// При опита да го запишеш в примитивен 'int', Java прави Unboxing върху null -> CRASH!
int currentScore = scores.get("Ivan"); // NullPointerException!

// 🚨 ГРЕШКА 2: Използване на изменяем (Mutable) обект (напр. List) за ключ в HashMap!
List<String> keyList = new ArrayList<>(List.of("A"));
Map<List<String>, Integer> badMap = new HashMap<>();
badMap.put(keyList, 100);
keyList.add("B"); // Променяме списъка -> hashCode() се променя -> ключът е изгубен завинаги!
System.out.println(badMap.get(keyList)); // null!
```
*   **Как да се избегне:** За броячи и суми **винаги** използвай **`map.getOrDefault(key, 0)`** или **`map.merge(key, 1, Integer::sum)`**. За ключове в `Map` използвай само **неизменяеми (Immutable)** типове (`String`, `Integer`, `UUID`, `record`).

### 🟢 ПРАВИЛЕН КОД (Модерно броене и групиране на 1 ред с `merge()` и `computeIfAbsent()`):
```java
import java.util.*;

public class CleanMapsDemo {
    public static void main(String[] args) {
        Map<String, Integer> wordCounts = new LinkedHashMap<>();
        String[] words = {"java", "spring", "java", "boot", "java"};

        // Най-чистият начин за броене в Java 8+ (без if-else проверки!)
        for (String word : words) {
            wordCounts.merge(word, 1, Integer::sum);
        }
        System.out.println(wordCounts); // {java=3, spring=1, boot=1}

        // Най-чистият начин за групиране в списък (Map<String, List<String>>)
        Map<String, List<String>> departments = new HashMap<>();
        departments.computeIfAbsent("IT", k -> new ArrayList<>()).add("Елизабет");
        departments.computeIfAbsent("IT", k -> new ArrayList<>()).add("Иван");
    }
}
```

---

## 6. ИНТЕРВЮ КАПАНИ, ВЪПРОСИ И УЛОВКИ

*   **Въпрос на интервюто:** *„Какво представлява **Load Factor** (коефициент на запълване) в Java `HashMap`, каква е стойността му по подразбиране и какво се случва, когато бъде надвишен?“*
*   **Уловката:** Кандидатите знаят, че `HashMap` се разширява, но не знаят точния механизъм и цената на операцията **Rehashing**.
*   **Точен Senior отговор:** По подразбиране `HashMap` стартира с **16 кофи (Buckets)** и **`Load Factor = 0.75`**. Когато броят на елементите достигне `16 × 0.75 = 12`, картата преценява, че рискът от сблъсъци (Hash Collisions) става твърде висок, и извършва операция **Rehashing**:
    1.  Заделя нов масив с **двоен размер (`16 -> 32 -> 64...`)**.
    2.  Преизчислява позицията на **всеки един съществуващ ключ** спрямо новия размер на таблицата ($O(n)$ операция).
    *   *Pro Tip:* Ако предварително знаеш, че ще сложиш 10 000 записа в `HashMap`, подай начален капацитет в конструктора (`new HashMap<>(16384)`), за да избегнеш всички междинни преоразмерявания!

---
[⬅️ 01-06. Objects, Classes & Records](01-06-00-Objects-And-Classes.md) | 📂 Папка: Module-01-Java-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 01-07-01. Syntax From Scratch](01-07-01-Syntax-From-Scratch.md)
