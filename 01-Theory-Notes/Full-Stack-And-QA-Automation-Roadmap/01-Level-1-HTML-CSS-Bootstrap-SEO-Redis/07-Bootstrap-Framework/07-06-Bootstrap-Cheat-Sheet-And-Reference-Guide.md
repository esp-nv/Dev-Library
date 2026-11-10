[⬅️ 07-05. Инженерен Справочник](07-05-Advanced-Bootstrap-Traps-And-Snippets.md) | 📂 Папка: 07-Bootstrap-Framework | [🏠 Начало](../../README.md)

---

# 07-06. Глобален Изпитен и Интервю Cheat Sheet & Reference Guide

**Модул 07:** Работа с Bootstrap v5.3 | **Цел:** Глобална, бърза синтактична справка (Reference Guide) за разширените утилити класове, разпределения на пространства, типография, цветови режими и готов за внедряване валидиран UI шаблон.

## 🧭 Съдържание на темата
* [1. GLOBAL REFERENCE GUIDE: Свойства, Размери и Цветови таксономии](#1-global-reference-guide-свойства-размери-и-цветови-таксономии)
* [2. BOOTSTRAP UTILITIES CHEAT SHEET (Бърз синтактичен пищов)](#2-bootstrap-utilities-cheat-sheet-бърз-синтактичен-пищов)
* [3. ГОТОВ UI SNIPPET: "Професионална Студентска Кошница за Достъп (Checkout Form)"](#3-готов-ui-snippet-професионална-студентска-кошница-за-достъп-checkout-form)

---

## 1. GLOBAL REFERENCE GUIDE: Свойства, Размери и Цветови таксономии

### 🗛 А. Спецификация на Адаптивните Класове за Пространство (Spacing):
Управлението на отстоянията в Bootstrap v5.3 се базира на вградена скала от `0` до `5` (където `1` съответства на `0.25rem` / `4px`, а `3` е основната база `1rem` / `16px`):
*   **Падинг (Padding):** `.p-3` (всички страни, 16px), `.px-4` (хоризонтално ляво/дясно), `.py-2` (вертикално горе/долу), `.pt-5` (само отгоре).
*   **Марджин (Margin):** `.m-5` (максимален марджин, 48px), `.mt-3` (отгоре), `.mb-0` (премахва дефолтния марджин отдолу), `.mx-auto` (хоризонтално центриране на блока).

### 🗛 Б. Семантични Цветови Схеми (Contextual Colors):
Фреймуъркът капсулира бизнес логиката на цветовете в следните заковани класове, приложими за фонове (`bg-*`), текстове (`text-*`), бутони (`btn-*`) и рамки (`border-*`):
*   `.primary` ➔ Основен корпоративен цвят (Син).
*   `.success` ➔ Индикатор за успех, валидност или завършена трансакция (Зелен).
*   `.danger` ➔ Индикатор за критична грешка, отказ от достъп или дефект (Червен).
*   `.warning` ➔ Внимание, предупреждение или изтичаща сесия (Жълт).
*   `.secondary` / `.light` / `.dark` ➔ Спомагателни неутрални сиви и контрастни фонове.

---

## 2. BOOTSTRAP UTILITIES CHEAT SHEET (БЪРЗ СИНТАКТИЧЕН ПИЩОВ)

*   `d-flex align-items-center justify-content-center` ➔ Златният бърз пищов за моментално хоризонтално и вертикално центриране на елемент в родителска кутия.
*   `text-truncate` ➔ Предотвратява счупване на Layout-а при динамични данни — ако текстът надхвърли широчината на колоната, браузърът автоматично го отрязва и инжектира безопасни три точки (`...`).
*   `w-100 h-100` ➔ Разпъва елемента автоматично до заемане на точно 100% широчина и височина от неговия директен родител.
*   `rounded-circle` ➔ Превръща всяка квадратна кутия или снимка в съвършен софтуерен кръг (идеално за потребителски аватари).

---

## 3. ГОТОВ UI SNIPPET: "ПРОФЕСИОНАЛНА СТУДЕНТСКА КОШНИЦА ЗА ДОСТЪП (CHECKOUT FORM)"

Копирай този готов, напълно валидиран HTML и CSS код директно във твоето уеб приложение. Той съдържа комплексна многоколона форма с падащи менюта, чекбоксове, вградени `.invalid-feedback` съобщения за грешки и активирана `.was-validated` симулация за .NET Model Binding средите:

```html
<!-- Изчистен, готов за употреба Bootstrap v5.3 Checkout Компонент -->
<div class="container my-5 font-sans">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            
            <div class="card shadow-sm border-2 rounded-3 p-4">
                <div class="card-body">
                    <h3 class="card-title text-center text-dark mb-2 font-bold">Заявка за Достъп</h3>
                    <p class="text-muted text-center text-sm mb-4">Попълнете формата за активиране на софтуерния модул</p>
                    
                    <!-- Форма със симулирана активна Bootstrap валидация -->
                    <form action="/api/v1/checkout/activate" method="POST" class="was-validated needs-validation" novalidate>
                        
                        <!-- Две колони в ред на компютър (col-md-6) | Една колона на телефон -->
                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <label for="FirstName" class="form-label font-semibold text-sm">Име:</label>
                                <input type="text" id="FirstName" name="FirstName" class="form-control" placeholder="Иван" required />
                                <div class="invalid-feedback text-xs">Моля, въведете име.</div>
                            </div>
                            <div class="col-md-6">
                                <label for="LastName" class="form-label font-semibold text-sm">Фамилия:</label>
                                <input type="text" id="LastName" name="LastName" class="form-control" placeholder="Иванов" required />
                                <div class="invalid-feedback text-xs">Моля, въведете фамилия.</div>
                            </div>
                        </div>

                        <!-- Единично голямо поле за Имейл -->
                        <div class="mb-3">
                            <label for="UserEmail" class="form-label font-semibold text-sm">Имейл адрес:</label>
                            <input type="email" id="UserEmail" name="Email" class="form-control" placeholder="user@softuni.bg" required />
                            <div class="invalid-feedback text-xs">Въведете валиден корпоративен имейл адрес.</div>
                        </div>

                        <!-- Падащо меню (Dropdown Select) -->
                        <div class="mb-3">
                            <label for="ModuleSelect" class="form-label font-semibold text-sm">Избор на Специализация:</label>
                            <select id="ModuleSelect" name="ModuleId" class="form-select" required>
                                <option value="">-- Изберете софтуерен курс --</option>
                                <option value="101">ASP.NET Core Advanced</option>
                                <option value="102">System Design Architecture</option>
                                <option value="103">QA Automation with C#</option>
                            </select>
                            <div class="invalid-feedback text-xs">Трябва да изберете легитимен модул.</div>
                        </div>

                        <!-- Защитена Изборна Капсула (Checkbox) -->
                        <div class="form-check mb-4 pt-1">
                            <input type="checkbox" id="AcceptTerms" name="AcceptTerms" value="true" class="form-check-input" required />
                            <label for="AcceptTerms" class="form-check-label text-sm text-secondary select-none cursor-pointer">
                                Приемам условията за сигурност на личните данни
                            </label>
                            <div class="invalid-feedback text-xs">Декларацията за съгласие е задължителна.</div>
                        </div>

                        <!-- Екшън Бутон с Hover ефекти на Bootstrap -->
                        <button type="submit" class="btn btn-primary w-100 py-2.5 font-bold text-md shadow-sm transition-all">
                            Активирай Софтуерен Лиценз
                        </button>

                    </form>
                </div>
            </div>

        </div>
    </div>
</div>

<style>
    /* Премахва грозния браузърен аутлайн при фокус за перфектен дизайн */
    .form-control:focus, .form-select:focus {
        box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.15) !important;
        border-color: #0d6efd !important;
    }
    .font-sans { font-family: system-ui, -apple-system, sans-serif; }
    .font-bold { font-weight: 700; }
    .font-semibold { font-weight: 600; }
    .text-sm { font-size: 14px; }
    .text-xs { font-size: 12px; }
    .text-md { font-size: 16px; }
    .cursor-pointer { cursor: pointer; }
</style>
```

---
[⬅️ 07-05. Инженерен Справочник](07-05-Advanced-Bootstrap-Traps-And-Snippets.md) | 📂 Папка: 07-Bootstrap-Framework | [🏠 Начало](../../README.md)
