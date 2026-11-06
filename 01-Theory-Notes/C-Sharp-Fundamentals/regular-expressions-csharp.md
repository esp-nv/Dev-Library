# 📘 Учебник: Регулярни изрази (Regex), Групи, Lookaround и Backtracking в C#

**Файл:** `regular-expressions-csharp.md` | **Ниво:** Архитектурно ядро / C# Internals  
**Технологии:** .NET `System.Text.RegularExpressions`, NFA Engine, Named Groups, Lookahead/Lookbehind

---

## 🏗️ РАЗДЕЛ 1: КАК REGEX ДВИГАТЕЛЯТ РАБОТИ ПОД КАПАКА В .NET (NFA И CACHING)

В .NET библиотеката `System.Text.RegularExpressions` използва двигател тип **NFA (Nondeterministic Finite Automaton)** с поддръжка на Backtracking (връщане назад). Когато използваш статичните методи (`Regex.Match(...)`), .NET кешира последните 15 компилирани шаблона в паметта за по-бързо изпълнение.

```csharp
// ==================== REGEX МЕТОДИ, КОМПИЛАЦИЯ И ОПТИМИЗАЦИЯ ====================

using System;
using System.Text.RegularExpressions;

public class RegexEngineMechanics
{
    // 1. ИНСТАНЦИОНЕН REGEX СЪС ЗАЩИТА ОТ ЗАБИВАНЕ (Timeout)
    // Винаги подавай TimeSpan като 3-ти аргумент при работа с външен потребителски вход!
    private static readonly Regex EmailRegex = new Regex(
        pattern: @"^(?<user>[a-zA-Z0-9._-]+)@(?<domain>[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})\$",
        options: RegexOptions.Compiled | RegexOptions.IgnoreCase,
        matchTimeout: TimeSpan.FromMilliseconds(200) // Прекъсва търсенето при опасност от Backtracking
    );

    public void ExplainCoreOperations()
    {
        string sampleText = "Контакт: ivan.petrov@softuni.bg и admin@dev-library.org";

        // 2. ISMATCH: Бърза булева проверка (връща true/false)
        bool hasValidEmail = EmailRegex.IsMatch("ivan.petrov@softuni.bg"); // TRUE

        // 3. MATCHES: Извличане на ВСИЧКИ съвпадения от многоредов или дълъг текст
        string inlinePattern = @"(?<user>[a-zA-Z0-9._-]+)@(?<domain>[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})";
        MatchCollection matches = Regex.Matches(sampleText, inlinePattern);

        foreach (Match m in matches)
        {
            // m.Value е цялото съвпадение ("ivan.petrov@softuni.bg")
            // m.Groups["name"] достъпва конкретната именувана група
            Console.WriteLine(\$"Потребител: {m.Groups["user"].Value} | Домейн: {m.Groups["domain"].Value}");
        }

        // 4. REPLACE С LAMBDA (MatchEvaluator): Динамична трансформация на всяко съвпадение
        string censored = Regex.Replace(sampleText, inlinePattern, match =>
        {
            // Заменяме потребителското име със звездички, но запазваме домейна
            return \$"***@{match.Groups["domain"].Value}";
        });
        // Резултат: "Контакт: ***@softuni.bg и ***@dev-library.org"
    }
}
```

---

## 🧩 РАЗДЕЛ 2: ИМЕНУВАНИ ГРУПИ, BACKREFERENCES И GREEDY VS LAZY

Групите не само извличат данни в C#, но могат да се използват и **вътре в самия шаблон** (чрез Backreferences), за да търсят повтарящи се думи или съвпадащи отварящи и затварящи тагове.

```csharp
// ==================== NAMED GROUPS, BACKREFERENCES И GREEDY VS LAZY ====================

using System;
using System.Text.RegularExpressions;

public class RegexGroupsAndQuantifiers
{
    public void ExplainGroupsAndLaziness()
    {
        // 1. GREEDY (АЛЧЕН) СРЕЩУ LAZY (МЪРЗЕЛИВ) КВАНТИФИКАТОР
        string html = "<div>Първи блок</div> и <div>Втори блок</div>";

        // ❌ GREEDY (.*): Започва от първия <div> и гълта всичко чак до ПОСЛЕДНИЯ </div>!
        Match greedyMatch = Regex.Match(html, @"<div>.*</div>");
        // greedyMatch.Value == "<div>Първи блок</div> и <div>Втори блок</div>" (ГРЕШКА!)

        // ✅ LAZY (.*?): Въпросителният знак кара двигателя да спре на ПЪРВИЯ възможен </div>
        Match lazyMatch = Regex.Match(html, @"<div>.*?</div>");
        // lazyMatch.Value == "<div>Първи блок</div>" (ТОЧНО!)

        // 2. BACKREFERENCES (\k<name> или \1): Търсене на дублирани елементи
        // Шаблонът улавя дума в група 'word', търси интервал и изисква СЪЩАТА дума веднага след това!
        string textWithRepeats = "Това е е много важен важен тест.";
        string duplicatePattern = @"\b(?<word>\w+)\s+\k<word>\b";

        // Премахваме дублираните думи, като оставяме само едното срещане (\${word})
        string cleanedText = Regex.Replace(textWithRepeats, duplicatePattern, "\${word}", RegexOptions.IgnoreCase);
        // Резултат: "Това е много важен тест."

        // 3. NEGATED CHARACTER CLASSES ([^...]) - По-бързата алтернатива на .*?
        // Вместо бавното ".*?", казваме: "вземи всичко, което НЕ Е кавичка" ([^"]*)
        string config = "Server=\"ProdDB\"; User=\"Admin\";";
        MatchCollection values = Regex.Matches(config, @"(?<key>\w+)=""(?<val>[^""]*)""");
        // Извлича чисто: key="Server", val="ProdDB" и key="User", val="Admin"
    }
}
```

---

## 🔍 РАЗДЕЛ 3: LOOKAHEAD И LOOKBEHIND (ПРОВЕРКА БЕЗ КОНСУМИРАНЕ НА СИМВОЛИ)

Lookaround конструкциите проверяват дали преди или след текущата позиция има определен текст, **но без да го включват в крайния резултат (`Match.Value`)**. Това е ключово при валидация на сложни пароли.

```csharp
// ==================== POSITIVE / NEGATIVE LOOKAHEAD И LOOKBEHIND ====================

using System;
using System.Text.RegularExpressions;

public class RegexLookaroundDemo
{
    public void ExplainLookarounds()
    {
        // 1. POSITIVE LOOKAHEAD (?=...): "Трябва да е последвано от..."
        // Искаме да извлечем САМО числото на цената, но само ако след него пише " BGN"
        string prices = "Laptop: 1500 BGN, Mouse: 50 USD, Keyboard: 120 BGN";
        MatchCollection bgnPrices = Regex.Matches(prices, @"\d+(?=\s+BGN)");
        // Резултат: "1500" и "120" (без " BGN" в резултата, а "50" е игнорирано!)

        // 2. NEGATIVE LOOKAHEAD (?!...): "НЕ трябва да е последвано от..."
        // Взимаме числа, които НЕ са последвани от USD
        MatchCollection nonUsdPrices = Regex.Matches(prices, @"\b\d+\b(?!\s+USD)");

        // 3. POSITIVE LOOKBEHIND (?<=...): "Трябва да е предшествано от..."
        // Извличаме потребителското име след символа '@', без да взимаме самото '@'
        string tweet = "Благодарности на @IvanDev и @MariaQA за помощта!";
        MatchCollection mentions = Regex.Matches(tweet, @"(?<=@)\w+");
        // Резултат: "IvanDev" и "MariaQA"

        // 4. КЛАСИЧЕСКА ВАЛИДАЦИЯ НА СИЛНА ПАРОЛА С МНОЖЕСТВО LOOKAHEADS
        // Изисквания: поне 1 малка буква, поне 1 главна буква, поне 1 цифра, минимум 8 символа
        string strongPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@\(!\%*?&]{8,}\)";
        bool isStrong = Regex.IsMatch("Pass1234", strongPasswordPattern); // TRUE
    }
}
```

---

## ⚠️ РАЗДЕЛ 4: CATASTROPHIC BACKTRACKING (КАПАНЪТ НА ВЛОЖЕНИТЕ КВАНТИФИКАТОРИ)

Когато вложиш един квантификатор (`+` или `*`) вътре в друг квантификатор (напр. `(a+)+`), при несъвпадащ вход NFA двигателят започва да изпробва милиони комбинации ($O(2^n)$ сложност) и процесорът забива на 100%.

```csharp
// ==================== КАК ДА ИЗБЕГНЕМ CATASTROPHIC BACKTRACKING ====================

public class BacktrackingSafetyDemo
{
    public void ExplainDangerAndFix()
    {
        // ❌ ОПАСЕН ШАБЛОН (Nested Quantifiers):
        // При текст "aaaaaaaaaaaaaaaaaaaaaaaaX" двигателят ще тества милиони разбивания на 'a'-тата,
        // преди да осъзнае, че накрая няма 'b'!
        string evilPattern = @"^(a+)+b\$";

        // ✅ БЕЗОПАСЕН ШАБЛОН:
        // 1. Премахни вложените квантификатори: @"^a+b\$"
        // 2. Използвай Atomic Group (?>...), която забранява на двигателя да се връща назад:
        string safeAtomicPattern = @"^(?>a+)b\$";

        // 3. Винаги задавай matchTimeout в конструктора на Regex, за да получиш
        // RegexMatchTimeoutException вместо безкрайно забиване на сървъра!
    }
}
```

---

## 🎯 РАЗДЕЛ 5: АРХИТЕКТУРНИ ПРАВИЛА ЗА ИЗПИТА (CHECKLIST)

1. **Винаги използвай `@"..."` (Verbatim string)** за Regex шаблони, за да не удвояваш всяка наклонена черта (`\\d` вместо `\d`).
2. **Използвай именувани групи `(?<name>...)`** вместо номера (`Groups`), защото при добавяне на нова скоба в шаблона номерата се изместват, а имената остават сигурни.
3. **Не забравяй `^` и `$` при валидация:** Без тях `Regex.IsMatch("abc123xyz", @"\d+")` връща `true`, защото намира цифрите по средата.
4. **Заменяй бавното `.*?` с отрицателен клас `[^...]`**, когато знаеш кой символ прекратява групата (напр. `[^"]+` вместо `.*?` между кавички).
5. **Кеширай често използваните Regex обекти** в `private static readonly Regex`, ако ги викаш хиляди пъти в цикъл.

---
© 2026 Dev-Library / Пълен учебен модул по Регулярни изрази в C#
