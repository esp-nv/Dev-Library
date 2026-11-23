[⬅️ 02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md) | [📂 Предишна папка: 01. HTML5 & CSS3 Foundations](../01-HTML-And-CSS-Foundations/README.md) | [📂 Текуща папка: 02. Responsive, Forms & Frameworks](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 03. Technical SEO & Redis Caching](../03-SEO-And-Redis-Caching/README.md) | [➡️ 02-03-00. Tailwind CSS (Utility-First Архитектура)](02-03-00-Tailwind-CSS-Utility-First.md)

---

# 02-02-01. Syntax-From-Scratch & Validated Form Snippets + Cheat Sheet

**Раздел 02:** Responsive Design, Forms & CSS Frameworks | **Цел:** Практическо изграждане на семантични и валидирани HTML5 форми от нулата, качване на файлове (`enctype="multipart/form-data"`), вход/изход примери, табличен **Module Cheat Sheet** и готов за копиране компонент за регистрационна форма.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Свързване на Label, Input и Submit бутон](#1-стъпка-по-стъпка-свързване-на-label-input-и-submit-бутон)
* [2. Синтаксис от нулата: Качване на файлове, Select и Radio бутони (Вход / Изход)](#2-синтаксис-от-нулата-качване-на-файлове-select-и-radio-бутони-вход--изход)
* [3. Module Cheat Sheet (HTML5 Input Типове & Валидационни атрибути)](#3-module-cheat-sheet-html5-input-типове--валидационни-атрибути)
* [4. Готов за директно ползване Snippet: Валидирана Регистрационна Форма](#4-готов-за-директно-ползване-snippet-валидирана-регистрационна-форма)
* [5. Системни капани: Качване на файл без multipart/form-data и Checkbox липсваща стойност](#5-системни-капани-качване-на-файл-без-multipartform-data-и-checkbox-липсваща-стойност)


---

## 1. СТЪПКА ПО СТЪПКА: СВЪРЗВАНЕ НА LABEL, INPUT И SUBMIT БУТОН

Винаги свързвай `<label>` с `<input>` чрез еднакви стойности в `for` и `id`. Така при клик върху текста на етикета курсорът автоматично влиза в полето:

```html
<form action="/api/login" method="POST">
    <div class="form-group">
        <label for="login-email">Имейл адрес:</label>
        <input type="email" id="login-email" name="email" required placeholder="name@example.com" autocomplete="email">
    </div>
    <button type="submit">Вход в системата</button>
</form>
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: КАЧВАНЕ НА ФАЙЛОВЕ, SELECT И RADIO БУТОНИ (ВХОД / ИЗХОД)

### 🔹 Пример А: Група от Radio бутони (Единствен избор)
* **Вход (`index.html`):**
  ```html
  <fieldset>
      <legend>Ниво на достъп:</legend>
      <label><input type="radio" name="role" value="qa" checked> QA Engineer</label>
      <label><input type="radio" name="role" value="dev"> Full-Stack Developer</label>
  </fieldset>
  ```
* **Изход в HTTP Request Body (при избор на втори бутон):** `role=dev`
* *Забележка:* Всички radio бутони от една група **задължително** споделят еднакво `name="role"`, за да се изключват взаимно.

### 🔹 Пример Б: Падащо меню (`<select>`) с групиране (`<optgroup>`)
* **Вход:**
  ```html
  <label for="stack">Избери стек:</label>
  <select id="stack" name="techStack" required>
      <option value="" disabled selected>-- Моля изберете --</option>
      <optgroup label="Front-End">
          <option value="ts">TypeScript & Vue</option>
          <option value="react">React</option>
      </optgroup>
      <optgroup label="Back-End">
          <option value="java">Java Spring</option>
          <option value="csharp">C# .NET</option>
      </optgroup>
  </select>
  ```
* **Изход в HTTP Request Body:** `techStack=java` (изпраща се стойността от атрибута `value`, а не видимия текст).

---

## 3. MODULE CHEAT SHEET (HTML5 INPUT ТИПОВЕ & ВАЛИДАЦИОННИ АТРИБУТИ)

### 🎛️ Основни HTML5 `<input>` типове

| Тип (`type="..."`) | Предназначение | Вградено поведение в браузъра |
| :--- | :--- | :--- |
| `text` | Стандартен едноредов текст | Приема произволни символи (име, град, заглавие). |
| `password` | Поле за парола | Маскира символите с точки/звездички. |
| `email` | Имейл адрес | Валидира наличието на `@` и домейн преди изпращане. |
| `number` | Числови стойности | Добавя стрелки за увеличаване/намаляване (`step`, `min`, `max`). |
| `date` / `datetime-local` | Избор на дата и час | Отваря вградения календар на операционната система. |
| `file` | Прикачване на файлове | Изисква `enctype="multipart/form-data"` на `<form>` (поддържа `accept=".pdf,.png"`). |
| `checkbox` | Множествен избор (Да/Не) | Изпраща `value` само ако е отметнат (`checked`). |
| `radio` | Единичен избор от група | Изисква еднакво `name` за всички опции в групата. |
| `hidden` | Скрито поле | Пренася системни данни (напр. `CSRF` токен или `userId`) без да се вижда. |

### 🛡️ Атрибути за клиентска валидация и UX

| Атрибут | Пример | Ефект върху полето |
| :--- | :--- | :--- |
| `required` | `<input type="text" required>` | Полето е задължително за попълване. |
| `minlength` / `maxlength` | `minlength="8" maxlength="32"` | Ограничава минималния и максималния брой символи. |
| `min` / `max` / `step` | `min="1" max="100" step="5"` | Задава граници и стъпка при числови и дата полета. |
| `pattern` | `pattern="[0-9]{10}"` | Валидира текста по регулярен израз (напр. точно 10 цифри за телефон). |
| `readonly` vs `disabled` | `readonly` / `disabled` | `readonly` се изпраща със заявката; `disabled` се игнорира и не се праща! |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: ВАЛИДИРАНА РЕГИСТРАЦИОННА ФОРМА

Копирай този готов, семантичен и визуално валидиран компонент с CSS `:focus` и `:invalid` индикатори:

```html
<div class="auth-form-wrapper">
    <form action="/api/v1/register" method="POST" class="modern-form">
        <h2>Създаване на профил</h2>

        <div class="field-box">
            <label for="reg-username">Потребителско име *</label>
            <input type="text" id="reg-username" name="username" required minlength="3" maxlength="20" placeholder="Мин. 3 символа">
        </div>

        <div class="field-box">
            <label for="reg-email">Служебен имейл *</label>
            <input type="email" id="reg-email" name="email" required placeholder="dev@company.com">
        </div>

        <div class="field-box">
            <label for="reg-pass">Парола (мин. 8 символа) *</label>
            <input type="password" id="reg-pass" name="password" required minlength="8" placeholder="••••••••">
        </div>

        <label class="checkbox-row">
            <input type="checkbox" name="acceptTerms" value="true" required>
            <span>Съгласен съм с общите условия за ползване</span>
        </label>

        <button type="submit" class="btn-submit">Регистрация</button>
    </form>
</div>

<style>
    .auth-form-wrapper * {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }

    .auth-form-wrapper {
        font-family: 'Segoe UI', sans-serif;
        max-width: 420px;
        margin: 20px auto;
        padding: 24px;
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
    }

    .modern-form h2 {
        margin-bottom: 18px;
        color: #0f172a;
        font-size: 22px;
    }

    .field-box {
        margin-bottom: 14px;
    }

    .field-box label {
        display: block;
        margin-bottom: 6px;
        font-size: 14px;
        font-weight: 600;
        color: #334155;
    }

    .field-box input {
        width: 100%;
        padding: 10px 12px;
        border: 1px solid #cbd5e1;
        border-radius: 6px;
        font-size: 15px;
        outline: none;
        transition: border-color 0.2s;
    }

    .field-box input:focus {
        border-color: #2563eb;
    }

    /* Визуална индикация само след като потребителят е въвел текст */
    .field-box input:not(:placeholder-shown):invalid {
        border-color: #ef4444;
    }

    .field-box input:not(:placeholder-shown):valid {
        border-color: #10b981;
    }

    .checkbox-row {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 13px;
        color: #475569;
        margin: 16px 0;
        cursor: pointer;
    }

    .btn-submit {
        width: 100%;
        padding: 12px;
        background-color: #2563eb;
        color: #ffffff;
        border: none;
        border-radius: 6px;
        font-size: 16px;
        font-weight: 600;
        cursor: pointer;
    }

    .btn-submit:hover {
        background-color: #1d4ed8;
    }
</style>
```

---

## 5. СИСТЕМНИ КАПАНИ: КАЧВАНЕ НА ФАЙЛ БЕЗ MULTIPART/FORM-DATA И CHECKBOX ЛИПСВАЩА СТОЙНОСТ

### 🚨 Капан №1: Слагаш `<input type="file">`, но сървърът получава само името на файла като текст, а не самия файл
* **Защо се случва:** По подразбиране `<form>` кодира данните като `application/x-www-form-urlencoded` (обикновен текст), който не може да пренася двоични (binary) файлове.
* **Как да го поправиш:** Добави задължително **`enctype="multipart/form-data"`** в отварящия таг `<form action="/upload" method="POST" enctype="multipart/form-data">`.

### 🚨 Капан №2: Неотметнат Checkbox изобщо не присъства в заявката към сървъра
* **Защо се случва:** По спецификация на HTML, ако потребителят **не отметне** `<input type="checkbox" name="subscribe">`, браузърът изобщо не изпраща ключа `subscribe` към сървъра (за разлика от текстовите полета, които се пращат като празен низ `subscribe=`).

---
[⬅️ 02-02-00. Работа с HTML форми и клиентска валидация](02-02-00-Working-With-Forms-And-Validation.md) | [📂 Предишна папка: 01. HTML5 & CSS3 Foundations](../01-HTML-And-CSS-Foundations/README.md) | [📂 Текуща папка: 02. Responsive, Forms & Frameworks](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 03. Technical SEO & Redis Caching](../03-SEO-And-Redis-Caching/README.md) | [➡️ 02-03-00. Tailwind CSS (Utility-First Архитектура)](02-03-00-Tailwind-CSS-Utility-First.md)
---
© 2026 Dev-Library / 02-02-01. Syntax-From-Scratch & Validated Form Snippets + Cheat Sheet
