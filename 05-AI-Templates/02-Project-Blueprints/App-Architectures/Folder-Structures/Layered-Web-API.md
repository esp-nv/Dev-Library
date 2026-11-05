# 🏛️ Шаблон: Layered Web API / MVC Дизайн

```text
MyProject/
│── src/
│   ├── WebAPI/ (или Controllers/)   # Контролери, Ендпоинти, Middleware
│   ├── Application/                 # Бизнес Логика, Services
│   │   ├── DTOs/                    # Обекти за трансфер на данни (Request/Response)
│   │   └── Interfaces/              # Специфични договори/интерфейси (напр. IUserService.cs)
│   ├── Domain/                      # Чисти сурови обекти / Домейн модели (Entities)
│   └── Infrastructure/ (или Data/)  # Техническа имплементация
│       ├── DbContext/               # EF Core контекст конфигурация (ApplicationDbContext.cs)
│       ├── Migrations/              # Автоматични DB миграции
│       └── Repositories/            # Директни SQL CRUD заявки / Специфични Хранилища
└── Tests/                           # Папка за автоматизирано тестване (xUnit/NUnit)
```

### Логически поток на данните
```text
[Потребителски вход] ──> [Controllers / API] ──> [DTO Валидация] ──> [Services / Core Логика]
                                                                            │
[Край на потока]   ◄── [Data mapping назад] ◄── [DB Context / SQL] ◄───────┘
```
