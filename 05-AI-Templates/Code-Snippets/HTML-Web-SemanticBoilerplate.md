### 📄 HTML-Web-SemanticBoilerplate.md
*Базовият, чист HTML5 скелет, съобразен със съвременните стандарти за достъпност (SEO и структура), за да не го пишеш ръчно.*

```markdown
### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Начален файл за всеки нов уеб сайт или уеб компонент.
- Модифицирай: Заглавието (`<title>`) и съдържанието вътре в семантичните тагове.

### 🛠️ ГОТОВ КОД (HTML5)
```html
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Моят Проект</title>
    <!-- Връзка с локалния CSS файл -->
    <link rel="stylesheet" href="style.css">
</head>
<body>

    <header>
        <nav>
            <ul>
                <li><a href="#home">Начало</a></li>
                <li><a href="#projects">Проекти</a></li>
            </ul>
        </nav>
    </header>

    <main>
        <section id="hero">
            <h1>Добре дошли в моята библиотека</h1>
            <p>Това е проект, по който работя и уча едновременно.</p>
        </section>
    </main>

    <footer>
        <p>&copy; 2026 Dev-Library. Всички права запазени.</p>
    </footer>

    <!-- Връзка с JS файла (зарежда се последен за по-бърз сайт) -->
    <script src="app.js"></script>
</body>
</html>
```
