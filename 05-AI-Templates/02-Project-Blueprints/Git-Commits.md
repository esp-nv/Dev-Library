# 📝 Git Commit Standards & Conventional Commits

## 📋 СПРАВОЧНИК: Конвенция за съобщения

**Формат:** `<type>(<scope>): <short description in lowercase>`
*Забележка: Описанието винаги започва с малка буква и е в сегашно време (напр. "add", а не "added").*

| Тип (`type`) | Кога се използва? (Приложение) | Пример за съобщение |
| :--- | :--- | :--- |
| **`feat`** | Добавяне на изцяло нова функционалност / нов фийчър | `feat(auth): add google oauth2 login` |
| **`fix`** | Поправка на бъг или неочаквано поведение в кода | `fix(orders): resolve null pointer in total calculation` |
| **`test`** | Добавяне на нови тестове (xUnit/Jest) или корекция на стари | `test(users): add AAA unit tests for registration` |
| **`refactor`** | Промяна в кода, която нито поправя бъг, нито добавя фийчър | `refactor(cart): simplify linq expression in repository` |
| **`style`** | Промени в дизайна или форматирането (интервали, CSS, линтери) | `style(nav): fix flexbox alignment on mobile view` |
| **`docs`** | Промени само в документацията (Markdown файлове, README, коментари) | `docs(readme): update setup instructions for dotnet core` |
| **`chore`** | Обновяване на зависимости, build инструменти или пакети (NPM/NuGet) | `chore(deps): bump fluentvalidation from 11.0 to 11.5` |

---

## 🔗 Референции за шаблоните (Свързани файлове)
* **Главен диспечерски пулт:** [Назад към 01-MASTER-BLUEPRINT.md](Main-Control-Panel-and-Trackers/01-MASTER-BLUEPRINT.md)
* **Динамичен прогрес:** [Към Project-Tracker.md](Main-Control-Panel-and-Trackers/Project-Tracker.md)
* **Писане на основен код:** [Към подшаблона Sub-Write-Code.md](Sub-Actions/Sub-Write-Code.md)
* **Писане на тестове:** [Към подшаблона Sub-Write-Tests.md](Sub-Actions/Sub-Write-Tests.md)
* **Разработка на фийчъри:** [Към подшаблона Feature-Development.md](Sub-Actions/Feature-Development.md)
