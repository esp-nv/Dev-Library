[⬅️ 04-04-00. Теория & Сравнение](04-04-00-Relationships.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-02. SQLAlchemy Syntax](04-04-02-SQLAlchemy-Syntax.md)

---

# 04-04-01. Django ORM Relationships: Синтаксис от нулата & Cheat Sheet

**Цел:** Точни синтактични шаблони за One-to-One, One-to-Many, Many-to-Many (включително междинни таблици `through`) и сериализация на свързани обекти в JSON/XML от нулата.

## 🧭 Съдържание на темата
* [1. One-to-One (1:1) и One-to-Many (1:N) релации](#1-one-to-one-11-и-one-to-many-1n-релации)
* [2. Many-to-Many (M:N) и Междинна таблица (`through`)](#2-many-to-many-mn-и-междинна-таблица-through)
* [3. Навигация в двете посоки (`related_name` и `__`)](#3-навигация-в-двете-посоки-related_name-и-__)
* [4. Свързани обекти в JSON и XML йерархии (Стъпка по стъпка)](#4-свързани-обекти-в-json-и-xml-йерархии-стъпка-по-стъпка)
* [5. Обобщение / Бърз Cheat Sheet](#5-обобщение--бърз-cheat-sheet)

---

## 1. ONE-TO-ONE (1:1) И ONE-TO-MANY (1:N) РЕЛАЦИИ

```python
from django.db import models

class User(models.Model):
    username = models.CharField(max_length=50)

# 1. ONE-TO-ONE (1:1) - Всеки потребител има точно 1 профил
class UserProfile(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name="profile")
    bio = models.TextField(blank=True)

# 2. ONE-TO-MANY (1:N) - Един потребител може да напише много статии
class Article(models.Model):
    author = models.ForeignKey(User, on_delete=models.CASCADE, related_name="articles")
    title = models.CharField(max_length=150)
```

---

## 2. MANY-TO-MANY (M:N) И МЕЖДИННА ТАБЛИЦА (`THROUGH`)

```python
# Вариант А: Автоматична свързваща таблица (Django я прави сам зад кулисите)
class Tag(models.Model):
    name = models.CharField(max_length=30)

class Post(models.Model):
    title = models.CharField(max_length=100)
    tags = models.ManyToManyField(Tag, related_name="posts")

# Вариант Б: Свързваща таблица с допълнителни колони (например дата на записване и оценка!)
class Student(models.Model):
    name = models.CharField(max_length=50)

class Course(models.Model):
    title = models.CharField(max_length=100)
    students = models.ManyToManyField(Student, through="Enrollment")

class Enrollment(models.Model):
    student = models.ForeignKey(Student, on_delete=models.CASCADE)
    course = models.ForeignKey(Course, on_delete=models.CASCADE)
    enrolled_on = models.DateField(auto_now_add=True)
    grade = models.DecimalField(max_digits=3, decimal_places=2, null=True)
```

---

## 3. НАВИГАЦИЯ В ДВЕТЕ ПОСОКИ (`RELATED_NAME` И `__`)

```python
# 1. От Детето към Родителя (Forward lookup):
article = Article.objects.get(id=1)
print(article.author.username)

# 2. От Родителя към Децата (Reverse lookup чрез related_name):
user = User.objects.get(id=1)
user_articles = user.articles.all()     # Връща всички статии на този потребител!

# 3. Работа с Many-to-Many (.add, .remove, .clear):
post = Post.objects.get(id=1)
tag = Tag.objects.create(name="Python")
post.tags.add(tag)                      # Добавя връзка в междинната таблица
post.tags.remove(tag)                   # Премахва връзката

# 4. Филтриране през релация с двойна долна черта (__):
articles_by_eli = Article.objects.filter(author__username="elizabeth")
```

---

## 4. СВЪРЗАНИ ОБЕКТИ В JSON И XML ЙЕРАРХИИ (СТЪПКА ПО СТЪПКА)

Когато прехвърляш релационни данни към външна система в JSON или XML, вграденият сериализатор на Django пази само ID-то на външния ключ, затова за вложени дървовидни структури се сглобява речник или XML елемент:

```python
import json
import xml.etree.ElementTree as ET

# Стъпка 1: Сглобяване на вложен JSON обект (Родител + Деца вътре)
author_payload = {
    "id": user.id,
    "username": user.username,
    "articles": [
        {"id": a.id, "title": a.title} for a in user.articles.all()
    ]
}
json_tree = json.dumps(author_payload, ensure_ascii=False, indent=2)

# Стъпка 2: Сглобяване на вложено XML дърво
author_xml = ET.Element("Author", id=str(user.id), username=user.username)
articles_node = ET.SubElement(author_xml, "Articles")

for a in user.articles.all():
    art_el = ET.SubElement(articles_node, "Article", id=str(a.id))
    art_el.text = a.title

xml_tree = ET.tostring(author_xml, encoding="unicode")
```

---

## 5. ОБОБЩЕНИЕ / БЪРЗ CHEAT SHEET

| Релация / Операция | Синтаксис в Django ORM |
| :--- | :--- |
| **One-to-One (1:1)** | `models.OneToOneField(Parent, on_delete=models.CASCADE)` |
| **One-to-Many (1:N)** | `models.ForeignKey(Parent, on_delete=models.PROTECT, related_name="items")` |
| **Many-to-Many (M:N)** | `models.ManyToManyField(OtherModel, related_name="items")` |
| **M:N с допълнителни полета** | `models.ManyToManyField(OtherModel, through="JoinModel")` |
| **Четене от родител към деца** | `parent.related_name.all()` |
| **Добавяне / Махане в M:N** | `obj.tags.add(tag)` / `obj.tags.remove(tag)` |
| **Филтриране по поле на родител** | `Child.objects.filter(parent__name="Ivan")` |

---
[⬅️ 04-04-00. Теория & Сравнение](04-04-00-Relationships.md) | 📂 Папка: Module-04-Python-ORM | [🏠 Начало](../../../../README.md) | [➡️ 04-04-02. SQLAlchemy Syntax](04-04-02-SQLAlchemy-Syntax.md)
