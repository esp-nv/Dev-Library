[⬅️ 01-08-00. Теория & Капани](01-08-00-Text-Processing.md) | 📂 Папка: Module-01-Basic-and-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 01-09-00. Regular Expressions](01-09-00-Regular-Expressions.md)

---

# 01-08-01. Text Processing: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за форматиране, почистване, търсене, замяна и проверка на текстови низове в Python от нулата.

## 🧭 Съдържание на темата
* [1. Индексиране, Slicing и базови операции](#1-индексиране-slicing-и-базови-операции)
* [2. Методи за промяна на регистъра и почистване](#2-методи-за-промяна-на-регистъра-и-почистване)
* [3. Търсене, броене и замяна (Find, Count, Replace)](#3-търсене-броене-и-замяна-find-count-replace)
* [4. Валидация на символи и ASCII таблица](#4-валидация-на-символи-и-ascii-таблица)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ИНДЕКСИРАНЕ, SLICING И БАЗОВИ ОПЕРАЦИИ

```python
text = "Python Programming"

# 1. Достъп по индекс и Slicing (работи точно като при списъците!)
print(text[0])          # Първа буква -> 'P'
print(text[-1])         # Последна буква -> 'g'
print(text[0:6])        # От индекс 0 до 5 -> 'Python'
print(text[::-1])       # Обръща целия текст наопаки -> 'gnimmargorP nohtyP'

# 2. Съединяване (+) и повторение (*)
greeting = "Здравей" + " " + "свят!"
stars = "*" * 10        # Резултат: '**********'
```

---

## 2. МЕТОДИ ЗА ПРОМЯНА НА РЕГИСТЪРА И ПОЧИСТВАНЕ

*Важно: Всички тези методи връщат **нов** текст, без да променят оригинала!*

```python
raw = "   hElLo pYtHoN   "

print(raw.strip())          # Маха интервалите отляво и отдясно -> 'hElLo pYtHoN'
print(raw.lstrip())         # Маха интервалите само отляво
print(raw.rstrip())         # Маха интервалите само отдясно

clean = raw.strip()
print(clean.lower())        # Всичко с малки букви -> 'hello python'
print(clean.upper())        # Всичко с ГЛАВНИ букви -> 'HELLO PYTHON'
print(clean.capitalize())   # Само първата буква главна -> 'Hello python'
print(clean.title())        # Всяка дума с главна буква -> 'Hello Python'
```

---

## 3. ТЪРСЕНЕ, БРОЕНЕ И ЗАМЯНА (FIND, COUNT, REPLACE)

```python
sentence = "I love C#, and C# is great!"

# 1. Проверка дали подниз се съдържа в текста (in / not in)
if "C#" in sentence:
    print("Намерено е C#!")

# 2. Намиране на позиция (индекс)
idx = sentence.find("C#")       # Връща индекса на първото срещане -> 7 (ако го няма, връща -1)

# 3. Броене и замяна
occurrences = sentence.count("C#")                  # Брои колко пъти се среща -> 2
updated = sentence.replace("C#", "Python")          # Заменя ВСИЧКИ срещания -> 'I love Python, and Python is great!'
only_first = sentence.replace("C#", "Python", 1)    # Заменя САМО първото срещане!
```

---

## 4. ВАЛИДАЦИЯ НА СИМВОЛИ И ASCII ТАБЛИЦА

```python
# 1. Булеви проверки на съдържанието (връщат True или False)
print("12345".isdigit())        # Само цифри ли е? -> True
print("Python".isalpha())       # Само букви ли е? -> True
print("User123".isalnum())      # Само букви и/или цифри ли е? -> True
print("   ".isspace())          # Само празни интервали ли е? -> True
print("file.pdf".endswith(".pdf"))      # Завършва ли на '.pdf'? -> True
print("https://bg".startswith("https")) # Започва ли с 'https'? -> True

# 2. Работа с ASCII кодове (ord и chr)
ascii_code = ord('A')           # Буква -> ASCII число (65)
character = chr(65)             # ASCII число -> Буква ('A')
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис | Пример / Резултат |
| :--- | :--- | :--- |
| **Изчистване на интервали** | `s.strip()` | Маха празни места в началото и края |
| **Малки / Главни букви** | `s.lower()` / `s.upper()` | Променя регистъра на целия текст |
| **Замяна на текст** | `s.replace("стар", "нов")` | Заменя всички срещания |
| **Проверка за подниз** | `if "sub" in s:` | Връща `True` или `False` |
| **Индекс на търсена дума** | `s.find("дума")` | Връща индекса или `-1` ако липсва |
| **Начало / Край на текст** | `s.startswith()` / `s.endswith()` | Удобно за проверка на файлови разширения |
| **Проверка за цифри / букви** | `s.isdigit()` / `s.isalpha()` | Валидира съдържанието на стринга |
| **Символ ➔ ASCII / ASCII ➔ Символ** | `ord('a')` / `chr(97)` | `ord('a')` връща `97` |

---
[⬅️ 01-08-00. Теория & Капани](01-08-00-Text-Processing.md) | 📂 Папка: Module-01-Basic-and-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 01-09-00. Regular Expressions](01-09-00-Regular-Expressions.md)
