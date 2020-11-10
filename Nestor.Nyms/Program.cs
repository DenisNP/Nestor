using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nestor.Nyms
{
    public static class Program
    {
        private static NestorMorph _nMorph;
        
        public static void Main()
        {
            _nMorph = new NestorMorph();

            //ParseFile("Data/synonyms.csv", "synonyms.txt");
            ParseFile("Data/antonyms.csv", "antonyms.txt");
        }

        private static void ParseFile(string fileName, string outFileName)
        {
            Console.WriteLine($"Start file {fileName}");
            var file = File.ReadAllLines(fileName);
            var result = new Dictionary<string, List<string>>();

            for (var i = 0; i < file.Length; i++)
            {
                var line = file[i];
                if (string.IsNullOrEmpty(line)) continue;

                var data = line.Split(";");
                if (data.Length < 2) continue;

                if (data[0].Length == 0 || data[0].Substring(0, 1).ToLower() != data[0].Substring(0, 1)) continue;
                
                var word = _nMorph.Clean(data[0]);
                if (!_nMorph.WordExists(word)) continue;

                var mainInfo = _nMorph.WordInfo(word)[0];
                var mainLemma = mainInfo.Lemma.Word;
                
                var nyms = data[1].Split("|")
                    .Select(n =>
                    {
                        var nw = _nMorph.Clean(n);
                        if (!_nMorph.WordExists(nw)) return null;

                        var info = _nMorph.WordInfo(nw);
                        return info.Length == 0 ? null : info[0].Lemma.Word;
                    })
                    .Where(x => x != null && x != mainLemma)
                    .ToArray();
                
                if (nyms.Length == 0) continue;

                if (!result.ContainsKey(mainLemma))
                    result.Add(mainLemma, new List<string>());

                result[mainLemma] = result[mainLemma].Concat(nyms).Distinct().ToList();

                if (i % 1000 == 0) Console.WriteLine($"{i}/{file.Length} parsed: {result.Count}");
            }
            
            File.WriteAllLines(outFileName, result.Select(r => $"{r.Key};{string.Join("|", r.Value.OrderBy(x => x))}"));
            Console.WriteLine($"Done: {result.Count}");
        }
    }
}