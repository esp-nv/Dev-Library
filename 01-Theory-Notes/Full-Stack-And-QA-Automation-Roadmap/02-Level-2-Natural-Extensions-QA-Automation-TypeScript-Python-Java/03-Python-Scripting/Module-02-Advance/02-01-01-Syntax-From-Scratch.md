[⬅️ 02-01-00. Теория & Капани](02-01-00-Lists-As-Stacks-And-Queues.md) | [📂 Папка: Module-02-Advance](README.md) | [🏠 Начало](../../../../README.md) | [➡️ 02-02-00. Tuples and Sets](02-02-00-Tuples-And-Sets.md)

---

# 02-01-01. Stacks and Queues: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за работа със Стекове (`list`) и Опашки (`deque`) в Python от нулата.

## 🧭 Съдържание на темата
* [1. Синтаксис на Стек (Stack - LIFO)](#1-синтаксис-на-стек-stack---lifo)
* [2. Синтаксис на Опашка (Queue - FIFO)](#2-синтаксис-на-опашка-queue---fifo)
* [3. Специални методи на `deque` (Rotate и Maxlen)](#3-специални-методи-на-deque-rotate-и-maxlen)
* [4. Вложени операции (Обръщане на текст със Стек)](#4-вложени-операции-обръщане-на-текст-със-стек)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СИНТАКСИС НА СТЕК (STACK - LIFO)

За стек използваме вградения списък (`list`), като работим **само от дясната страна**.

```python
# 1. Създаване на празен стек
stack = []

# 2. Добавяне на елемент най-отгоре (Push)
stack.append("Страница 1")
stack.append("Страница 2")
stack.append("Страница 3")

# 3. Поглеждане на най-горния елемент без да го трием (Peek)
top_element = stack[-1]
print(f"В момента гледаме: {top_element}")     # Изход: Страница 3

# 4. Изваждане и премахване на най-горния елемент (Pop)
removed = stack.pop()
print(f"Върнахме се назад от: {removed}")     # Изход: Страница 3

# 5. Изпразване на целия стек докато не стане празен
while stack:
    print(f"Затваряне на: {stack.pop()}")
```

---

## 2. СИНТАКСИС НА ОПАШКА (QUEUE - FIFO)

За опашка **винаги** импортираме `deque` от вградения модул `collections`.

```python
from collections import deque

# 1. Създаване на празна или начална опашка
queue = deque()
clients = deque(["Иван", "Мария"])

# 2. Добавяне в КРАЯ на опашката (Enqueue)
clients.append("Георги")                # Ред: ['Иван', 'Мария', 'Георги']

# 3. Добавяне с предимство в НАЧАЛОТО на опашката
clients.appendleft("VIP_Елизабет")      # Ред: ['VIP_Елизабет', 'Иван', 'Мария', 'Георги']

# 4. Поглеждане на първия на опашката без да го махаме
first_in_line = clients
print(f"Следващ на касата: {first_in_line}")

# 5. Обслужване и премахване на ПЪРВИЯ от опашката (Dequeue)
served_client = clients.popleft()
print(f"Обслужен: {served_client}")     # Изход: VIP_Елизабет
```

---

## 3. СПЕЦИАЛНИ МЕТОДИ НА `DEQUE` (ROTATE И MAXLEN)

```python
from collections import deque

# 1. Завъртане на опашката (Rotate) - много полезно за игри като "Горещ картоф"
ring = deque()
ring.rotate(1)      # Мести последния елемент най-отпред -> deque()
ring.rotate(-1)     # Мести първия елемент най-отзад -> deque()

# 2. Опашка с фиксиран максимален размер (maxlen)
recent_logs = deque(maxlen=3)
recent_logs.append("Грешка 1")
recent_logs.append("Грешка 2")
recent_logs.append("Грешка 3")
recent_logs.append("Грешка 4")  # "Грешка 1" автоматично се изтрива!
print(list(recent_logs))        # Изход: ['Грешка 2', 'Грешка 3', 'Грешка 4']
```

---

## 4. ВЛОЖЕНИ ОПЕРАЦИИ (ОБРЪЩАНЕ НА ТЕКСТ СЪС СТЕК)

```python
word = "Python"
char_stack = []

# Ниво 1: Пълним стека буква по буква
for ch in word:
    char_stack.append(ch)

reversed_word = ""
# Ниво 2: Вадим буквите отгоре надолу (обръщат се автоматично)
while char_stack:
    reversed_word += char_stack.pop()

print(f"Оригинал: {word} | Обърнато: {reversed_word}")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Структура | Действие | Синтаксис | Скорост |
| :--- | :--- | :--- | :--- |
| **Стек (`list`)** | Добавяне най-отгоре (Push) | `stack.append(x)` | O(1) |
| **Стек (`list`)** | Вадене най-отгоре (Pop) | `stack.pop()` | O(1) |
| **Стек (`list`)** | Поглеждане отгоре (Peek) | `stack[-1]` | O(1) |
| **Опашка (`deque`)** | Импортиране | `from collections import deque` | — |
| **Опашка (`deque`)** | Добавяне в края (Enqueue) | `q.append(x)` | O(1) |
| **Опашка (`deque`)** | Вадене от началото (Dequeue) | `q.popleft()` | O(1) |
| **Опашка (`deque`)** | Добавяне най-отпред | `q.appendleft(x)` | O(1) |
| **Опашка (`deque`)** | Завъртане на елементите | `q.rotate(n)` | O(k) |

---
[⬅️ 02-01-00. Теория & Капани](02-01-00-Lists-As-Stacks-And-Queues.md) | [📂 Папка: Module-02-Advance](README.md) | [🏠 Начало](../../../../README.md) | [➡️ 02-02-00. Tuples and Sets](02-02-00-Tuples-And-Sets.md)
