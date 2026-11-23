[⬅️ 00-06-03. JavaScript Implementation](00-06-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [🏠 Към Главния Хъб на Библиотеката](../README.md)

---

# 00-06-04. Dynamic Programming: Java Имплементация & Cheat Sheet

**Език:** Java 17+ | **Цел:** Чисти имплементации на Top-Down Memoization с `HashMap`, Coin Change DP и 0/1 Knapsack в Java от нулата.

## 🧭 Съдържание на файла
* [1. Top-Down Memoization с `HashMap<Integer, Long>`](#1-top-down-memoization-с-hashmapinteger-long)
* [2. Coin Change (Bottom-Up Tabulation в Java)](#2-coin-change-bottom-up-tabulation-в-java)
* [3. 0/1 Knapsack (Целочислена раница в Java)](#3-01-knapsack-целочислена-раница-в-java)
* [4. Java Специфики & Cheat Sheet](#4-java-специфики--cheat-sheet)

---

## 1. TOP-DOWN MEMOIZATION С `HASHMAP<INTEGER, LONG>`

```java
import java.util.HashMap;
import java.util.Map;

public class MemoizationDemo {
    private static final Map<Integer, Long> memo = new HashMap<>();

    public static long climbStairs(int n) {
        if (n <= 2) return n;
        if (memo.containsKey(n)) return memo.get(n);

        long res = climbStairs(n - 1) + climbStairs(n - 2);
        memo.put(n, res);
        return res;
    }

    public static void main(String[] args) {
        System.out.println(climbStairs(50)); // 20365011074
    }
}
```

---

## 2. COIN CHANGE (BOTTOM-UP TABULATION В JAVA)

```java
import java.util.Arrays;

public class CoinChangeDp {
    public static int minCoins(int[] coins, int amount) {
        // Не ползваме Integer.MAX_VALUE, защото Integer.MAX_VALUE + 1 прелива в отрицателно число!
        int maxVal = amount + 1;
        int[] dp = new int[amount + 1];
        Arrays.fill(dp, maxVal);
        dp = 0;

        for (int sum = 1; sum <= amount; sum++) {
            for (int coin : coins) {
                if (sum - coin >= 0) {
                    dp[sum] = Math.min(dp[sum], dp[sum - coin] + 1);
                }
            }
        }

        return dp[amount] > amount ? -1 : dp[amount];
    }

    public static void main(String[] args) {
        System.out.println(minCoins(new int[]{1, 3, 4}, 6)); // 2
    }
}
```

---

## 3. 0/1 KNAPSACK (ЦЕЛОЧИСЛЕНА РАНИЦА В JAVA)

```java
public class Knapsack01 {
    public static int solve(int capacity, int[] weights, int[] values) {
        int[] dp = new int[capacity + 1];

        for (int i = 0; i < weights.length; i++) {
            int w = weights[i];
            int v = values[i];

            for (int cap = capacity; cap >= w; cap--) {
                dp[cap] = Math.max(dp[cap], dp[cap - w] + v);
            }
        }

        return dp[capacity];
    }

    public static void main(String[] args) {
        System.out.println(solve(7, new int[]{1, 3, 4, 5}, new int[]{1, 4, 5, 7})); // 9
    }
}
```

---

## 4. JAVA СПЕЦИФИКИ & CHEAT SHEET

| Операция | Синтаксис в Java | Бележка |
| :--- | :--- | :--- |
| **Запълване на DP масив** | `Arrays.fill(dp, amount + 1);` | Предпазва от `Integer.MAX_VALUE + 1` overflow |
| **Кеш с `computeIfAbsent`** | `memo.computeIfAbsent(k, key -> calc(key))` | Кратък функционален запис (но не за рекурсия!) |
| **2D таблица** | `int[][] dp = new int[rows][cols];` | Автоматично се инициализира с нули `0` |

---
[⬅️ 00-06-03. JavaScript Implementation](00-06-03-JavaScript-Implementation.md) | 📂 Папка: Algorithms-Introduction | [🏠 Начало](../README.md) | [🏠 Към Главния Хъб на Библиотеката](../README.md)
