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
* **Главен контролен панел:** [Назад към MASTER-BLUEPRINT.md](../02-Project-Blueprints/MASTER-BLUEPRINT.md)
* **Генериране на код:** [Към подшаблона за Писане на код](../02-Project-Blueprints/Sub-Write-Code.md)
* **Генериране на тестове:** [Към подшаблона за Тестове](../02-Project-Blueprints/Sub-Write-Tests.md)
