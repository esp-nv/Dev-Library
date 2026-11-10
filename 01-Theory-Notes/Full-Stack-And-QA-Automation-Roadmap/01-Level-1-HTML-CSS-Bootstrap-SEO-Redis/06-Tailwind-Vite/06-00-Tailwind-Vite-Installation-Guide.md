[⬅️ Назад към Индекса](README.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | [🏠 Начало](../../README.md) | [➡️ 06-01. Структура и Осите](06-01-Core-Concepts-Layout-Flexbox-Grid.md)

---

# 06-00. Инсталация и Конфигурация: Комплексно ръководство за Tailwind v4 + Vite

**Модул 06:** Напреднал Tailwind CSS v4 | **Цел:** Пълно софтуерно и практическо внедряване на Tailwind CSS v4 чрез компилационния двигател Vite.

## 🧭 Съдържание на темата
* [1. Инициализиране на проекта през Vite](#1-инициализиране-на-проекта-през-vite-vite-project-setup)
* [2. Инсталиране на пакети през npm](#2-инсталиране-на-пакети-през-node-package-manager-npm)
* [3. Настройка на Vite Плъгина и Свързване на CSS нишките](#3-настройка-на-vite-плъгина-и-свързване-на-css-нишките)
* [4. Системни капани и отстраняване на грешки](#4-системни-капани-грешки-при-компилация-missing-plugin--build-failure)
* [5. INSTALLATION CHEAT SHEET & HTML Test Snippet](#5-installation-cheat-sheet--бързи-конзолни-snippets)

---

## 1. ИНИЦИАЛИЗИРАНЕ НА ПРОЕКТА ПРЕЗ VITE
Използва се **Vite** вместо ръчно отваряне на HTML файлове от диска:
```bash
npm create vite@latest my-tailwind-project
cd my-tailwind-project
```

## 2. ИНСТАЛИРАНЕ НА ПАКЕТИ ПРЕЗ NPM
Tailwind CSS v4 се интегрира нативно с Vite чрез два основни пакета (без нужда от традиционния `postcss.config.js`):
```bash
npm install tailwindcss @tailwindcss/vite
```

## 3. НАСТРОЙКА НА VITE ПЛЪГИНА И CSS ИМПОРТ
* **`vite.config.js` / `vite.config.ts`:**
  ```typescript
  import { defineConfig } from 'vite'
  import tailwindcss from '@tailwindcss/vite'

  export default defineConfig({
    plugins: [tailwindcss()],
  })
  ```
* **Глобален CSS файл (напр. `src/style.css`):**
  ```css
  @import "tailwindcss";
  ```

## 4. СИСТЕМНИ КАПАНИ: ГРЕШКИ ПРИ КОМПИЛАЦИЯ
* **Грешка:** `Cannot find module '@tailwindcss/vite'` при изпълнение на `npm run dev`.
* **Причина:** Пропусната команда `npm install` след инициализация, поради което папката `node_modules` е празна.
* **Решение:** Изпълнете `npm install` преди стартиране на сървъра за разработка.

## 5. INSTALLATION CHEAT SHEET & HTML TEST SNIPPET
1. `npm create vite@latest my-app -- --template vanilla`
2. `cd my-app && npm install`
3. `npm install tailwindcss @tailwindcss/vite`
4. Конфигуриране на плъгина във `vite.config.js`
5. Добавяне на `@import "tailwindcss";` в CSS файла
6. `npm run dev` на порт `http://localhost:5173`

*(Пълният тестов HTML код с Tailwind v4 класове за бърза проверка на визуалния компонент може да бъде прегледан директно в изходния документ).*

---
[⬅️ Назад към Индекса](README.md) | [📂 Подпапка: 06-Tailwind-Vite](README.md) | [🏠 Начало](../../README.md) | [➡️ 06-01. Структура и Осите](06-01-Core-Concepts-Layout-Flexbox-Grid.md)

© 2026 Dev-Library / Tailwind v4 + Vite: Инсталационен Справочник
