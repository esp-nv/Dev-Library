[⬅️ 01-00. Git & GitHub Архитектура](01-00-Git-And-GitHub-Architecture.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [🏠 Начало 01-Theory-Notes](../../README.md) | [📂 Текуща папка: Git & GitHub](README.md)| [📂 Следващ домейн: 02. Frontend & UI/UX](../../02-Frontend-And-UI-UX/README.md) | [➡️ Следващ модул: Алгоритми](../Algorithms-And-Data-Structures/README.md)


---

# 00-03-01. Git, GitHub Desktop & TortoiseGit: Практика от Нулата & Cheat Sheet

**Раздел 00:** Course Intro & Development Environment Setup | **Цел:** Практическо ръководство от нулата за първоначална настройка на Git и SSH/HTTPS автентикация, ежедневни команди в **Git CLI** (вход / изход), стъпка по стъпка работа с **GitHub Desktop** и **TortoiseGit („Костенурката“)**, разрешаване на **Merge Conflicts** и 3-колонна справочна таблица (**Cheat Sheet**).

## 🧭 Съдържание на темата
* [1. Първоначална конфигурация на Git от нулата (Терминал)](#1-първоначална-конфигурация-на-git-от-нулата-терминал)
* [2. Ежедневен Git CLI работен цикъл (Вход / Изход)](#2-ежедневен-git-cli-работен-цикъл-вход--изход)
* [3. Практически наръчник за GitHub Desktop (Стъпка по стъпка)](#3-практически-наръчник-за-github-desktop-стъпка-по-стъпка)
* [4. Практически наръчник за TortoiseGit („Костенурката“) в Windows Explorer](#4-практически-наръчник-за-tortoisegit-костенурката-в-windows-explorer)
* [5. Как се разрешава Git Merge Conflict за 30 секунди?](#5-как-се-разрешава-git-merge-conflict-за-30-секунди)
* [6. Обобщение / Бърз Cheat Sheet](#6-обобщение--бърз-cheat-sheet)

---

## 1. ПЪРВОНАЧАЛНА КОНФИГУРАЦИЯ НА GIT ОТ НУЛАТА (ТЕРМИНАЛ)

След инсталация на [Git](https://git-scm.com), отвори терминала и представи името и имейла си (те се записват във всеки твой комит):

```bash
# 1. Задаване на глобално име и имейл (трябва да съвпада с имейла в GitHub)
git config --global user.name "Твоето Име"
git config --global user.email "tvoiat.email@example.com"

# 2. Задаване на 'main' като основен клон и VS Code като редактор по подразбиране
git config --global init.defaultBranch main
git config --global core.editor "code --wait"

# 3. Проверка на текущите настройки
git config --list
```

---

## 2. ЕЖЕДНЕВЕН GIT CLI РАБОТЕН ЦИКЪЛ (ВХОД / ИЗХОД)

### 🔹 Сценарий А: Клониране на съществуващо хранилище от GitHub и качване на промени
* **Вход (Терминал):**
  ```bash
  git clone https://github.com
  cd Dev-Library
  git status
  git add README.md
  git commit -m "docs: update level 1 roadmap links"
  git push origin main
  ```
* **Изход в конзолата:**
  ```text
  [main 8f4a21c] docs: update level 1 roadmap links
   1 file changed, 12 insertions(+), 4 deletions(-)
  To https://github.com
     3a1b09e..8f4a21c  main -> main
  ```

---

## 3. ПРАКТИЧЕСКИ НАРЪЧНИК ЗА GITHUB DESKTOP (СТЪПКА ПО СТЪПКА)

**GitHub Desktop** е най-удобният визуален клиент за синхронизация с GitHub без писане на команди:

1. **Клониране на проект:** `File` ➔ `Clone Repository...` (`Ctrl + Shift + O`) ➔ избираш хранилището от списъка и локалната папка на диска.
2. **Преглед на промените (Changes Tab):** В лявата колона виждаш всички модифицирани файлове (`M` = Modified, `+` = New, `-` = Deleted). В дясната виждаш точните редове (червено = изтрито, зелено = добавено).
3. **Създаване на Commit:** Сложи отметка на файловете, които искаш да включиш ➔ долу вляво напиши кратко описание в полето **Summary** ➔ натисни синия бутон **`Commit to main`**.
4. **Изпращане и изтегляне:** Натисни бутона **`Push origin`** горе в лентата (или **`Fetch origin`** / **`Pull origin`**, за да свалиш промените, направени директно в сайта на GitHub).
5. **Създаване на нов клон:** Натисни менюто **`Current Branch`** горе ➔ **`New Branch`** ➔ въведи име (напр. `feature/ui-ux`) ➔ **`Publish branch`**.

---

## 4. ПРАКТИЧЕСКИ НАРЪЧНИК ЗА TORTOISEGIT („КОСТЕНУРКАТА“) В WINDOWS EXPLORER

**TortoiseGit** се интегрира директно в файловия мениджър на Windows (изисква първо да има инсталиран стандартен Git за Windows):

### 🔹 Значение на иконите върху папките и файловете в Windows Explorer:
* 🟢 **Зелена отметка (`✓`):** Файлът е чист и напълно синхронизиран с последната версия в репозиторито.
* 🔴 **Червен удивителен (`!`):** Файлът е модифициран локално и чака да бъде комитнат.
* 🔵 **Син плюс (`+`):** Нов файл, добавен в Staging Area (`Add`).
* ❓ **Син въпросителен (`?`):** Нов файл (`Untracked`), който още не е добавен в Git.

### 🔹 Основни действия с десен бутон върху папката на проекта:
1. **`Git Commit -> "main"...`:** Отваря прозорец с всички променени файлове, поле за съобщение и бутон **`Commit & Push`** (извършва двете операции наведнъж!).
2. **`TortoiseGit` ➔ `Pull...` / `Fetch...`:** Изтегля последните промени от GitHub.
3. **`TortoiseGit` ➔ `Show Log`:** Отваря мощна визуална графика на всички клонове, комити, автори и променени редове по дати.
4. **`TortoiseGit` ➔ `Diff with previous version`:** Сравнява текущия файл със старата му версия едно до друго (Side-by-Side).
5. **`TortoiseGit` ➔ `Revert...`:** Връща файла към последното запазено състояние (изтрива нежеланите локални промени).

---

## 5. КАК СЕ РАЗРЕШАВА GIT MERGE CONFLICT ЗА 30 СЕКУНДИ?

Когато си променил файл едновременно в сайта на GitHub и локално на компютъра, при `Pull` се появяват конфликтни маркери:

```markdown
<<<<<<< HEAD
Новият текст, който написах локално на компютъра си.
=======
Старият или различен текст, който е на сървъра в GitHub.
>>>>>>> origin/main
```

### 🛠️ Стъпки за фиксиране:
1. Отвори файла във **VS Code** (през бутона *Open in Visual Studio Code* в GitHub Desktop или директно).
2. Използвай бутоните над конфликта:
   * **`Accept Current Change`** — запазва само твоята локална версия (`HEAD`).
   * **`Accept Incoming Change`** — запазва само версията от GitHub (`origin/main`).
   * **`Accept Both Changes`** — запазва и двете версии една под друга.
3. Изтрий ръчно маркерите (`<<<<<<<`, `=======`, `>>>>>>>`), ако останат, и запази файла (`Ctrl + S`).
4. Натисни **`Continue merge`** в GitHub Desktop (или `git add .` ➔ `git commit` в терминала) и **`Push origin`**.

---

## 6. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Задача | Git CLI Команда | Еквивалент в GitHub Desktop / TortoiseGit |
| :--- | :--- | :--- |
| **Сваляне на проект от GitHub** | `git clone <url>` | **Desktop:** `File -> Clone Repository`<br>**Tortoise:** Десен бутон -> `Git Clone...` |
| **Проверка на промените** | `git status` / `git diff` | **Desktop:** Таб `Changes`<br>**Tortoise:** Цветните иконки в Explorer |
| **Добавяне и Запис (Commit)** | `git add .`<br>`git commit -m "msg"` | **Desktop:** Попълни Summary -> `Commit to main`<br>**Tortoise:** Десен бутон -> `Git Commit -> "main"` |
| **Качване в GitHub (Push)** | `git push origin main` | **Desktop:** Бутон `Push origin`<br>**Tortoise:** `TortoiseGit -> Push...` |
| **Сваляне от GitHub (Pull)** | `git pull origin main` | **Desktop:** Бутон `Pull origin`<br>**Tortoise:** `TortoiseGit -> Pull...` |
| **Нов клон и превключване** | `git checkout -b feature/x` | **Desktop:** `Current Branch -> New Branch`<br>**Tortoise:** `TortoiseGit -> Create Branch...` |
| **Временно скриване на промени** | `git stash` / `git stash pop` | **Desktop:** `Branch -> Stash All Changes`<br>**Tortoise:** `TortoiseGit -> Stash Save / Pop` |
| **Отмяна на промени във файл** | `git checkout -- file.md` | **Desktop:** Десен бутон върху файла -> `Discard Changes`<br>**Tortoise:** `TortoiseGit -> Revert...` |

---
[⬅️ 01-00. Git & GitHub Архитектура](01-00-Git-And-GitHub-Architecture.md) | [⬅️ Назад към 01-Core-Tools-And-Fundamentals](../README.md) | [🏠 Начало 01-Theory-Notes](../../README.md) | [📂 Текуща папка: Git & GitHub](README.md)| [📂 Следващ домейн: 02. Frontend & UI/UX](../../02-Frontend-And-UI-UX/README.md) | [➡️ Следващ модул: Алгоритми](../Algorithms-And-Data-Structures/README.md)

---
© 2026 Dev-Library / 00-03-01. Git, GitHub Desktop & TortoiseGit: Практика от Нулата & Cheat Sheet
