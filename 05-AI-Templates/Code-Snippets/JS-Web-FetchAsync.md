### 📄 JS-Web-FetchAsync.md
*Когато правиш Уеб проекти (Етап 4 от Web-Steps) и ти трябва готовият JavaScript код за взимане на данни от Back-end (API).*


### [КОНТЕКСТ ЗА МОДИФИКАЦИЯ]
- Използвай го във Front-end-а с JavaScript, за да извикаш данни от сървъра без презареждане на страницата.
- Модифицирай: URL адреса и начина, по който визуализираш данните в конзолата или HTML.

### 🛠️ ГОТОВ КОД (JavaScript ES6+)
```javascript
// Модерен async/await модел за Fetch заявка
async function loadDataFromApi() {
    const url = 'https://example.com'; // Твоят API линк

    try {
        const response = await fetch(url);

        // Проверка дали сървърът е върнал грешка (напр. 404 или 500)
        if (!response.ok) {
            throw new Error(`Грешка при мрежова заявка: ${response.status}`);
        }

        const data = await response.json(); // Превръщане на отговора в JSON обект
        
        // Твоята модификация: Какво правиш с данните
        console.log('Успешно взети данни:', data);
        return data;

    } catch (error) {
        console.error('Възникна проблем с Fetch операцията:', error.message);
    }
}
```
