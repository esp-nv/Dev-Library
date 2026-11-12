[⬅️ 04-04-00. Теория & Капани](04-04-00-Spring-MVC-Thymeleaf.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-05-00. State Management (Cookies & Session)](04-05-00-State-Management.md)

---

# 04-04-01. Spring MVC with Thymeleaf & Validation: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за Spring MVC контролери, Jakarta Bean Validation анотации и Thymeleaf HTML шаблони.

## 🧭 Съдържание на темата
* [1. DTO с Jakarta Bean Validation анотации (`@NotBlank`, `@Size`, `@Email`, `@Positive`)](#1-dto-с-jakarta-bean-validation-анотации-notblank-size-email-positive)
* [2. Spring MVC Контролер (`@GetMapping`, `@PostMapping`, `@PathVariable`, `@RequestParam`)](#2-spring-mvc-контролер-getmapping-postmapping-pathvariable-requestparam)
* [3. Thymeleaf HTML шаблон за форма с валидационни грешки](#3-thymeleaf-html-шаблон-за-форма-с-валидационни-грешки)
* [4. Thymeleaf HTML шаблон за списък и условия (`th:each`, `th:if`, `th:href`)](#4-thymeleaf-html-шаблон-за-списък-и-условия-theach-thif-thhref)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. DTO С JAKARTA BEAN VALIDATION АНОТАЦИИ (`@NOTBLANK`, `@SIZE`, `@EMAIL`, `@POSITIVE`)

Изисква зависимостта `spring-boot-starter-validation` в `pom.xml` с анотации като `@NotBlank`, `@Size`, `@NotNull`, `@Positive` и `@Email` върху полетата на DTO класа.

## 2. SPRING MVC КОНТРОЛЕР (`@GETMAPPING`, `@POSTMAPPING`, `@PATHVARIABLE`, `@REQUESTPARAM`)

Използване на `@Controller` с `@RequestMapping`, `@GetMapping` и `@PostMapping` анотации, комбинирани с `@RequestParam` и `@PathVariable` за обработка на входящи HTTP заявки и предаване на данни към `Model`.

## 3. THYMELEAF HTML ШАБЛОН ЗА ФОРМА С ВАЛИДАЦИОННИ ГРЕШКИ

Използване на `th:object`, `th:field` и `#fields.hasErrors()` за визуализиране на полетата от формата и съответните съобщения за грешки.

## 4. THYMELEAF HTML ШАБЛОН ЗА СПИСЪК И УСЛОВИЯ (`TH:EACH`, `TH:IF`, `TH:HREF`)

Използване на `th:if`, `th:unless`, `th:each` за итерация по колекции и динамично генериране на линкове чрез `th:href`.

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Thymeleaf Синтаксис | Предназначение | Пример |
| :--- | :--- | :--- |
| **`${variable}`** | Достъпва атрибут от Spring `Model` | `th:text="${username}"` |
| **`*{field}`** | Достъпва поле от обекта в `th:object` | `th:field="*{email}"` |
| **`@{/url}`** | Генерира контекстно-безопасен URL линк | `th:href="@{/users/{id}(id=${u.id})}"` |
| **`~{fragments/nav :: navbar}`** | Вгражда споделен HTML фрагмент (Header/Footer) | `th:replace="~{fragments/nav :: navbar}"` |
| **`th:each="item : ${list}"`** | Итерира през колекция в HTML | Създава по 1 елемент за всеки запис |
| **`th:if` / `th:unless`** | Условно показване / скриване на HTML блок | `th:if="${isAdmin}"` |

---
[⬅️ 04-04-00. Теория & Капани](04-04-00-Spring-MVC-Thymeleaf.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-05-00. State Management (Cookies & Session)](04-05-00-State-Management.md)
