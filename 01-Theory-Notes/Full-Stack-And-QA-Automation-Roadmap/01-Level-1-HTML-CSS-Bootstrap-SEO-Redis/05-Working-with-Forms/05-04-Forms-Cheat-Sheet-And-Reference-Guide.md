[⬅️ 05-03. Инженерен Интервю Справочник](05-03-Advanced-Forms-Traps-And-Snippets.md) | 📂 Папка: 05-Working-with-Forms | [🏠 Начало](../../README.md)

---

# 05-04. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 05:** Работа с уеб форми | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за атрибутите, контролите и декларативните валидатори на уеб формите, придружена от готов за производство UI софтуерен компонент.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Свойства и Атрибути на уеб формите](#1-global-reference-guide-свойства-и-атрибути-на-уеб-формите)
* [2. FORMS VALIDATION CHEAT SHEET (Бърз синтактичен пищов)](#2-forms-validation-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI SNIPPET: "Професионална Студентска Форма за Оценяване"](#3-готов-ui-snippet-професионална-студентска-форма-за-оценяване)

---

## 1. GLOBAL REFERENCE GUIDE: Свойства и Атрибути на уеб формите

### 🗛 А. Свойства за контрол над структурата:
*   `<form>` ➔ Корен на формуляра. Изисква `action` (ендпоинт път) и `method="POST"` за сигурност.
*   `<input>` ➔ Самонаказващ се входящ елемент. Контролира се от критичния атрибут `name` за .NET Model Binding средите.
*   `<label for="id">` ➔ Етикет за достъпност. Свързва се софтуерно с `id` на съответния инпут.
*   `<select>` + `<option value="key">` ➔ Изгражда падащо меню (Dropdown).
*   `<textarea>` ➔ Многоредово текстово поле (няма атрибут `value`, текстът се пише между таговете).

---

## 2. FORMS VALIDATION CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `required` ➔ Прави попълването на полето **задължително**. Спира изпращането, ако е празно.
*   `placeholder="..."` ➔ Блед подсказващ текст вътре в кутията, който изчезва при първата написана буква.
*   `disabled` ➔ Деактивира напълно уеб контролата. Браузърът я оцветява в сиво и **игнорира нейните данни при Submit пакетиране**.
*   `<button type="submit">` ➔ Единственият правилен тип за изпитен бутон — той физически задейства изпращането на формата по мрежата.

---

## 3. ГОТОВ UI SNIPPET: "ПРОФЕСИОНАЛНА СТУДЕНТСКА ФОРМА ЗА ОЦЕНЯВАНЕ"

Копирай този готов, семантично свързан HTML и CSS фрагмент, за да имаш напълно застрахована от Model Binding крашове уеб форма:

```html
<!-- Готов уеб компонент за форми, съвместим с .NET среди -->
<div class="form-card-wrapper">
    <form action="/api/v1/students/submit-grade" method="POST" class="evaluation-form">
        <h3 class="form-title">Оценяване на Курсист</h3>

        <!-- Текстово поле с име -->
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
    .form-card-wrapper *, .form-card-wrapper *::before, .form-card-wrapper *::after {
        box-sizing: border-box;
        margin: 0; padding: 0;
    }
    .form-card-wrapper { font-family: Arial, sans-serif; display: flex; justify-content: center; padding: 30px; }
    .evaluation-form { width: 100%; max-width: 450px; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 24px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
    .form-title { color: #0f172a; margin-bottom: 20px; text-align: center; }
    .input-block { margin-bottom: 16px; display: flex; flex-direction: column; gap: 6px; }
    .input-block label { color: #475569; font-size: 14px; font-weight: bold; }
    .input-block input, .input-block select, .input-block textarea { width: 100%; padding: 10px; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 14px; color: #334155; }
    .input-block input:focus, .input-block select:focus, .input-block textarea:focus { outline: none; border-color: #2563eb; }
    .submit-action-btn { width: 100%; background-color: #2563eb; color: white; border: none; padding: 12px; font-size: 16px; font-weight: bold; border-radius: 4px; cursor: pointer; margin-top: 10px; transition: background 0.2s; }
    .submit-action-btn:hover { background-color: #1d4ed8; }
</style>
```

---
[⬅️ 05-03. Инженерен Интервю Справочник](05-03-Advanced-Forms-Traps-And-Snippets.md) | 📂 Папка: 05-Working-with-Forms | [🏠 Начало](../../README.md)
