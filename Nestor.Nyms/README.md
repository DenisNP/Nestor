**Nestor.Nyms** — библиотека для быстрого получения синонимов и антонимов к словам. На текущий момент демонстрационная версия, поддерживает только леммы слов.

## Инициализация
```c#
var nNyms = new NestorNyms();
```

## Синонимы
```c#
HashSet<string> synonyms = nNyms.Synonyms("враг"); // неприятель, противник, ....
```
```c#
var areSynonyms = nNyms.AreSynonyms("красивый", "прекрасный"); // True
```

## Антонимы
```c#
HashSet<string> antonyms = nNyms.Antonyms("враг"); // друг
```
```c#
var areAntonyms = nNyms.AreAntonyms("большой", "маленький"); // True
```
