[⬅️ 00-01-03. JavaScript Implementation](00-01-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-00-Searching-Algorithms](00-02-00-Searching-Algorithms.md)

---

# 00-01-04. Recursion & Backtracking: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на рекурсия, комбинации и Backtracking на лабиринт в Java от нулата.

## 🧭 Съдържание на файла
* [1. Базова рекурсия (Факториел и Сума на масив)](#1-базова-рекурсия-факториел-и-сума-на-масив)
* [2. Backtracking: Всички пътища в лабиринт (2D Матрица)](#2-backtracking-всички-пътища-в-лабиринт-2d-матрица)
* [3. Backtracking: Генериране на битови вектори (0 и 1)](#3-backtracking-генериране-на-битови-вектори-0-и-1)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. БАЗОВА РЕКУРСИЯ (ФАКТОРИЕЛ И СУМА НА МАСИВ)

```java
public class RecursionBasics {
    public static long factorial(int n) {
        if (n <= 1) return 1;
        return n * factorial(n - 1);
    }

    public static int arraySum(int[] arr, int index) {
        if (index == arr.length) return 0;
        return arr[index] + arraySum(arr, index + 1);
    }
}
```

---

## 2. BACKTRACKING: ВСИЧКИ ПЪТИЩА В ЛАБИРИНТ (2D МАТРИЦА)

```java
import java.util.ArrayList;
import java.util.List;

public class LabyrinthSolver {
    private static final List<Character> path = new ArrayList<>();
    private static final List<String> allPaths = new ArrayList<>();

    public static List<String> findPaths(char[][] lab) {
        allPaths.clear();
        backtrack(lab, 0, 0, '\0');
        return allPaths;
    }

    private static void backtrack(char[][] lab, int r, int c, char direction) {
        int rows = lab.length;
        int cols = lab[0].length;

        if (r < 0 || r >= rows || c < 0 || c >= cols || lab[r][c] == '*' || lab[r][c] == 'v') {
            return;
        }

        if (direction != '\0') path.add(direction);

        if (lab[r][c] == 'e') {
            StringBuilder sb = new StringBuilder();
            for (char ch : path) sb.append(ch);
            allPaths.add(sb.toString());
            if (!path.isEmpty()) path.remove(path.size() - 1);
            return;
        }

        // CHOOSE
        lab[r][c] = 'v';

        // EXPLORE
        backtrack(lab, r, c + 1, 'R');
        backtrack(lab, r + 1, c, 'D');
        backtrack(lab, r, c - 1, 'L');
        backtrack(lab, r - 1, c, 'U');

        // UNCHOOSE
        lab[r][c] = '-';
        if (!path.isEmpty()) path.remove(path.size() - 1);
    }
}
```

---

## 3. BACKTRACKING: ГЕНЕРИРАНЕ НА БИТОВИ ВЕКТОРИ (0 И 1)

```java
import java.util.Arrays;

public class BitVectors {
    public static void generate(int index, int[] vector) {
        if (index == vector.length) {
            System.out.println(Arrays.toString(vector));
            return;
        }

        for (int bit = 0; bit <= 1; bit++) {
            vector[index] = bit;
            generate(index + 1, vector);
        }
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Премахване на последния елемент** | `path.remove(path.size() - 1)` (или `path.removeLast()` в Java 21+) | Еквивалент на Python `pop()` |
| **Копиране на списък в дъното** | `result.add(new ArrayList<>(current))` | Пази моментна снимка в Heap паметта |
| **Грешка при дълбок стек** | `java.lang.StackOverflowError` | Наследява `Error` (не `Exception`) |

---
[⬅️ 00-01-03. JavaScript Implementation](00-01-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [➡️ 00-02-00-Searching-Algorithms](00-02-00-Searching-Algorithms.md)
