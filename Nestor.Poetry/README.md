# Модуль для работы с поэзией
## Определение стихотворного размера
Поддерживаются базовые пять размеров силлабо-тонического стиха:
* Ямб
* Хорей
* Дактиль
* Амфибрахий
* Анапест

Использование:
```c#
var analyzer = new FootAnalyzer();
```

### Определение размера по одной строке
```c#
const string line = "тучки небесные вечные странники";
Foot foot = analyzer.FindBestFootByLine(line, out _);
Console.WriteLine(foot.Type.ToString()); // Dactyl
```

### Определение размера по всему стихотворенью
```c#
const string poem =   "Выхожу один я на дорогу;\n" +
                    + "Сквозь туман кремнистый путь блестит;\n" +
                    + "Ночь тиха. Пустыня внемлет богу,\n" +
                    + "И звезда с звездою говорит.";
Foot foot = analyzer.FindBestFootByPoem(poem);
Console.WriteLine(foot.Type.ToString()); // Chorea
```

## Оценка рифмы между двумя словами
Использование:
```c#
var analyzer = new RhymeAnalyzer();

RhymingPair pair = analyzer.ScoreRhyme("любовь", "морковь");
Console.WriteLine(pair.Score); // 0.85
```
