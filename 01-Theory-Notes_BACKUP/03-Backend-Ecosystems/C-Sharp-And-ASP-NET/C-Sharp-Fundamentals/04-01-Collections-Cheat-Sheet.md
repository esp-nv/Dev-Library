# 📋 Cheat Sheet: Колекции и Структури от данни в C#

**Файл:** `04-01-Collections-Cheat-Sheet.md` | **Цел:** Бърз избор на структура, методи, Big-O скорост и C# 12 синтаксис

---

## ⚡ 1. БЪРЗА СРАВНИТЕЛНА МАТРИЦА (КАКВО ДА ИЗБЕРА НА ИЗПИТА?)

| Колекция | Подредба | Уникални елементи? | Достъп по индекс `[i]`? | Търсене (Contains / Key) | Кога да я използваш? |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`T[]` (Array)** | По ред на добавяне | Не | **Да** `O(1)` | Бавно `O(n)` | Когато размерът е **фиксиран** и не се променя. |
| **`List<T>`** | По ред на добавяне | Не | **Да** `O(1)` | Бавно `O(n)` | Основна динамична колекция (добавяне/триене). |
| **`Dictionary<K,V>`** | По ред на добавяне | **Да** (само ключовете) | По ключ `dict[k]` | **Мигновено `O(1)`** | Когато търсиш стойност по уникален ключ (ID, Име). |
| **`SortedDictionary<K,V>`**| **Сортирана по ключ** | **Да** (само ключовете) | По ключ `dict[k]` | Бързо `O(log n)` | Когато данните трябва винаги да са сортирани по ключ. |
| **`HashSet<T>`** | Няма гарантиран ред | **Да** (всички) | **Не** | **Мигновено `O(1)`** | За премахване на дубликати и светкавична проверка дали елемент съществува. |
| **`SortedSet<T>`** | **Сортирана** | **Да** (всички) | **Не** | Бързо `O(log n)` | Когато ти трябват уникални И автоматично сортирани елементи. |
| **`Stack<T>`** | **LIFO** (Последният влиза, първи излиза) | Не | **Не** | Бавно `O(n)` | Обратен ред, история (Undo), проверка на скоби. |
| **`Queue<T>`** | **FIFO** (Първият влиза, първи излиза) | Не | **Не** | Бавно `O(n)` | Опашки за обработка, задачи по ред на пристигане. |

---

## 🛠️ 2. ТАБЛИЦА НА КЛЮЧОВИТЕ МЕТОДИ ПО КОЛЕКЦИИ

### 🔹 `List<T>` (Динамичен списък)
* **Добавяне:** `list.Add(item)`, `list.AddRange(collection)`, `list.Insert(index, item)`
* **Премахване:** `list.Remove(item)`, `list.RemoveAt(index)`, `list.RemoveAll(x => x > 5)`, `list.Clear()`
* **Търсене и подредба:** `list.Contains(item)`, `list.IndexOf(item)`, `list.Sort()`, `list.Reverse()`

### 🔹 `Dictionary<TKey, TValue>` (Речник)
* **Добавяне / Обновяване:**
  * `dict.Add(key, val)` *(хвърля грешка, ако ключът вече съществува!)*
  * `dict[key] = val` *(добавя нов или презаписва стария без грешка)*
  * `dict.TryAdd(key, val)` *(връща `false`, ако ключът съществува)*
* **Безопасно четене:**
  * `if (dict.TryGetValue(key, out var val))` *(най-бързият начин — търси само 1 път в паметта!)*
  * `dict.ContainsKey(key)` (`O(1)`), `dict.ContainsValue(val)` (`O(n)`)
* **Обхождане:** `foreach (var (key, value) in dict)` или през `dict.Keys` / `dict.Values`.

### 🔹 `HashSet<T>` (Множество от уникални елементи)
* **Основни:** `set.Add(item)` *(връща `false` при дубликат)*, `set.Remove(item)`, `set.Contains(item)`
* **Операции с множества:**
  * `set1.UnionWith(set2)` *(Обединение — добавя всички от `set2`)*
  * `set1.IntersectWith(set2)` *(Сечение — оставя само общите)*
  * `set1.ExceptWith(set2)` *(Разлика — маха срещаните в `set2`)*

### 🔹 `Stack<T>` (LIFO) и `Queue<T>` (FIFO)
* **`Stack<T>`:** `stack.Push(item)` (слага отгоре), `stack.Pop()` (вади и трие отгоре), `stack.Peek()` (само поглежда отгоре).
* **`Queue<T>`:** `queue.Enqueue(item)` (слага отзад), `queue.Dequeue()` (вади и трие отпред), `queue.Peek()` (само поглежда отпред).

---

## 🧠 3. ЗЛАТНОТО ПРАВИЛО: STATIC СРЕЩУ INSTANCE МЕТОДИ

> **Запомни:** **Единствено `Array` (масивът)** използва статични методи върху името на класа. Всички модерни Generic колекции (`List<T>`, `Dictionary`, `HashSet`) използват само инстанционни методи върху самата променлива!

```csharp
// ✅ ARRAY (Статични методи върху класа Array + свойство .Length)
int[] arr = { 3, 1, 2 };
Array.Sort(arr);
Array.Reverse(arr);
int len = arr.Length;

// ✅ LIST<T> (Инстанционни методи върху обекта + свойство .Count)
List<int> list = new List<int> { 3, 1, 2 };
list.Sort();
list.Reverse();
int count = list.Count;
```

---

## 🆕 4. МОДЕРЕН СИНТАКСИС (C# 8+ ИНДЕКСИ И C# 12 COLLECTION EXPRESSIONS)

В по-новите версии на C# (.NET 8+) можеш да срещнеш този съкратен запис:

```csharp
// 1. Индекси отзад напред (^ - Hat operator) и отрязъци (.. - Range operator)
int[] nums =; // Нов начин за инициализация (C# 12)
int last = nums[^1];               // 50 (последният елемент)
int[] middle = nums[1..4];         // [20, 30, 40] (от индекс 1 до 3 вкл.)

// 2. Обединяване на колекции със Spread оператора (..)
int[] part1 =;
int[] part2 =;
int[] combined = [..part1, ..part2, 5]; // [1, 2, 3, 4, 5]

// 3. Празен списък или речник
List<string> emptyList = [];
Dictionary<string, int> emptyDict = [];
```

---

## 📚 ПОЛЕЗНИ ВЪНШНИ СПРАВОЧНИЦИ

* **[Cheatography: C# Collection Cheat Sheet](https://cheatography.com/masterofcode/cheat-sheets/c-collection/)** — Компактен преглед на основните класове и методи в `System.Collections.Generic`.
* **[DEV Community: C# Collections Quick Reference](https://dev.to/recurpixel/c-collections-quick-reference-k1h)** — Подробно сравнение на 13 колекции и правилото за *Static vs Instance* членове.
* **[LabinatorSolutions: C# Ultimate Cheatsheet](https://github.com/LabinatorSolutions/csharp-cheatsheet/blob/main/docs/ultimate-cheatsheet.md)** — Пълен справочник за целия език C# (от версия 1 до 14).
* **[Zenn: C# Collection Expressions Cheatsheet](https://zenn.dev/inuinu/articles/84c6d5ca85c41f?locale=en)** — Примери за новия съкратен синтаксис с `[..]` в C# 12+.

---
© 2026 Dev-Library / Бърз изпитен Cheat Sheet по Колекции
