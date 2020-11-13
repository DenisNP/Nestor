**Nestor.Nyms** — библиотека для быстрого получения синонимов и антонимов к словам. На текущий момент демонстрационная версия, поддерживает только леммы слов.

## Инициализация
`Install-Package Nestor.Nyms -Version 0.0.1`
```cs
var nNyms = new NestorNyms();
```

## Синонимы
```cs
HashSet<string> synonyms = nNyms.Synonyms("враг"); // неприятель, противник, ....
```
```cs
var areSynonyms = nNyms.AreSynonyms("красивый", "прекрасный"); // True
```

## Антонимы
```cs
HashSet<string> antonyms = nNyms.Antonyms("враг"); // друг
```
```cs
var areAntonyms = nNyms.AreSynonyms("большой", "маленький"); // True
```
