```markdown
[⬅️ 04-01-00. Теория & Капани](04-01-00-Internet-And-HTTP.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-02-00. Spring Boot Introduction](04-02-00-Spring-Boot-Intro.md)

---

# 04-01-01. Internet Explained & HTTP Protocol: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за изпращане на HTTP заявки с вградения Java 21 `HttpClient` и модерния Spring Boot 3.2+ `RestClient`, четене на заглавия и статус кодове и конфигурация на CORS в Java от нулата.

## 🧭 Съдържание на темата
* [1. Вграден Java 21 `HttpClient` (GET и POST заявки без външни библиотеки)](#1-вграден-java-21-httpclient-get-и-post-заявки-без-външни-библиотеки)
* [2. Модерен Spring Boot 3.2+ `RestClient` (Fluent API)](#2-модерен-spring-boot-32-restclient-fluent-api)
* [3. Четене и връщане на HTTP Headers и Status Codes в Spring (`ResponseEntity`)](#3-четене-и-връщане-на-http-headers-и-status-codes-в-spring-responseentity)
* [4. Конфигурация на CORS (`Cross-Origin Resource Sharing`) в Spring Boot](#4-конфигурация-на-cors-cross-origin-resource-sharing-в-spring-boot)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ВГРАДЕН JAVA 21 `HTTPCLIENT` (GET И POST ЗАЯВКИ БЕЗ ВЪНШНИ БИБЛИОТЕКИ)

```java
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

public class NativeJavaHttpDemo {
    public static void main(String[] args) throws Exception {
        // 1. Създаваме клиента веднъж (преизползва се за всички заявки)
        HttpClient client = HttpClient.newBuilder()
                .version(HttpClient.Version.HTTP_2)
                .connectTimeout(Duration.ofSeconds(5))
                .build();

        // 2. GET заявка с Authorization Header
        HttpRequest getRequest = HttpRequest.newBuilder()
                .uri(URI.create("https://typicode.com"))
                .header("Accept", "application/json")
                .GET()
                .build();

        HttpResponse<String> getResponse = client.send(getRequest, HttpResponse.BodyHandlers.ofString());
        System.out.println("Статус: " + getResponse.statusCode());
        System.out.println("Тяло: " + getResponse.body());

        // 3. POST заявка с JSON тяло
        String jsonBody = """
                {"title": "Java 21", "body": "HTTP/2 Client", "userId": 1}
                """;

        HttpRequest postRequest = HttpRequest.newBuilder()
                .uri(URI.create("https://typicode.com"))
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(jsonBody))
                .build();

        HttpResponse<String> postResponse = client.send(postRequest, HttpResponse.BodyHandlers.ofString());
        System.out.println("Създаден статус: " + postResponse.statusCode()); // 201
    }
}
```

---

## 2. МОДЕРЕН SPRING BOOT 3.2+ `RESTCLIENT` (FLUENT API)

```java
import org.springframework.http.MediaType;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;

@Service
public class ExternalWeatherService {

    private final RestClient restClient;

    public ExternalWeatherService(RestClient.Builder builder) {
        this.restClient = builder
                .baseUrl("https://example.com")
                .defaultHeader("Accept", MediaType.APPLICATION_JSON_VALUE)
                .build();
    }

    public WeatherDto getForecast(String city) {
        return restClient.get()
                .uri("/v1/forecast?city={city}", city)
                .retrieve()
                .body(WeatherDto.class);
    }

    public WeatherDto createAlert(AlertRequestDto request) {
        return restClient.post()
                .uri("/v1/alerts")
                .contentType(MediaType.APPLICATION_JSON)
                .body(request)
                .retrieve()
                .body(WeatherDto.class);
    }
}
```

---

## 3. ЧЕТЕНЕ И ВРЪЩАНЕ НА HTTP HEADERS И STATUS CODES В SPRING (`RESPONSEENTITY`)

```java
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/demo")
public class HeadersDemoController {

    @GetMapping("/inspect")
    public ResponseEntity<String> inspectHeaders(
            @RequestHeader(value = "User-Agent", defaultValue = "Unknown") String userAgent) {

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.set("X-Powered-By", "Spring-Boot-3");

        return ResponseEntity
                .status(HttpStatus.OK)
                .headers(responseHeaders)
                .body("Твоят браузър/клиент е: " + userAgent);
    }
}
```

---

## 4. КОНФИГУРАЦИЯ НА CORS (`CROSS-ORIGIN RESOURCE SHARING`) В SPRING BOOT

Когато фронтендът (напр. Angular/React на `http://localhost:4200`) извиква Spring Boot API (на `http://localhost:8080`), браузърът блокира заявката по подразбиране заради *Same-Origin Policy*, докато не разрешиш произхода:

```java
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Configuration
public class WebCorsConfig implements WebMvcConfigurer {
    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/api/**")
                .allowedOrigins("http://localhost:4200", "http://localhost:5173")
                .allowedMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                .allowedHeaders("*")
                .allowCredentials(true);
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| HTTP Метод | Safe (Не променя данни)? | Idempotent (Повторение = същия ефект)? | Типичен Успешен Статус Код |
| :--- | :--- | :--- | :--- |
| **`GET`** | **Да** | **Да** | `200 OK` |
| **`HEAD`** | **Да** | **Да** | `200 OK` (само Headers, без Body) |
| **`POST`** | Не | **Не** (създава нов ресурс всеки път) | `201 Created` |
| **`PUT`** | Не | **Да** (пълна замяна на ресурса) | `200 OK` или `204 No Content` |
| **`PATCH`** | Не | Зависи от имплементацията | `200 OK` |
| **`DELETE`** | Не | **Да** (изтрит ресурс си остава изтрит) | `204 No Content` |

---
[⬅️ 04-01-00. Теория & Капани](04-01-00-Internet-And-HTTP.md) | 📂 Папка: Module-04-Spring-Fundamentals | [🏠 Начало](../../README.md) | [➡️ 04-02-00. Spring Boot Introduction](04-02-00-Spring-Boot-Intro.md)