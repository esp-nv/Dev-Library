[⬅️ 03-02-00. Redis Caching Архитектура и Структури от данни](03-02-00-Redis-Caching-Architecture.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 04-00. Intro & Judge Exam Strategy](../04-Exam-Preparation/04-00-Intro.md)

---

# 03-02-01. Syntax-From-Scratch & Redis CLI Cheat Sheet + Docker Setup

**Раздел 03:** Technical SEO & Redis In-Memory Caching | **Цел:** Практическо стартиране на Redis сървър чрез Docker от нулата, работа с `redis-cli` за всички основни структури от данни (`String`, `Hash`, `List`, `Set`, `Sorted Set`), управление на `TTL` и трансакции (`MULTI/EXEC`), вход/изход примери и пълен табличен **Module Cheat Sheet**.

## 🧭 Съдържание на темата
* [1. Стъпка по стъпка: Стартиране на Redis в Docker и свързване с redis-cli](#1-стъпка-по-стъпка-стартиране-на-redis-в-docker-и-свързване-с-redis-cli)
* [2. Синтаксис от нулата: String, Hash, Sorted Set и TTL (Вход / Изход)](#2-синтаксис-от-нулата-string-hash-sorted-set-и-ttl-вход--изход)
* [3. Module Cheat Sheet (Пълна Redis CLI синтактична справка)](#3-module-cheat-sheet-пълна-redis-cli-синтактична-справка)
* [4. Готов за директно ползване Snippet: Конвенция за именуване на ключове & Трансакции](#4-готов-за-директно-ползване-snippet-конвенция-за-именуване-на-ключове--трансакции)
* [5. Системни капани: Командата KEYS * в продукция (Блокиране на сървъра)](#5-системни-капани-командата-keys--в-продукция-блокиране-на-сървъра)
* [6. Детайлни подмодули към Тема 03-02](#6-детайлни-подмодули-към-тема-03-02)

---

## 1. СТЪПКА ПО СТЪПКА: СТАРТИРАНЕ НА REDIS В DOCKER И СВЪРЗВАНЕ С REDIS-CLI

Най-чистият и бърз начин да вдигнеш Redis локално (на стандартен порт **`6379`**) е чрез Docker контейнер:

```bash
# 1. Изтегляне и стартиране на официалния Redis контейнер във фонов режим
docker run -d --name dev-redis -p 6379:6379 redis:7-alpine

# 2. Влизане в интерактивната конзола redis-cli вътре в контейнера
docker exec -it dev-redis redis-cli

# 3. Проверка на връзката
127.0.0.1:6379> PING
PONG
```

---

## 2. СИНТАКСИС ОТ НУЛАТА: STRING, HASH, SORTED SET И TTL (ВХОД / ИЗХОД)

### 🔹 Пример А: Кеширане на JSON с автоматично изтичане (`SET ... EX` и `TTL`)
* **Вход (`redis-cli`):**
  ```text
  SET catalog:featured '{"count":12,"currency":"EUR"}' EX 300
  GET catalog:featured
  TTL catalog:featured
  ```
* **Изход в конзолата:**
  ```text
  OK
  "{\"count\":12,\"currency\":\"EUR\"}"
  (integer) 294
  ```
  *(Ключът ще се самоунищожи автоматично след 294 секунди, освобождавайки RAM паметта.)*

### 🔹 Пример Б: Съхраняване на обект с `Hash` (`HSET` / `HGETALL`)
* **Вход (`redis-cli`):**
  ```text
  HSET user:1001 username "elena_qa" role "SDET" logins 1
  HINCRBY user:1001 logins 1
  HGETALL user:1001
  ```
* **Изход в конзолата:**
  ```text
  (integer) 3
  (integer) 2
  1) "username"
  2) "elena_qa"
  3) "role"
  4) "SDET"
  5) "logins"
  6) "2"
  ```

### 🔹 Пример В: Класация в реално време със `Sorted Set` (`ZADD` / `ZREVRANGE`)
* **Вход (`redis-cli`):**
  ```text
  ZADD exam:leaderboard 95 "Ivan" 100 "Elena" 88 "Georgi"
  ZREVRANGE exam:leaderboard 0 -1 WITHSCORES
  ```
* **Изход в конзолата (подредено автоматично по низходящ резултат):**
  ```text
  1) "Elena"
  2) "100"
  3) "Ivan"
  4) "95"
  5) "Georgi"
  6) "88"
  ```

---

## 3. MODULE CHEAT SHEET (ПЪЛНА REDIS CLI СИНТАКТИЧНА СПРАВКА)

| Структура / Категория | Основни Команди | Синтаксис и Действие |
| :--- | :--- | :--- |
| **Общи / Управление на ключове** | `EXISTS`, `DEL`, `EXPIRE`, `TTL`, `TYPE` | `EXPIRE key 60` (задава живот 60 сек.); `TTL key` (`-1` = без срок, `-2` = не съществува). |
| **String (Низове и Броячи)** | `SET`, `GET`, `SETNX`, `INCR`, `DECR` | `SET key val EX 120` (запис със срок); `INCR views:page1` (атомарно +1). |
| **Hash (Обекти)** | `HSET`, `HGET`, `HGETALL`, `HDEL`, `HINCRBY` | `HSET product:5 price 99.90 stock 14` — чете и обновява отделни полета на обект. |
| **List (Опашки и Списъци)** | `LPUSH`, `RPUSH`, `LPOP`, `RPOP`, `LRANGE` | `LPUSH queue:emails "msg1"`; `LRANGE queue:emails 0 9` (първите 10 елемента). |
| **Set (Уникални множества)** | `SADD`, `SREM`, `SMEMBERS`, `SISMEMBER`, `SCARD` | `SADD tags:post1 "redis" "seo"`; `SCARD tags:post1` (брой уникални елементи). |
| **Sorted Set (Класации)** | `ZADD`, `ZRANGE`, `ZREVRANGE`, `ZRANK`, `ZINCRBY` | `ZREVRANGE top:players 0 2 WITHSCORES` — връща Топ 3 играчи с техните точки. |
| **Сървър & Диагностика** | `PING`, `INFO memory`, `DBSIZE`, `MONITOR`, `FLUSHDB` | `INFO memory` показва заетата RAM; `FLUSHDB` изтрива текущата база (внимание!). |

---

## 4. ГОТОВ ЗА ДИРЕКТНО ПОЛЗВАНЕ SNIPPET: КОНВЕНЦИЯ ЗА ИМЕНУВАНЕ НА КЛЮЧОВЕ & ТРАНСАКЦИИ

За да не се превърне Redis в хаос от хиляди неразбираеми ключове, индустриалният стандарт използва **двоеточие (`:`) за йерархично разделяне на пространствата от имена (Namespaces)**:

```text
# ✅ Златен стандарт за структура на ключове:
<приложение>:<домейн>:<идентификатор>:<подтип>

Примери:
• devlib:course:01:metadata         (Hash с метаданни за курс 01)
• devlib:user:482:session           (String с JWT / сесиен токен + TTL 3600s)
• devlib:ratelimit:ip:192.168.1.10  (String брояч на заявки в минута + TTL 60s)
```

### 🔒 Атомарна трансакция в Redis (`MULTI` / `EXEC`):
```text
MULTI
DECR product:55:stock
LPUSH orders:queue "order_9012"
EXEC
```
*(Гарантира, че двете операции се изпълняват последователно като едно цяло без прекъсване от други клиенти.)*

---

## 5. СИСТЕМНИ КАПАНИ: КОМАНДАТА KEYS * В ПРОДУКЦИЯ (БЛОКИРАНЕ НА СЪРВЪРА)

### 🚨 Капан №1: Изпълняваш `KEYS *` в продукционна среда с милиони ключове
* **Защо е опасно:** Понеже Redis работи в **една единствена нишка**, командата `KEYS *` сканира цялата RAM памет наведнъж и **блокира напълно сървъра** за всички останали потребители, докато не приключи (може да отнеме няколко секунди, причинявайки масови Timeouts).
* **Как да го избегнеш:** В продукция никога не използвай `KEYS *` (и `FLUSHALL`). Вместо това използвай неблокиращия курсор **`SCAN 0 MATCH devlib:user:* COUNT 100`**, който обхожда ключовете на малки порции без да спира обслужването на заявки.

---
[⬅️ 03-02-00. Redis Caching Архитектура и Структури от данни](03-02-00-Redis-Caching-Architecture.md) | [📂 Предишна папка: 02. Responsive, Forms & Frameworks](../02-Responsive-Forms-And-Frameworks/README.md) | [📂 Текуща папка: 03. Technical SEO & Redis Caching](README.md) | [🏠 Начало](../../../../README.md) | [📂 Следваща папка: 04. Exam Preparation & Judge Simulations](../04-Exam-Preparation/README.md) | [➡️ 04-00. Intro & Judge Exam Strategy](../04-Exam-Preparation/04-00-Intro.md)
---
© 2026 Dev-Library / 03-02-01. Syntax-From-Scratch & Redis CLI Cheat Sheet + Docker Setup
