**Нестор** — библиотека для работы со словоформами на русском языке. Аналог `pymorphy2`, но для C#. Основана на [словаре словоформ М.Хагена](http://www.speakrus.ru/dict2/index.htm#morph-paradigm).
* Поиск леммы и грамматического описания слова по словоформе
* Постановка слова в заданную форму
* Токенизация и лемматизация строк
* **Информация об ударениях**

# Инициализация библиотеки
`Install-Package Nestor -Version 0.5.0`
```cs
using Nestor;

var nMorph = new NestorMorph(); // загрузка библиотеки занимает некоторое время
```

# Информация о словоформе
```cs
Word[] words = nMorph.WordInfo("стали");
```
В `words` будет массив объектов `Word` со всеми вариантами слова (слова считаются разными, если у них разные леммы — то есть нормальные формы). Например, для слова *стали* это два варианта: существительное *сталь* и глагол *стать*.

## Структура объекта Word
```cs
string Stem; // неизменяемая часть слова
WordForm[] Forms; // список всех форм слова
WordForm Lemma; // нормальная форма слова
Tag Tag; // тег — грамматическое описание нормальной формы слова (см. ниже)
```

## Структура объекта WordForm
```cs
string Word; // строковое представление текущей формы слова
int Stress; // номер слога, на который приходится ударение, начиная с 1
Tag Tag; // тег — грамматическое описание формы (см. ниже)
string[] Grammemes; // граммемы словоформы в сыром виде из словаря
```

## Структура объекта Tag
Описание значений перечислений приведены ниже
```cs
Pos Pos; // часть речи
Gender Gender; // род
Number Number; // число
Case Case; // падеж
Tense Tense; // время
Person Person; // лицо
```

## Значения перечислений
### Pos — часть речи
|Часть речи|Значение|
|--|--|
|Отсутствует*|`Pos.None`|
|Имя существительное|`Pos.Noun`|
|Имя прилагательное|`Pos.Adjective`|
|Глагол|`Pos.Verb`|
|Наречие|`Pos.Adverb`|
|Имя числительное|`Pos.Numeral`|
|Причастие|`Pos.Participle`|
|Деепричастие|`Pos.Transgressive`|
|Местоимение|`Pos.Pronoun`|
|Предлог|`Pos.Preposition`|
|Союз|`Pos.Conjunction`|
|Частица|`Pos.Particle`|
|Междометие|`Pos.Interjection`|
|Предикатив|`Pos.Predicative`|
|Вводное слово|`Pos.Parenthesis`|

\* при нормальной работе слов без части речи не должно быть

### Case — падеж
|Падеж|Значение|
|--|--|
|Отсутствует|`Case.None`|
|Именительный|`Case.Nominative`|
|Родительный|`Case.Genitive`|
|Дательный|`Case.Dative`|
|Винительный|`Case.Accusative`|
|Творительный|`Case.Instrumental`|
|Предложный|`Case.Prepositional`|
|Местный (*на мосту*)|`Case.Locative`|
|Частичный/партитив (*чаю*)|`Case.Partitive`|
|Звательный (*старче*)|`Case.Vocative`|

### Gender — род
|Род|Значение|
|--|--|
|Отсутствует|`Gender.None`|
|Мужской|`Gender.Masculine`|
|Женский|`Gender.Feminine`|
|Средний|`Gender.Neuter`|
|Общий (*коллега*)|`Gender.Common`|

### Number — число
|Число|Значение|
|--|--|
|Отсутствует|`Number.None`|
|Единственное|`Number.Singular`|
|Множественное|`Number.Plural`|

### Tense — время
|Время|Значение|
|--|--|
|Отсутствует|`Tense.None`|
|Прошедшее|`Tense.Past`|
|Настоящее|`Tense.Present`|
|Будущее|`Tense.Future`|
|Инфинитив|`Tense.Infinitive`|

### Person — лицо
|Лицо|Значение|
|--|--|
|Отсутствует|`Person.None`|
|Первое|`Person.First`|
|Второе|`Person.Second`|
|Третье|`Person.Third`|

# Постановка слова в нужную форму
```cs
const string w = "красивый";
var info = _nMorph.WordInfo(w)[0];
var f = info.ClosestForm(gender: Gender.Feminine, @case: Case.Accusative, number: Number.Singular);
Console.WriteLine(f.Word); // красивую
```
```cs
const string w = "красить";
var info = _nMorph.WordInfo(w)[0];
var f = info.ClosestForm(gender: Gender.Feminine, number: Number.Singular, tense: Tense.Past);
Console.WriteLine(f.Word); // красила
```

> Документация дополняется

# Лицензия
Данный проект использует для хранения словоформ ациклический граф с помощью библиотеки [DawgSharp](https://github.com/bzaar/DawgSharp). По условиям лицензии данной библиотеки Нестор тоже попадает под GPL. Но если вы напишете свою релизацию Dawg, мою часть кода можете брать по MIT.
