[⬅️ 05-04-00. Теория & Капани](05-04-00-Forms-And-ModelForms.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-05-00. Class-Based Views](05-05-00-Class-Based-Views.md)

---

# 05-04-01. Django Forms and ModelForms: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на `Form` и `ModelForm`, добавяне на CSS класове (Widgets), персонализирана валидация и HTML рендиране в Django от нулата.

## 🧭 Съдържание на темата
* [1. Създаване на стандартна форма (`forms.Form`)](#1-създаване-на-стандартна-форма-formsform)
* [2. Създаване на форма от модел (`forms.ModelForm`)](#2-създаване-на-форма-от-модел-formsmodelform)
* [3. Валидация на едно поле (`clean_<field>`) и на цялата форма (`clean`)](#3-валидация-на-едно-поле-clean_field-и-на-цялата-форма-clean)
* [4. Рендиране на формата в HTML шаблон (`{% csrf_token %}`)](#4-рендиране-на-формата-в-html-шаблон--csrf_token-)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ НА СТАНДАРТНА ФОРМА (`FORMS.FORM`)

Създай файл **`forms.py`** в папката на приложението си:

```python
from django import forms

class ContactForm(forms.Form):
    full_name = forms.CharField(
        max_length=100,
        label="Вашето име",
        widget=forms.TextInput(attrs={"class": "input-field", "placeholder": "Иван Иванов"})
    )
    email = forms.EmailField(label="Имейл адрес")
    message = forms.CharField(
        widget=forms.Textarea(attrs={"rows": 4}),
        min_length=10
    )
    agree_terms = forms.BooleanField(required=True)
```

---

## 2. СЪЗДАВАНЕ НА ФОРМА ОТ МОДЕЛ (`FORMS.MODELFORM`)

```python
from django import forms
from .models import Book

class BookCreateForm(forms.ModelForm):
    class Meta:
        model = Book
        # Вариант А: Изброяваш точно кои колони да се покажат (Препоръчително!)
        fields = ["title", "author_name", "price", "description"]
        
        # Вариант Б: Всички колони без посочените (по-рисково при добавяне на нови колони)
        # exclude = ["created_at", "owner"]

        labels = {
            "title": "Заглавие на книгата",
            "price": "Цена (лв.)",
        }
        help_texts = {
            "price": "Въведете цена с ДДС.",
        }
```

---

## 3. ВАЛИДАЦИЯ НА ЕДНО ПОЛЕ (`CLEAN_<FIELD>`) И НА ЦЯЛАТА ФОРМА (`CLEAN`)

```python
from django import forms
from django.core.exceptions import ValidationError

class RegisterForm(forms.Form):
    username = forms.CharField(max_length=30)
    password = forms.CharField(widget=forms.PasswordInput)
    confirm_password = forms.CharField(widget=forms.PasswordInput)

    # 1. Валидация само за 'username'
    def clean_username(self):
        username = self.cleaned_data["username"]
        if "admin" in username.lower():
            raise ValidationError("Името не може да съдържа думата 'admin'!")
        return username  # ЗАДЪЛЖИТЕЛНО връщаме почистената стойност!

    # 2. Обща валидация за сравняване на две полета едновременно
    def clean(self):
        cleaned_data = super().clean()
        pwd1 = cleaned_data.get("password")
        pwd2 = cleaned_data.get("confirm_password")

        if pwd1 and pwd2 and pwd1 != pwd2:
            raise ValidationError("Двете пароли не съвпадат!")
        return cleaned_data
```

---

## 4. РЕНДИРАНЕ НА ФОРМАТА В HTML ШАБЛОН (`{% CSRF_TOKEN %}`)

```html
<!-- Ако формата има FileField или ImageField, ЗАДЪЛЖИТЕЛНО добави enctype="multipart/form-data" -->
<form method="post" enctype="multipart/form-data">
    {% csrf_token %}

    <!-- Вариант 1: Бързо автоматично рендиране в <p> или <div> тагове -->
    {{ form.as_div }}

    <!-- Вариант 2: Ръчен контрол върху всяко поле и грешките му -->
    <div class="field-group">
        {{ form.title.label_tag }}
        {{ form.title }}
        {% if form.title.errors %}
            <div class="error">{{ form.title.errors }}</div>
        {% endif %}
    </div>

    <button type="submit">Запази</button>
</form>
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Действие / Конструкция | Синтаксис в Django Forms |
| :--- | :--- |
| **Защита в HTML `<form>`** | `{% csrf_token %}` (без него Django връща `403 Forbidden`) |
| **Форма за редакция (Edit)** | `form = BookForm(request.POST or None, instance=book_obj)` |
| **Проверка на валидност** | `if form.is_valid():` |
| **Достъп до валидните данни** | `form.cleaned_data["field_name"]` |
| **Запис в базата (`ModelForm`)** | `obj = form.save()` |
| **Запис с ръчна промяна преди SQL** | `obj = form.save(commit=False); obj.user = request.user; obj.save()` |
| **Запис на M2M при `commit=False`** | `form.save_m2m()` |
| **Скриване на поле във форма** | `widget=forms.HiddenInput()` |
| **Отключване на поле (Read-only)** | `self.fields["email"].disabled = True` в `__init__` |

---
[⬅️ 05-04-00. Теория & Капани](05-04-00-Forms-And-ModelForms.md) | 📂 Папка: Module-05-Django-Fundamentals | [🏠 Начало](../../../../README.md) | [➡️ 05-05-00. Class-Based Views](05-05-00-Class-Based-Views.md)
