[⬅️ 04-05-00. Теория & Капани](04-05-00-State-Management.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-06-00. Spring Essentials (Events & Scheduling)](04-06-00-Spring-Essentials.md)

---

# 04-05-01. State Management (Cookies & HttpSession): Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване, четене и изтриване на Cookies (`ResponseCookie` и `@CookieValue`), работа с `HttpSession` и конфигуриране на сесийни настройки в `application.properties` в Java от нулата.

## 🧭 Съдържание на темата
* [1. Създаване, четене и изтриване на Cookies (`ResponseCookie` & `@CookieValue`)](#1-създаване-четене-и-изтриване-на-cookies-responsecookie--cookievalue)
* [2. Работа с `HttpSession` (Съхраняване на потребителски контекст и кошница)](#2-работа-с-httpsession-съхраняване-на-потребителски-контекст-и-кошница)
* [3. Настройка на сигурността и таймаута на сесията в `application.properties`](#3-настройка-на-сигурността-и-таймаута-на-сесията-в-applicationproperties)
* [4. `@SessionAttributes` в Spring MVC контролер](#4-sessionattributes-в-spring-mvc-контролер)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. СЪЗДАВАНЕ, ЧЕТЕНЕ И ИЗТРИВАНЕ НА COOKIES (`RESPONSECOOKIE` & `@COOKIEVALUE`)

```java
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseCookie;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import java.time.Duration;

@Controller
@RequestMapping("/cookies")
public class CookieDemoController {

    // 1. Задаване на сигурна бисквитка
    @PostMapping("/language")
    public String setLanguage(@RequestParam String lang, HttpServletResponse response) {
        ResponseCookie cookie = ResponseCookie.from("preferred_lang", lang)
                .httpOnly(true)
                .secure(true)
                .sameSite("Lax")
                .path("/")
                .maxAge(Duration.ofDays(7))
                .build();

        response.addHeader(HttpHeaders.SET_COOKIE, cookie.toString());
        return "redirect:/cookies/home";
    }

    // 2. Четене на бисквитка с резервна стойност по подразбиране
    @GetMapping("/home")
    public String home(@CookieValue(value = "preferred_lang", defaultValue = "bg") String lang,
                       Model model) {
        model.addAttribute("currentLang", lang);
        return "home";
    }

    // 3. Изтриване на бисквитка (задаваме maxAge = 0!)
    @PostMapping("/clear")
    public String deleteLanguageCookie(HttpServletResponse response) {
        ResponseCookie expiredCookie = ResponseCookie.from("preferred_lang", "")
                .path("/")
                .maxAge(0)
                .build();

        response.addHeader(HttpHeaders.SET_COOKIE, expiredCookie.toString());
        return "redirect:/cookies/home";
    }
}
```

---

## 2. РАБОТА С `HTTPSESSION` (СЪХРАНЯВАНЕ НА ПОТРЕБИТЕЛСКИ КОНТЕКСТ И КОШНИЦА)

```java
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import java.util.ArrayList;
import java.util.List;

@Controller
@RequestMapping("/cart")
public class ShoppingCartController {

    @PostMapping("/add")
    public String addToCart(@RequestParam Long productId, HttpSession session) {
        @SuppressWarnings("unchecked")
        List<Long> cart = (List<Long>) session.getAttribute("cartItems");

        if (cart == null) {
            cart = new ArrayList<>();
        }
        cart.add(productId);
        session.setAttribute("cartItems", cart);

        return "redirect:/products";
    }

    @PostMapping("/checkout-complete")
    public String clearCart(HttpSession session) {
        // Премахва само конкретен ключ от сесията (запазва логнатия потребител!)
        session.removeAttribute("cartItems");
        return "redirect:/orders";
    }
}
```

---

## 3. НАСТРОЙКА НА СИГУРНОСТТА И ТАЙМАУТА НА СЕСИЯТА В `APPLICATION.PROPERTIES`

```properties
# Време на неактивност, след което сървърната сесия изтича (по подразбиране е 30m)
server.servlet.session.timeout=30m

# Глобална защита на сесийната бисквитка JSESSIONID
server.servlet.session.cookie.http-only=true
server.servlet.session.cookie.secure=true
server.servlet.session.cookie.same-site=lax
```

---

## 4. `@SESSIONATTRIBUTES` В SPRING MVC КОНТРОЛЕР

Удобен начин за многостъпкови форми (Wizard), при който атрибут от `Model` се пази автоматично в сесията до извикване на `SessionStatus.setComplete()`:

```java
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.bind.support.SessionStatus;

@Controller
@RequestMapping("/wizard")
@SessionAttributes("wizardForm")
public class RegistrationWizardController {

    @ModelAttribute("wizardForm")
    public WizardDto initWizard() {
        return new WizardDto();
    }

    @PostMapping("/finish")
    public String finish(@ModelAttribute("wizardForm") WizardDto form, SessionStatus status) {
        // Запис в базата данни...
        status.setComplete(); // Изчиства "wizardForm" от сесията!
        return "redirect:/success";
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Конструкция / Метод | Какво прави | Бележка |
| :--- | :--- | :--- |
| **`ResponseCookie.from(name, val)`** | Модерен билдер за сигурни бисквитки | Поддържа `.sameSite("Lax" / "Strict")` |
| **`@CookieValue(value = "...", defaultValue = "...")`** | Чете бисквитка в параметър на контролер | Без `defaultValue` хвърля грешка, ако липсва |
| **`cookie.maxAge(0)`** | Изтрива бисквитката от браузъра | Пътят `.path("/")` трябва да съвпада с оригиналния |
| **`session.setAttribute("key", obj)`** | Записва обект в сървърната сесия | Пази само леки данни (ID, роля, кошница) |
| **`session.removeAttribute("key")`** | Изтрива един запис от сесията | Сесията остава активна |
| **`session.invalidate()`** | Унищожава цялата сесия на сървъра | Задължително при Logout |

---
[⬅️ 04-05-00. Теория & Капани](04-05-00-State-Management.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-06-00. Spring Essentials (Events & Scheduling)](04-06-00-Spring-Essentials.md)
