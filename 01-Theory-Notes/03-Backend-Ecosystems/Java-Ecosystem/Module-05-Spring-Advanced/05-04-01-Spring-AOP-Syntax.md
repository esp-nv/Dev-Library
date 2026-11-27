[⬅️ 05-04-00. Теория & Капани](05-04-00-Spring-AOP.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-05-00. Docs, Testing & Docker](05-05-00-Docs-Testing-And-Docker.md)

---

# 05-04-01. Spring AOP (Aspect-Oriented Programming) & Interceptors: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за създаване на потребителска анотация като Pointcut (`@TrackExecutionTime`), използване на `@Before`, `@AfterReturning`, `@AfterThrowing` и `@Around`, конфигуриране на Spring MVC `HandlerInterceptor` и основните Pointcut изрази в Java от нулата.

## 🧭 Съдържание на темата
* [1. Потребителска анотация (`@TrackExecutionTime`) + `@Around` Aspect](#1-потребителска-анотация-trackexecutiontime--around-aspect)
* [2. Pointcut изрази с `@Before`, `@AfterReturning` и `@AfterThrowing`](#2-pointcut-изрази-с-before-afterreturning-и-afterthrowing)
* [3. Spring MVC `HandlerInterceptor` (Прехващане на HTTP заявки преди Controller)](#3-spring-mvc-handlerinterceptor-прехващане-на-http-заявки-преди-controller)
* [4. Най-често използваните Pointcut изрази в Spring AOP](#4-най-често-използваните-pointcut-изрази-в-spring-aop)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ПОТРЕБИТЕЛСКА АНОТАЦИЯ (`@TRACKEXECUTIONTIME`) + `@AROUND` ASPECT

Изисква зависимостта `org.springframework.boot:spring-boot-starter-aop` в `pom.xml`:

```java
import java.lang.annotation.*;

// 1. Дефинираме наша собствена анотация
@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
public @interface TrackExecutionTime {
    String label() default "";
}
```

```java
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

// 2. Създаваме Aspect, който следи за тази анотация
@Aspect
@Component
public class ExecutionTimeAspect {

    private static final Logger log = LoggerFactory.getLogger(ExecutionTimeAspect.class);

    @Around("@annotation(trackAnnotation)")
    public Object trackTime(ProceedingJoinPoint joinPoint, TrackExecutionTime trackAnnotation) throws Throwable {
        long start = System.nanoTime();
        Object result = joinPoint.proceed(); // Изпълнява истинския метод
        long durationMs = (System.nanoTime() - start) / 1_000_000;

        String tag = trackAnnotation.label().isEmpty()
                ? joinPoint.getSignature().getName()
                : trackAnnotation.label();

        log.info("[AOP METRIC] {} завърши за {} ms", tag, durationMs);
        return result;
    }
}

// 3. Използване върху всеки @Service метод:
// @TrackExecutionTime(label = "Генериране на месечен отчет")
// public ReportDto buildMonthlyReport() { ... }
```

---

## 2. POINTCUT ИЗРАЗИ С `@BEFORE`, `@AFTERRETURNING` И `@AFTERTHROWING`

```java
import org.aspectj.lang.JoinPoint;
import org.aspectj.lang.annotation.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;
import java.util.Arrays;

@Aspect
@Component
public class ServiceAuditAspect {

    private static final Logger log = LoggerFactory.getLogger(ServiceAuditAspect.class);

    // Именуван Pointcut за повторна употреба: всички методи в пакет service и подпакетите му
    @Pointcut("execution(* bg.softuni.bookstore.service..*(..))")
    public void allServiceMethods() {}

    @Before("allServiceMethods()")
    public void logMethodCall(JoinPoint joinPoint) {
        log.info("➡️ Извикване на: {} с аргументи: {}",
                joinPoint.getSignature().getName(),
                Arrays.toString(joinPoint.getArgs()));
    }

    @AfterReturning(pointcut = "allServiceMethods()", returning = "returnedValue")
    public void logMethodSuccess(JoinPoint joinPoint, Object returnedValue) {
        log.info("✅ Успешно завърши: {} | Върна: {}",
                joinPoint.getSignature().getName(), returnedValue);
    }

    @AfterThrowing(pointcut = "allServiceMethods()", throwing = "ex")
    public void logMethodException(JoinPoint joinPoint, Throwable ex) {
        log.error("❌ Грешка в метод {}: {}",
                joinPoint.getSignature().getName(), ex.getMessage());
    }
}
```

---

## 3. SPRING MVC `HANDLERINTERCEPTOR` (ПРЕХВАЩАНЕ НА HTTP ЗАЯВКИ ПРЕДИ CONTROLLER)

```java
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.context.annotation.Configuration;
import org.springframework.stereotype.Component;
import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.config.annotation.InterceptorRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Component
public class ApiKeyAuditInterceptor implements HandlerInterceptor {

    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) {
        System.out.println("Incoming Request: " + request.getMethod() + " " + request.getRequestURI());
        // Връща true -> заявката продължава към Controller-а
        // Връща false -> спира заявката веднага!
        return true;
    }
}

@Configuration
public class WebInterceptorConfig implements WebMvcConfigurer {
    private final ApiKeyAuditInterceptor auditInterceptor;

    public WebInterceptorConfig(ApiKeyAuditInterceptor auditInterceptor) {
        this.auditInterceptor = auditInterceptor;
    }

    @Override
    public void addInterceptors(InterceptorRegistry registry) {
        registry.addInterceptor(auditInterceptor)
                .addPathPatterns("/api/**")
                .excludePathPatterns("/api/v1/auth/**");
    }
}
```

---

## 4. НАЙ-ЧЕСТО ИЗПОЛЗВАНИТЕ POINTCUT ИЗРАЗИ В SPRING AOP

| Pointcut Директива | Синтаксис | Какво обхваща |
| :--- | :--- | :--- |
| **`execution(...)`** | `execution(* com.app.service.*.*(..))` | Всички методи в директните класове на пакет `service` |
| **`execution` (с подпакети)** | `execution(* com.app.service..*(..))` | Всички методи в пакет `service` и **всички негови подпакети (`..`)** |
| **`@annotation(...)`** | `@annotation(com.app.aop.TrackTime)` | Всеки метод, анотиран с `@TrackTime` |
| **`@within(...)`** | `@within(org.springframework.stereotype.Service)` | Всички методи на класове, маркирани със `@Service` |
| **Комбиниране** | `allServiceMethods() && !@annotation(NoLog)` | Логически оператори `&&`, `\|\|`, `!` |

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Анотация / Клас | Кога се изпълнява | Може ли да спре метода или да промени резултата? |
| :--- | :--- | :--- |
| **`@Before`** | Преди изпълнението на целевия метод | Не (освен ако не хвърли изключение) |
| **`@AfterReturning`** | Само след успешно завършване на метода | Не (но може да прочете `returning` стойността) |
| **`@AfterThrowing`** | Само ако методът хвърли изключение | Не (но може да логне `throwing` грешката) |
| **`@After`** | Винаги след метода (като `finally` блок) | Не |
| **`@Around`** | И преди, и след метода (обвива го изцяло) | **Да!** Контролира `joinPoint.proceed()` и `return` стойността |
| **`HandlerInterceptor`** | Преди/след `@Controller` в Spring MVC | **Да!** Ако `preHandle` върне `false`, спира заявката |

---
[⬅️ 05-04-00. Теория & Капани](05-04-00-Spring-AOP.md) | 📂 Папка: Module-05-Spring-Advanced | [🏠 Начало](../../README.md) | [➡️ 05-05-00. Docs, Testing & Docker](05-05-00-Docs-Testing-And-Docker.md)
