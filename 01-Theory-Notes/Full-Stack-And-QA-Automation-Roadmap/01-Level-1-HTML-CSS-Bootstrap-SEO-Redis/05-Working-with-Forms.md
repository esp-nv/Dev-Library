[⬅️ 04. Адаптивен дизайн](04-Responsive-Web-Design.md) | [📂 Папка: HTML-and-CSS](README.md)  | [🏠 Начало](../../README.md) | [➡️ 06. Модерен Tailwind CSS](06-Tailwind-CSS.md)

---

# 05. Работа с Уеб Форми (Working with Forms — Теория и Практика)

**Цел:** Дълбоко практическо и синтактично овладяване на HTML5 формите, управление на типовете данни (`input` елементи), изграждане на достъпни интерфейси и подсигуряване на съответствието на параметрите с .NET Model Binding средите.

## 🧭 Съдържание на темата
* [1. Анатомия на HTML Уеб Формите (Action и Method среди)](#1-анатомия-на-html-уеб-формите-action-и-method-среди)
* [2. Типове Инпути и Сървърна Валидация на клиентско ниво](#2-типове-инпути-и-сървърна-валидация-на-клиентско-ниво)
* [3. Достъпност и Валидност: Връзката Label-Input](#3-достъпност-и-валидност-връзката-label-input)
* [4. Системни капани: Изчезнали данни (Missing Name) и Срив на HTTP Глагола](#4-системни-капани-изчезнали-данни-missing-name-и-срив-на-http-глагола)
* [5. MODULE CHEAT SHEET & ГОТОВИ UI SNIPPETS (Копируеми кубчета)](#5-module-cheat-sheet-готови-ui-snippets-копируеми-кубчета)

---

## 1. АНАТОМИЯ НА HTML УЕБ ФОРМИТЕ (ACTION И METHOD СРЕДИ)

Уеб формулярите (`<form>`) служат като основен софтуерен мост за изпращане на потребителски данни от браузъра на клиента към уеб сървъра. Всеки `<form>` таг притежава два критични атрибута, които определят дестинацията на мрежовия трафик:
*   **`action`**: Указва точния URL адрес на уеб сървъра (напр. бекенд ендпоинта), който ще приеме и обработи данните (напр. `action="/api/v1/account/register"`).
*   **`method`**: Указва HTTP глагола, който браузърът ще наложи над пакета. За форми най-често се използва **`POST`** (скрива данните вътре в суровото HTTP Body тяло на заявката). Ако използваш `GET`, браузърът ще изплюе данните публично в URL адреса под формата на Query String.

---

## 2. ТИПОВЕ ИНПУТИ И СЪРВЪРНА ВАЛИДАЦИЯ НА КЛИЕНТСКО НИВО

HTML5 предоставя специализирани типове за `<input>` тага, които принуждават браузъра автоматично да наложи базова клиентска валидация още преди пакетът да е напуснал машината:
*   `type="text"` ➔ Стандартно текстово поле.
*   `type="email"` ➔ Браузърът автоматично спира заявката, ако в текста липсва символа `@` или валидно разширение на домейн.
*   `type="password"` ➔ Скрива въвежданите символи зад черни точки с цел сигурност.
*   `type="number"` ➔ Позволява въвеждане само на цифри и добавя стрелки за коригиране. Може да се ограничи с атрибути `min="1"` и `max="100"`.

---

## 3. ДОСТЪПНОСТ И ВАЛИДНОСТ: ВРЪЗКАТА LABEL-INPUT

За да бъде една форма професионално изградена, всеки входящ елемент трябва да притежава четим етикет (**`<label>`**). Връзката между тях се закотвя софтуерно чрез атрибута **`for`** на левъла и уникалното **`id`** на инпута:

```html
<!-- 🟢 СЪВЪРШЕНА СВЪРЗАНОСТ И ДОСТЪПНОСТ: -->
<label for="UserAge">Възраст:</label>
<input type="number" id="UserAge" name="Age" min="18">
```
*Когато клиентът кликне с мишката върху текста „Възраст:“, браузърът автоматично фокусира и отваря съответното текстово поле за писане.*

---

## 4. СИСТЕМНИ КАПАНИ: ИЗЧЕЗНАЛИ ДАННИ (MISSING NAME) И СРИВ НА HTTP ГЛАГОЛА

Неправилното конфигуриране на атрибутите на формата води до пълна парализа на .NET Model Binding средите.

### 🚨 Капан №1: Празни C# обекти в контролера при изпращане на форма (The Missing Name)
*   **Какво правиш:** Имаш работещ ASP.NET Core контролер. Изграждаш HTML форма в браузъра, попълваш я, натискаш бутона Submit, но на бекенда C# свойствата пристигат като `null` или `0`, въпреки че нямаш мрежови грешки.
*   **Защо се случва:** Ти си разписала `id="..."` атрибути за всяко поле, но си забравила да инжектираш критичния атрибут **`name="..."`**. Както научихме в Глава 01, .NET Model Binder сканира входящия HTTP пакет и търси съвпадения по името в атрибута `name`. Ако той липсва, браузърът изобщо не изпраща тези стойности по мрежата.
*   **Как да го поправиш:** Винаги подсигурявай, че стойността в атрибута `name` съвпада абсолютно точно с името на свойството във твоя C# DTO / Input модел:
    ```html
    <!-- ❌ АНТИ-ПАТЕРН (Бекендът ще получи NULL): -->
    <input type="email" id="email_field">

    <!-- 🟢 ПРАВИЛЕН АРХИТЕКТУРЕН СТАНДАРТ: -->
    <input type="email" id="email_field" name="Email">
    ```

### 🚨 Капан №2: Изтичане на данни в URL адреса при липсващ HTTP метод
*   **Какво правиш:** Създаваш форма за регистрация, съдържаща тайно поле за парола. Забравяш да попълниш атрибута `method="POST"` във `<form>` тага.
*   **Бизнес капан:** Потребителят пише паролата си и натиска Submit. Понеже си пропуснала метода, браузърът автоматично налага уеб стандарта по подразбиране — **`GET`**. Паролата се прикрепя чисто в уеб адреса: `https://localhost:7214/register?Password=123456`. Тя се записва в историята на браузъра, в уеб логовете на сървъра и става лесна плячка за хакери.
*   **Как да го поправиш:** Винаги за форми, които пишат на диска или пренасят пароли, изрично заковавай: `<form method="POST">`.

---

## 5. MODULE CHEAT SHEET & ГОТОВИ UI SNIPPETS (КОПИРУЕМИ КУБЧЕТА)

### 🛠️ Бърз Изпитен Пищов (Cheat Sheet):
*   `required` ➔ Атрибут, който прави полето **задължително за попълване**. Браузърът няма да пусне заявката, ако е празно.
*   `placeholder="текст"` ➔ Извежда блед подсказващ текст вътре в кутията, който изчезва веднага щом клиентът започне да пише.
*   `<button type="submit">` ➔ Единственият правилен тип за бутон — той физически задейства изпращането на формата по мрежата.

### 💾 Готов за директно ползване Snippet: "Професионална Студентска Форма за Оценяване"
Копирай този готов HTML и CSS откъс. Той съдържа пълна, семантично свързана форма с падащо меню (`<select>`), текстова зона (`<textarea>`) и сигурни `name` атрибути за .NET:

```html
<!-- Готов уеб компонент за форми -->
<div class="form-card-wrapper">
    <form action="/api/v1/students/submit-grade" method="POST" class="evaluation-form">
        <h3 class="form-title">Оценяване на Курсист</h3>

        <!-- Текстово поле -->
        <div class="input-block">
            <label for="StudentName">Име на студент:</label>
            <input type="text" id="StudentName" name="StudentName" placeholder="Георги Георгиев" required>
        </div>

        <!-- Падащо меню (Dropdown) -->
        <div class="input-block">
            <label for="CourseSelect">Избор на курс:</label>
            <select id="CourseSelect" name="CourseId" required>
                <option value="">-- Изберете софтуерен модул --</option>
                <option value="1">HTML and CSS</option>
                <option value="2">ASP.NET Core Fundamentals</option>
                <option value="3">System Design Architecture</option>
            </select>
        </div>

        <!-- Голяма текстова зона -->
        <div class="input-block">
            <label for="FeedbackNotes">Рецензия и бележки:</label>
            <textarea id="FeedbackNotes" name="ReviewNotes" rows="4" placeholder="Напишете коментар за проекта..."></textarea>
        </div>

        <!-- Изпитен Submit бутон -->
        <button type="submit" class="submit-action-btn">Запиши Оценка</button>
    </form>
</div>

<style>
    .form-card-wrapper *, 
    .form-card-wrapper *::before, 
    .form-card-wrapper *::after {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
    }

    .form-card-wrapper {
        font-family: Arial, sans-serif;
        display: flex;
        justify-content: center;
        padding: 30px;
    }

    .evaluation-form {
        width: 100%;
        max-width: 450px;
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 8px;
        padding: 24px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.05);
    }

    .form-title {
        color: #0f172a;
        margin-bottom: 20px;
        text-align: center;
    }

    .input-block {
        margin-bottom: 16px;
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .input-block label {
        color: #475569;
        font-size: 14px;
        font-weight: bold;
    }

    .input-block input,
    .input-block select,
    .input-block textarea {
        width: 100%;
        padding: 10px;
        border: 1px solid #cbd5e1;
        border-radius: 4px;
        font-size: 14px;
        color: #334155;
    }

    .input-block input:focus,
    .input-block select:focus,
    .input-block textarea:focus {
        outline: none;
        border-color: #3b82f6;
    }

    .submit-action-btn {
        width: 100%;
        background-color: #2563eb;
        color: white;
        border: none;
        padding: 12px;
        font-size: 16px;
        font-weight: bold;
        border-radius: 4px;
        cursor: pointer;
        margin-top: 10px;
        transition: background 0.2s;
    }

    .submit-action-btn:hover {
        background-color: #1d4ed8;
    }
</style>
```
---

[⬅️ 04. Адаптивен дизайн](04-Responsive-Web-Design.md) | [📂 Папка: HTML-and-CSS](README.md)  | [🏠 Начало](../../README.md) | [➡️ 06. Модерен Tailwind CSS](06-Tailwind-CSS.md)

---

© 2026 Dev-Library / Уеб Форми: Сървърна Валидация и Изпитни Тактики