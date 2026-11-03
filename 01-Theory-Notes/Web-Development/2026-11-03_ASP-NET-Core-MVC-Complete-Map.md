# 📘 Глава 6: Жизнен цикъл и Архитектура на ASP.NET Core MVC
**Статус:** Обединен технически наръчник (asp-net.md + asp-net-core-mvc.md) | **Цел:** Пълен мисловен модел от HTTP заявката до базата данни и Razor екрана

---

## 🗺️ 1. ГОЛЯМАТА КАРТИНА: ПЪТЯТ НА ЕДНА ЗАЯВКА (The HTTP Request Lifecycle)
Заявката от браузъра преминава през Middleware тръбопровода и рутирането, достига до съответния контролер и екшън метод (където се извършва Model Binding и валидация), след което се обработва от бизнес услугата или базата данни (EF Core) преди да се визуализира към потребителя [Dev-Library/01-Theory-Notes/Web-Development/asp-net.md]. Пълният диаграмен модел може да бъде прегледан в референтните файлове [Dev-Library/01-Theory-Notes/Web-Development/asp-net.md].

---

## 📂 2. АВТЕНТИЧНА СТРУКТУРА НА MVC ПРОЕКТ
За да се запази разделението на отговорностите (*Separation of Concerns*), проектът се организира в папки като `Controllers`, `Data`, `Models`, `Configurations`, `ViewModels` и `Views` [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].

---

## 🛠️ 3. ЕТАПИ НА ИЗГРАЖДАНЕ И РЕАЛЕН КОД
Процесът включва дефиниране на чист домейн модел (`Entity`) [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md], Fluent API конфигурации извън него за спазване на SOLID принципите [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md], централен `ApplicationDbContext` с автоматично сканиране [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md], `ViewModel` с дата анотации за екрана [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md], конфигуриране на DI и Middleware в `Program.cs` [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md], координация през `ProductsController` (с Post/Redirect/Get патърн) [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md] и силно типизирани Razor изгледи [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md]. 
*Пълните примерни кодове за тези етапи са налични в източника [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].*

---

## ❌ 4. УЧЕБНИ КАПАНИ: КЪДЕ НАЙ-ЧЕСТО СЕ ДОПУСКАТ ГРЕШКИ
*   **Капан №1:** Директно предаване на Entity вместо ViewModel (риск от Over-Posting) [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].
*   **Капан №2:** Пропуснат атрибут `[HttpPost]` над метода за запис [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].
*   **Капан №3:** Липса на проверка `ModelState.IsValid` [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].
*   **Капан №4:** Неправилен ред на Middleware компонентите в `Program.cs` [Dev-Library/01-Theory-Notes/Web-Development/asp-net-core-mvc.md].
