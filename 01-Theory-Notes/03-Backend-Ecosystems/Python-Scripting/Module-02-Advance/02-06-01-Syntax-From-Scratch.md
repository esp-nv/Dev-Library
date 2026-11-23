[⬅️ 02-06-00. Теория & Капани](02-06-00-File-Handling.md) | [📂 Папка: Module-02-Advance](README.md) | [🏠 Начало](../../../../README.md) | [➡️ Модул 03: OOP](../Module-03-OOP/README.md)

---

# 02-06-01. File Handling: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за четене, писане, дописване, проверка и изтриване на файлове и папки в Python от нулата.

## 🧭 Съдържание на темата
* [1. Писане (`"w"`) и Дописване (`"a"`) във файл](#1-писане-w-и-дописване-a-във-файл)
* [2. Четене от файл (`"r"`) — трите основни начина](#2-четене-от-файл-r--трите-основни-начина)
* [3. Работа с файлове и папки (`os` и `pathlib`)](#3-работа-с-файлове-и-папки-os-и-pathlib)
* [4. Работа с JSON файлове (Четене и Запис)](#4-работа-с-json-файлове-четене-и-запис)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПИСАНЕ (`"W"`) И ДОПИСВАНЕ (`"A"`) ВЪВ ФАЙЛ

*Запомни: `.write()` НЕ слага нов ред автоматично — трябва изрично да напишеш `\n`!*

```python
# 1. СЪЗДАВАНЕ И ПРЕЗАПИСВАНЕ ("w" - трие старото съдържание!)
with open("notes.txt", "w", encoding="utf-8") as file:
    file.write("Първи ред в документа.\n")
    file.write("Втори ред в документа.\n")

# 2. ДОБАВЯНЕ В КРАЯ ("a" - запазва старото и пише отдолу)
with open("notes.txt", "a", encoding="utf-8") as file:
    file.write("Трети ред, добавен по-късно.\n")
```

---

## 2. ЧЕТЕНЕ ОТ ФАЙЛ (`"R"`) — ТРИТЕ ОСНОВНИ НАЧИНА

```python
# Вариант 1: Четене ред по ред с цикъл (Най-добрият и безопасен начин!)
with open("notes.txt", "r", encoding="utf-8") as file:
    for line in file:
        print(line.strip())  # .strip() маха невидимия символ за нов ред \n

# Вариант 2: Четене на целия файл като един голям текст (само за малки файлове)
with open("notes.txt", "r", encoding="utf-8") as file:
    full_text = file.read()

# Вариант 3: Четене на всички редове като списък от текстове
with open("notes.txt", "r", encoding="utf-8") as file:
    lines_list = file.readlines()   # ['Първи ред...\n', 'Втори ред...\n']
```

---

## 3. РАБОТА С ФАЙЛОВЕ И ПАПКИ (`OS` И `PATHLIB`)

```python
import os
from pathlib import Path

# 1. Проверка дали файл съществува преди изтриване
if os.path.exists("notes.txt"):
    os.remove("notes.txt")          # Изтрива файла от диска
    print("Файлът е изтрит.")

# 2. Модерен начин с pathlib (препоръчителен!)
file_path = Path("data/users.txt")

if file_path.exists():              # Проверява дали съществува
    content = file_path.read_text(encoding="utf-8")   # Чете директно на 1 ред!
    file_path.unlink()              # Изтрива файла
```

---

## 4. РАБОТА С JSON ФАЙЛОВЕ (ЧЕТЕНЕ И ЗАПИС)

Тъй като Python речниците изглеждат точно като JSON, вграденият модул `json` прави записа и четенето на конфигурации изключително лесни:

```python
import json

user_data = {"name": "Елизабет", "level": 2, "skills": ["Python", "TypeScript", "QA"]}

# 1. Записване на речник директно в JSON файл (json.dump)
with open("profile.json", "w", encoding="utf-8") as f:
    json.dump(user_data, f, ensure_ascii=False, indent=4) # ensure_ascii=False пази кирилицата четима!

# 2. Четене на JSON файл обратно в Python речник (json.load)
with open("profile.json", "r", encoding="utf-8") as f:
    loaded_data = json.load(f)
    print(loaded_data["name"])      # Изход: Елизабет
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие | Синтаксис | Бележка / Ефект |
| :--- | :--- | :--- |
| **Безопасно отваряне** | `with open("f.txt", "r", encoding="utf-8") as f:` | Затваря файла автоматично |
| **Режим за четене / писане** | `"r"` / `"w"` / `"a"` / `"x"` | `"w"` изтрива старото съдържание |
| **Четене ред по ред** | `for line in f:` | Не натоварва RAM паметта |
| **Запис на текст** | `f.write("Текст\n")` | Изисква ръчно `\n` за нов ред |
| **Връщане в началото** | `f.seek(0)` | Мести курсора на байт 0 |
| **Проверка за файл** | `os.path.exists("f.txt")` или `Path("f.txt").exists()` | Връща `True` или `False` |
| **Изтриване на файл** | `os.remove("f.txt")` или `Path("f.txt").unlink()` | Изтрива файла от диска |
| **Запис / Четене на JSON** | `json.dump(obj, f)` / `json.load(f)` | Конвертира `dict <-> JSON` файл |

---
[⬅️ 02-06-00. Теория & Капани](02-06-00-File-Handling.md) | [📂 Папка: Module-02-Advance](README.md) | [🏠 Начало](../../../../README.md) | [➡️ Модул 03: OOP](../Module-03-OOP/README.md)
