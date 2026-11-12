[⬅️ 00-04-03. JavaScript Implementation](00-04-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-00. Graphs and Traversal](00-05-00-Graphs-And-Traversal.md)

---

# 00-04-04. Greedy Algorithms: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на Coin Change, Activity Selection и Fractional Knapsack с `record` модели, `Comparator` и `PriorityQueue` от нулата.

## 🧭 Съдържание на файла
* [1. Coin Change (Връщане на ресто с минимален брой монети)](#1-coin-change-връщане-на-ресто-с-минимален-брой-монети)
* [2. Activity Selection (Избор на интервали със сортиране по краен час)](#2-activity-selection-избор-на-интервали-със-сортиране-по-краен-час)
* [3. Fractional Knapsack (Дробна раница с Java `record` и `Comparator`)](#3-fractional-knapsack-дробна-раница-с-java-record-и-comparator)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. COIN CHANGE (ВРЪЩАНЕ НА РЕСТО С МИНИМАЛЕН БРОЙ МОНЕТИ)

```java
import java.util.*;

public class GreedyCoinChange {
    public static Map<Integer, Integer> getChange(Integer[] coins, int targetSum) {
        // Сортираме монетите от най-голямата към най-малката
        Arrays.sort(coins, Collections.reverseOrder());
        Map<Integer, Integer> result = new LinkedHashMap<>();

        for (int coin : coins) {
            if (targetSum == 0) break;
            int count = targetSum / coin;
            if (count > 0) {
                result.put(coin, count);
                targetSum -= count * coin;
            }
        }

        if (targetSum > 0) {
            throw new IllegalArgumentException("Сумата не може да бъде съставена с наличните монети.");
        }

        return result;
    }

    public static void main(String[] args) {
        Integer[] coins = {1, 2, 5, 10, 20, 50};
        System.out.println(getChange(coins, 93)); // {50=1, 20=2, 2=1, 1=1}
    }
}
```

---

## 2. ACTIVITY SELECTION (ИЗБОР НА ИНТЕРВАЛИ СЪС СОРТИРАНЕ ПО КРАЕН ЧАС)

```java
import java.util.*;

record Meeting(int start, int end) {}

public class Scheduler {
    public static List<Meeting> selectMaxMeetings(List<Meeting> meetings) {
        List<Meeting> sorted = new ArrayList<>(meetings);
        sorted.sort(Comparator.comparingInt(Meeting::end));

        List<Meeting> selected = new ArrayList<>();
        int lastEnd = Integer.MIN_VALUE;

        for (Meeting m : sorted) {
            if (m.start() >= lastEnd) {
                selected.add(m);
                lastEnd = m.end();
            }
        }

        return selected;
    }
}
```

---

## 3. FRACTIONAL KNAPSACK (ДРОБНА РАНИЦА С JAVA `RECORD` И `COMPARATOR`)

```java
import java.util.*;

record Item(double value, double weight) {
    public double ratio() {
        return value / weight;
    }
}

public class KnapsackSolver {
    public static double getMaxValue(double capacity, List<Item> items) {
        List<Item> sorted = new ArrayList<>(items);
        sorted.sort(Comparator.comparingDouble(Item::ratio).reversed());

        double totalValue = 0.0;

        for (Item item : sorted) {
            if (capacity <= 0) break;

            if (item.weight() <= capacity) {
                totalValue += item.value();
                capacity -= item.weight();
            } else {
                totalValue += item.value() * (capacity / item.weight());
                capacity = 0;
            }
        }

        return totalValue;
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Сортиране по дробно поле низходящо** | `Comparator.comparingDouble(Item::ratio).reversed()` | Избягва грешки при изваждане на `double` |
| **Моделиране на обект (Java 16+)** | `record Meeting(int start, int end) {}` | Еквивалент на C# `record` |
| **Динамична алчна опашка (Min-Heap)** | `PriorityQueue<Integer> pq = new PriorityQueue<>();` | Връща най-малкия елемент за O(log n) |
| **Max-Heap в Java** | `new PriorityQueue<>(Collections.reverseOrder());` | Връща най-големия елемент за O(log n) |

---
[⬅️ 00-04-03. JavaScript Implementation](00-04-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-05-00. Graphs and Traversal](00-05-00-Graphs-And-Traversal.md)
