[⬅️ 06-07-00. Теория & Капани](06-07-00-Deployment-Setup.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)

---

# 06-07-01. Deployment Setup: Синтаксис от нулата & Cheat Sheet

**Цел:** Готови, копи-пейст шаблони за конфигуриране на `WhiteNoise`, стартиране на `Gunicorn`, писане на `Dockerfile`, `docker-compose.yml` и `Nginx` конфигурация за жив Django сървър от нулата.

## 🧭 Съдържание на темата
* [1. Настройка на WhiteNoise и проверка за сигурност (`check --deploy`)](#1-настройка-на-whitenoise-и-проверка-за-сигурност-check---deploy)
* [2. Стартиране на Gunicorn сървър](#2-стартиране-на-gunicorn-сървър)
* [3. Готов `Dockerfile` и `docker-compose.yml` (Django + PostgreSQL + Redis + Celery)](#3-готов-dockerfile-и-docker-composeyml-django--postgresql--redis--celery)
* [4. Конфигурационен файл за Nginx Reverse Proxy](#4-конфигурационен-файл-за-nginx-reverse-proxy)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. НАСТРОЙКА НА WHITENOISE И ПРОВЕРКА ЗА СИГУРНОСТ (`CHECK --DEPLOY`)

```bash
pip install gunicorn whitenoise
```

Във файла **`config/settings.py`** добави `WhiteNoiseMiddleware` веднага след `SecurityMiddleware`:
```python
MIDDLEWARE = [
    "django.middleware.security.SecurityMiddleware",
    "whitenoise.middleware.WhiteNoiseMiddleware",  # <-- Слага се точно тук!
    "django.contrib.sessions.middleware.SessionMiddleware",
    # ...
]
```

### Подготовка и проверка в терминала:
```bash
# 1. Събиране на всички CSS/JS файлове в папка staticfiles/
python manage.py collectstatic --noinput

# 2. Автоматичен одит на сигурността за продукция (проверява 15 критични настройки)
python manage.py check --deploy
```

---

## 2. СТАРТИРАНЕ НА GUNICORN СЪРВЪР

Формулата за оптимален брой работници (Workers) е: **`(2 x брой CPU ядра) + 1`**.

```bash
# Стартиране на Gunicorn с 3 работника на порт 8000 (за WSGI проекти)
gunicorn config.wsgi:application --bind 0.0.0.0:8000 --workers 3

# За ASGI (асинхронни проекти с WebSockets) се ползва Uvicorn или Gunicorn + UvicornWorker:
gunicorn config.asgi:application -k uvicorn.workers.UvicornWorker --bind 0.0.0.0:8000
```

---

## 3. ГОТОВ `DOCKERFILE` И `DOCKER-COMPOSE.YML` (DJANGO + POSTGRESQL + REDIS + CELERY)

### А. Файл `Dockerfile` в корена на проекта:
```dockerfile
FROM python:3.12-slim

ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

RUN python manage.py collectstatic --noinput

EXPOSE 8000
CMD ["gunicorn", "config.wsgi:application", "--bind", "0.0.0.0:8000", "--workers", "3"]
```

### Б. Файл `docker-compose.yml` (вдига целия стек с 1 команда `docker compose up -d --build`):
```yaml
services:
  db:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: app_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: strong_password
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine

  web:
    build: .
    command: gunicorn config.wsgi:application --bind 0.0.0.0:8000 --workers 3
    ports:
      - "8000:8000"
    env_file:
      - .env
    depends_on:
      - db
      - redis

  celery_worker:
    build: .
    command: celery -A config worker --loglevel=info
    env_file:
      - .env
    depends_on:
      - db
      - redis

volumes:
  postgres_data:
```

---

## 4. КОНФИГУРАЦИОНЕН ФАЙЛ ЗА NGINX REVERSE PROXY

Примерен файл `/etc/nginx/sites-available/django_app`:
```nginx
server {
    listen 80;
    server_name myshop.bg www.myshop.bg;

    # 1. Директно сервиране на статичните файлове от диска (без да минават през Python)
    location /static/ {
        alias /app/staticfiles/;
    }

    # 2. Препращане на всички динамични заявки към Gunicorn на порт 8000
    location / {
        proxy_pass http://127.0.0.1:8000;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Команда / Инструмент | Какво прави |
| :--- | :--- |
| **`python manage.py collectstatic`** | Събира всички статични файлове в `STATIC_ROOT` |
| **`python manage.py check --deploy`** | Проверява `settings.py` за пропуски в сигурността |
| **`gunicorn config.wsgi:application`** | Стартира многопроцесен продукционен уеб сървър |
| **`WhiteNoiseMiddleware`** | Позволява на Python да сервира компресиран CSS/JS |
| **`docker compose up -d --build`** | Изгражда и стартира Web + DB + Redis + Celery |
| **`sudo nginx -t && sudo systemctl reload nginx`** | Проверява и презарежда конфигурацията на Nginx |

---
[⬅️ 06-07-00. Теория & Капани](06-07-00-Deployment-Setup.md) | 📂 Папка: Module-06-Django-Advanced | [🏠 Начало](../../../../README.md) | [🏠 Към Главния Хъб на Ниво 2](../../README.md)
